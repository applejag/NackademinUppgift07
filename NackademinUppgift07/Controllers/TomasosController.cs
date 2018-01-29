using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NackademinUppgift07.DataModels;
using NackademinUppgift07.Models;
using NackademinUppgift07.Models.Services;
using NackademinUppgift07.Utility;

namespace NackademinUppgift07.Controllers
{
    public class TomasosController : Controller
    {

	    public bool UserIsPremium => User.IsInRole(UserRole.PremiumUser) || User.IsInRole(UserRole.Admin);

	    private readonly TomasosContext dbContext;
	    private readonly UserManager<ApplicationUser> userManager;
	    private readonly ICartManager cartManager;

		public TomasosController(
		    TomasosContext dbContext,
		    UserManager<ApplicationUser> userManager,
			ICartManager cartManager)
	    {
		    this.dbContext = dbContext;
		    this.userManager = userManager;
		    this.cartManager = cartManager;
	    }

	    #region Actions
	    public async Task<IActionResult> Index()
	    {
		    ViewData["Title"] = "Alla maträtter";

		    List<Matratt> matratts = await dbContext.Matratt
			    .Include(m => m.MatrattTypNavigation)
			    .Include(m => m.MatrattProdukt).ThenInclude(m => m.Produkt).ToListAsync();

		    return View(matratts);
	    }

	    public async Task<IActionResult> Category(string beskrivning)
	    {
		    if (string.IsNullOrEmpty(beskrivning))
			    return RedirectToAction("Index");

		    // Try filter
		    MatrattTyp typ = await dbContext.MatrattTyp
			    .FirstOrDefaultAsync(m =>
				    m.Beskrivning.IndexOf(beskrivning, StringComparison.CurrentCultureIgnoreCase) != -1);

		    // Invalid filter?
		    if (typ == null)
			    return RedirectToAction("Index");

			// Filter items
		    ViewData["Title"] = typ.Beskrivning;

		    List<Matratt> matratts = await dbContext.Matratt
			    .Include(m => m.MatrattTypNavigation)
			    .Include(m => m.MatrattProdukt).ThenInclude(m => m.Produkt)
			    .Where(m => typ == null || m.MatrattTyp == typ.MatrattTyp1)
			    .ToListAsync();

		    return View("Index", matratts);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
	    public async Task<IActionResult> AddToCart(int id)
	    {
		    Matratt maträtt = await dbContext.Matratt
				.SingleOrDefaultAsync(m => m.MatrattId == id);

			if (maträtt != null)
				cartManager.AddToCart(maträtt.MatrattId);

		    return Json(new
		    {
			    success = maträtt != null,
				cartSize = cartManager.SavedCart.TotalCount,
		    });
	    }

		public IActionResult RemoveFromCart(int id)
		{
			cartManager.RemoveFromCart(id);

			return RedirectToAction("ViewCart");
		}

		public IActionResult ClearCart()
		{
			cartManager.ClearCart();

			return RedirectToAction("ViewCart");
		}

		public async Task<IActionResult> ViewCart()
	    {
		    ApplicationUser user = await userManager.GetUserAsync(User);

			return View(await cartManager.GetBestallningAsync(
				kundPremium: UserIsPremium,
				kundPoints: user?.Points ?? 0));
		}

		[Authorize]
	    public async Task<IActionResult> ViewOrder(int id)
	    {
		    ApplicationUser user = await userManager.GetUserAsync(User);

		    Bestallning cartInQuestion = await dbContext.Bestallning
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

		    Bestallning cart = await cartManager.GetBestallningAsync(
				kundPremium: UserIsPremium,
				kundPoints: user.Points);

		    cart.Kund = user;
		    user.Points += cart.TotalCount * Bestallning.POINTS_FOR_PIZZA_ORDER;
		    dbContext.Bestallning.Add(cart);
		    await dbContext.SaveChangesAsync();

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

			ApplicationUser filledUser = await dbContext.Users
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