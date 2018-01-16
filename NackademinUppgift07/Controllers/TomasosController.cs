using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NackademinUppgift07.Models;
using Newtonsoft.Json;

namespace NackademinUppgift07.Controllers
{
    public class TomasosController : AuthController
    {

	    private List<Matratt> _currentCart;
		public List<Matratt> CurrentCart
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
		    CurrentCart = await SessionLoadCart();
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
				SessionAddToCart(maträtt);

		    return RedirectToAction("Index");
	    }

	    public async Task<IActionResult> ViewCart()
	    {
			await Initialize();

			return View(ViewBag.Cart);
		}
		#endregion

		protected void SessionAddToCart(Matratt maträtt)
		{
			CurrentCart.Add(maträtt);
			SessionSaveCart(CurrentCart);
		}

		protected void SessionSaveCart(IEnumerable<Matratt> cart)
		{
			var savedCart = new SavedCart(cart);
			string serialized = JsonConvert.SerializeObject(savedCart);

			HttpContext.Session.SetString("Cart", serialized);
		}

		protected async Task<List<Matratt>> SessionLoadCart()
		{
			string serialized = HttpContext.Session.GetString("Cart");

			if (serialized == null)
				return new List<Matratt>();

			var savedCart = JsonConvert.DeserializeObject<SavedCart>(serialized);

			return await savedCart.ConvertToCart(context);
		}

	    public struct SavedCart
	    {
		    public int[] IDs;

		    public SavedCart(IEnumerable<Matratt> cart)
		    {
				IDs = cart.Select(m => m.MatrattId).ToArray();
		    }

		    public async Task<List<Matratt>> ConvertToCart(TomasosContext context)
		    {
			    if (IDs == null)
				    return new List<Matratt>();

			    return await (from mat in context.Matratt
					           join id in IDs on mat.MatrattId equals id
					           select mat)
				           .Include(m => m.MatrattTypNavigation)
				           .Include(m => m.MatrattProdukt).ThenInclude(p => p.Produkt)
				           .ToListAsync() ?? new List<Matratt>();
		    }
		}
	}
}