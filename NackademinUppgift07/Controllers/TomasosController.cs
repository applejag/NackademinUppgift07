using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NackademinUppgift07.DataModels;
using NackademinUppgift07.Models;
using Newtonsoft.Json;

namespace NackademinUppgift07.Controllers
{
    public partial class TomasosController : Controller
    {

	    protected readonly TomasosContext context;
	    protected readonly UserManager<ApplicationUser> userManager;
	    protected readonly SignInManager<ApplicationUser> signInManager;

	    public bool IsSignedIn => signInManager.IsSignedIn(User);

		public TomasosController(
		    TomasosContext context,
		    UserManager<ApplicationUser> userManager,
		    SignInManager<ApplicationUser> signInManager)
	    {
		    this.context = context;
		    this.userManager = userManager;
		    this.signInManager = signInManager;
	    }

		protected async Task Initialize()
		{
			ViewBag.Cart = new SavedCart(HttpContext);
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

		    var cart = new SavedCart(HttpContext);

		    for (var i = 0; i < cart.orders.Length; i++)
		    {
			    if (cart.orders[i].foodId == id && cart.orders[i].count > 0)
				    cart.orders[i].count--;
		    }

			cart.SaveCart(HttpContext);

			return RedirectToAction("ViewCart");
	    }

	    public async Task<IActionResult> ClearCart()
	    {
		    await Initialize();

			new SavedCart().SaveCart(HttpContext);

		    return RedirectToAction("ViewCart");
	    }

	    public async Task<IActionResult> ViewCart()
	    {
			await Initialize();

			return View(await new SavedCart(HttpContext).ConvertToOrder(context));
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

			var currentCart = new SavedCart(HttpContext);

		    if (currentCart.TotalCount == 0)
			    return RedirectToAction("ViewCart");

		    ApplicationUser user = await userManager.GetUserAsync(User);

			Bestallning cart = await currentCart.ConvertToOrder(context);
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
			new SavedCart().SaveCart(HttpContext);

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
			Bestallning cart = await new SavedCart(HttpContext).ConvertToOrder(context);
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

			new SavedCart(cart).SaveCart(HttpContext);
		}

	}
}