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
using NackademinUppgift07.Models.Services;
using Newtonsoft.Json;

namespace NackademinUppgift07.Controllers
{
    public partial class TomasosController : Controller
    {

	    protected readonly TomasosContext context;
	    protected readonly UserManager<ApplicationUser> userManager;
	    protected readonly SignInManager<ApplicationUser> signInManager;
	    protected readonly ICartManager cartManager;

		public TomasosController(
		    TomasosContext context,
		    UserManager<ApplicationUser> userManager,
		    SignInManager<ApplicationUser> signInManager,
			ICartManager cartManager)
	    {
		    this.context = context;
		    this.userManager = userManager;
		    this.signInManager = signInManager;
		    this.cartManager = cartManager;
	    }

	    #region Actions
	    public async Task<IActionResult> Index(string beskrivning)
	    {
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
		    Matratt maträtt = await context.Matratt
				.SingleOrDefaultAsync(m => m.MatrattId == id);

			if (maträtt != null)
				cartManager.AddToCart(maträtt.MatrattId);

			return RedirectToAction("Index", new
		    {
			    beskrivning = source,
		    });
	    }

	    public async Task<IActionResult> RemoveFromCart(int id)
	    {
			cartManager.RemoveFromCart(id);

			return RedirectToAction("ViewCart");
	    }

	    public async Task<IActionResult> ClearCart()
	    {
			cartManager.ClearCart();

		    return RedirectToAction("ViewCart");
	    }

	    public async Task<IActionResult> ViewCart()
	    {
			return View(await cartManager.GetBestallningAsync());
		}

		[Authorize]
	    public async Task<IActionResult> ViewOrder(int id)
	    {
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
		    if (cartManager.SavedCart.TotalCount == 0)
			    return RedirectToAction("ViewCart");

		    ApplicationUser user = await userManager.GetUserAsync(User);

		    Bestallning cart = await cartManager.GetBestallningAsync();
		    cart.Kund = user;

		    context.Bestallning.Add(cart);
		    await context.SaveChangesAsync();

			// Reset cart
			cartManager.ClearCart();

		    return RedirectToAction("ViewOrder", new
		    {
			    id = cart.BestallningId,
		    });
	    }

		[Authorize]
	    public async Task<IActionResult> ListOrders()
	    {
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

	}
}