using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NackademinUppgift07.Models;
using Newtonsoft.Json;

namespace NackademinUppgift07.Controllers
{
    public class TomasosController : AuthController
    {

	    private SavedCart _currentCart;
		public SavedCart CurrentCart
		{
			get => _currentCart;
			set => _currentCart =
				ViewBag.Cart = value;
		}

	    public TomasosController(TomasosContext context)
			: base(context)
	    { }

	    protected override async Task Initialize()
	    {
		    await base.Initialize();
		    CurrentCart = SessionLoadCart();
	    }

	    #region Actions
		public async Task<IActionResult> Index()
        {
	        await Initialize();

	        List<Matratt> maträtter = await context.Matratt
				.Include(m => m.MatrattTypNavigation)
				.Include(m => m.MatrattProdukt).ThenInclude(m => m.Produkt)
				.ToListAsync();

			return View(maträtter);
        }

	    public async Task<IActionResult> AddToCart(int id)
	    {
		    await Initialize();

		    Matratt maträtt = await context.Matratt
				.SingleOrDefaultAsync(m => m.MatrattId == id);

			if (maträtt != null)
				await SessionAddToCart(maträtt);

		    return RedirectToAction("Index");
	    }

	    public async Task<IActionResult> ViewCart()
	    {
			await Initialize();

			return View(await CurrentCart.ConvertToOrder(context));
		}

	    public async Task<IActionResult> ViewOrder(int id)
	    {
		    await Initialize();

		    if (!IsLoggedIn)
			    return RedirectToAction("Login");

		    Bestallning cartInQuestion = await context.Bestallning
			    .Include(b => b.Kund)
			    .Include(b => b.BestallningMatratt).ThenInclude(bm => bm.Matratt).ThenInclude(m => m.MatrattTypNavigation)
			    .Include(b => b.BestallningMatratt).ThenInclude(bm => bm.Matratt).ThenInclude(m => m.MatrattProdukt)
			    .ThenInclude(mp => mp.Produkt)
			    .SingleOrDefaultAsync(b => b != null && b.BestallningId == id && b.KundId == CurrentKund.KundId);

			if (cartInQuestion == null)
			    return RedirectToAction("ViewCart");

		    return View("ViewCart", cartInQuestion);
	    }

	    public async Task<IActionResult> OrderCart()
	    {
		    await Initialize();

		    if (!IsLoggedIn)
			    return RedirectToAction("ViewCart");

		    if (CurrentCart.TotalCount == 0)
			    return RedirectToAction("ViewCart");

		    Bestallning cart = await CurrentCart.ConvertToOrder(context);
		    context.Attach(CurrentKund);
		    cart.Kund = CurrentKund;
		    cart.KundId = CurrentKund.KundId;

			foreach (BestallningMatratt bestallningMatratt in cart.BestallningMatratt)
			{
				context.Attach(bestallningMatratt.Matratt);
			}

		    cart.Levererad = false;
		    cart.Totalbelopp = cart.BestallningMatratt
			    .Sum(bm => bm.Antal * bm.Matratt.Pris);
		    cart.BestallningDatum = DateTime.Now;

		    context.Bestallning.Add(cart);
		    await context.SaveChangesAsync();

			// Reset cart
			SessionSaveCart(new SavedCart());

		    return RedirectToAction("ViewOrder", new
		    {
			    id = cart.BestallningId,
		    });
	    }
		#endregion

		protected async Task SessionAddToCart(Matratt maträtt)
		{
			Bestallning cart = await CurrentCart.ConvertToOrder(context);
			BestallningMatratt group = cart.BestallningMatratt
				.SingleOrDefault(g => g.MatrattId == maträtt.MatrattId);

			if (group == null)
				// New group
				cart.BestallningMatratt.Add(new BestallningMatratt
				{
					Matratt = maträtt,
					MatrattId = maträtt.MatrattId,
					Antal = 1,
				});
			else
				// Increment existing
				group.Antal++;

			SessionSaveCart(new SavedCart(cart));
		}

		protected void SessionSaveCart(SavedCart cart)
		{
			string serialized = JsonConvert.SerializeObject(cart);

			HttpContext.Session.SetString("Cart", serialized);
		}

		protected SavedCart SessionLoadCart()
		{
			string serialized = HttpContext.Session.GetString("Cart");

			if (serialized == null)
				return new SavedCart();

			var savedCart = JsonConvert.DeserializeObject<SavedCart>(serialized);

			return savedCart;
		}

		public struct SavedCart
	    {
		    public (int foodId, int count)[] orders;
		    public int TotalCount => orders?.Sum(o => o.count) ?? 0;

		    public SavedCart(Bestallning cart)
		    {
				orders = (from b in cart.BestallningMatratt
						  select (foodId: b.Matratt.MatrattId, count: b.Antal))
						  .ToArray();
		    }

		    public async Task<Bestallning> ConvertToOrder(TomasosContext context)
		    {
			    if (orders == null)
				    return new Bestallning();

			    var matratts = context.Matratt
					.Include(m => m.MatrattTypNavigation)
				    .Include(m => m.MatrattProdukt).ThenInclude(p => p.Produkt);

			    return new Bestallning
			    {
				    BestallningMatratt =
					    await (from mat in matratts
						    join order in orders on mat.MatrattId equals order.foodId
						    select new BestallningMatratt
						    {
							    Matratt = mat,
							    MatrattId = mat.MatrattId,
							    Antal = order.count
						    }).ToListAsync(),
			    };
		    }
		}
	}
}