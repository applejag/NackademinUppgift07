using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NackademinUppgift07.Models;
using Newtonsoft.Json;

namespace NackademinUppgift07.Controllers
{
    public partial class TomasosController : Controller
    {

	    private SavedCart _currentCart;
		public SavedCart CurrentCart
		{
			get => _currentCart;
			set => _currentCart =
				ViewBag.Cart = value;
		}

	    protected async Task Initialize()
	    {
		    CurrentCart = SessionLoadCart();
		    ViewBag.MaträttTypes = await context.MatrattTyp.ToListAsync();
	    }

	    #region Actions
	    public async Task<IActionResult> Index(string beskrivning)
	    {
		    await Initialize();

		    ViewData["Title"] = "Alla maträtter";

			// Initial query
		    IIncludableQueryable<Matratt, Produkt> matratts = context.Matratt
			    .Include(m => m.MatrattTypNavigation)
			    .Include(m => m.MatrattProdukt).ThenInclude(m => m.Produkt);

			// All items
		    if (string.IsNullOrEmpty(beskrivning))
			    return View(await matratts.ToListAsync());

			// Try filter
			MatrattTyp typ = await context.MatrattTyp
			    .FirstOrDefaultAsync(m =>
				    m.Beskrivning.IndexOf(beskrivning, StringComparison.CurrentCultureIgnoreCase) != -1);

			// Invalid filter?
		    if (typ == null)
			    return RedirectToAction("Index", new
			    {
				    beskrivning = string.Empty,
			    });

			// Filtered items
		    ViewData["Title"] = typ.Beskrivning;
		    return View(await matratts
				.Where(m => typ == null || m.MatrattTyp == typ.MatrattTyp1)
			    .ToListAsync());
	    }

	    public async Task<IActionResult> AddToCart(int id, string source)
	    {
		    await Initialize();

		    Matratt maträtt = await context.Matratt
				.SingleOrDefaultAsync(m => m.MatrattId == id);

			if (maträtt != null)
				await SessionAddToCart(maträtt);

		    return RedirectToAction("Index", new
		    {
			    beskrivning = source,
		    });
	    }

	    public async Task<IActionResult> RemoveFromCart(int id)
	    {
		    await Initialize();

		    SavedCart cart = SessionLoadCart();

		    for (var i = 0; i < cart.orders.Length; i++)
		    {
			    if (cart.orders[i].foodId == id && cart.orders[i].count > 0)
				    cart.orders[i].count--;
		    }

			SessionSaveCart(cart);

		    return RedirectToAction("ViewCart");
	    }

	    public async Task<IActionResult> ClearCart()
	    {
		    await Initialize();

			SessionSaveCart(new SavedCart());

		    return RedirectToAction("ViewCart");
	    }

	    public async Task<IActionResult> ViewCart()
	    {
			await Initialize();

			return View(await CurrentCart.ConvertToOrder(context));
		}

		[Authorize]
	    public async Task<IActionResult> ViewOrder(int id)
	    {
		    await Initialize();

		    ApplicationUser user = await userManager.GetUserAsync(User);

		    Bestallning cartInQuestion = await context.Bestallning
			    .Include(b => b.Kund)
			    .Include(b => b.BestallningMatratt).ThenInclude(bm => bm.Matratt).ThenInclude(m => m.MatrattTypNavigation)
			    .Include(b => b.BestallningMatratt).ThenInclude(bm => bm.Matratt).ThenInclude(m => m.MatrattProdukt)
			    .ThenInclude(mp => mp.Produkt)
			    .SingleOrDefaultAsync(b => b != null && b.BestallningId == id && b.KundId == user.Id);

			if (cartInQuestion == null)
			    return RedirectToAction("ViewCart");

		    return View("ViewCart", cartInQuestion);
	    }

		[Authorize]
	    public async Task<IActionResult> OrderCart()
	    {
		    await Initialize();

		    if (CurrentCart.TotalCount == 0)
			    return RedirectToAction("ViewCart");

		    ApplicationUser user = await userManager.GetUserAsync(User);

			Bestallning cart = await CurrentCart.ConvertToOrder(context);
		    context.Attach(user);
		    cart.Kund = user;
		    cart.KundId = user.Id;

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

		[Authorize]
	    public async Task<IActionResult> ListOrders()
	    {
		    await Initialize();

		    ApplicationUser user = await userManager.GetUserAsync(User);

			ApplicationUser filledUser = await context.Users
				.Include(k => k.Bestallning)
					.ThenInclude(b => b.BestallningMatratt)
					.ThenInclude(bm => bm.Matratt)
					.ThenInclude(m => m.MatrattTypNavigation)
				.SingleAsync(k => k.Id == user.Id);

		    return View(filledUser);
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
			cart.orders = cart.orders?.Where(o => o.count >= 0).ToArray();
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