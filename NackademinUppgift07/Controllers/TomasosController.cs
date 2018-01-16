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
	    public TomasosController(TomasosContext context)
			: base(context)
	    { }

	    #region Actions
		public async Task<IActionResult> Index()
        {
	        List<Matratt> maträtter = await context.Matratt
				.Include(m => m.MatrattTypNavigation)
				.Include(m => m.MatrattProdukt).ThenInclude(m => m.Produkt)
				.ToListAsync();

	        ViewBag.Cart = await SessionLoadCart();
			ViewBag.Kund = await SessionGetKund();

			return View(maträtter);
        }

	    public async Task<IActionResult> AddToCart(int id)
	    {
		    Matratt maträtt = await context.Matratt
				.SingleOrDefaultAsync(m => m.MatrattId == id);

			if (maträtt != null)
				await SessionAddToCart(maträtt);

		    return RedirectToAction("Index");
	    }

	    public async Task<IActionResult> ViewCart()
	    {
			ViewBag.Kund = await SessionGetKund();
		    ViewBag.Cart = await SessionLoadCart();

			return View(ViewBag.Cart);
		}
	    #endregion

	    private async Task<Kund> SessionGetKund()
	    {
		    return await AuthController.AuthGetCurrentUser(HttpContext, context);
	    }

	    private async Task SessionAddToCart(Matratt maträtt)
	    {
		    List<Matratt> cart = await SessionLoadCart();

			cart.Add(maträtt);

			SessionSaveCart(cart);
	    }

		private void SessionSaveCart(IEnumerable<Matratt> cart)
		{
			int[] ids = cart.Select(m => m.MatrattId).ToArray();
			string serialized = JsonConvert.SerializeObject(ids);

			HttpContext.Session.SetString("Cart", serialized);
		}

		private async Task<List<Matratt>> SessionLoadCart()
		{
			string serialized = HttpContext.Session.GetString("Cart");

			if (serialized == null)
				return new List<Matratt>();

			var ids = JsonConvert.DeserializeObject<int[]>(serialized);

			if (ids == null)
				return new List<Matratt>();

			return await (from mat in context.Matratt
				join id in ids on mat.MatrattId equals id
				select mat)
				.Include(m => m.MatrattTypNavigation)
				.Include(m => m.MatrattProdukt).ThenInclude(p => p.Produkt)
				.ToListAsync() ?? new List<Matratt>();
		}
	}
}