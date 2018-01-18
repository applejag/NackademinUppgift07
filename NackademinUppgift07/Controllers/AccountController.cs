using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NackademinUppgift07.DataModels;
using NackademinUppgift07.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace NackademinUppgift07.Controllers
{
    public class AccountController : Controller
    {

	    private readonly UserManager<ApplicationUser> userManager;
	    private readonly SignInManager<ApplicationUser> signInManager;
	    private readonly TomasosContext dbContext;

	    public AccountController(
			TomasosContext dbContext,
		    UserManager<ApplicationUser> userManager,
		    SignInManager<ApplicationUser> signInManager)
	    {
		    this.dbContext = dbContext;
		    this.userManager = userManager;
		    this.signInManager = signInManager;
	    }

		[Authorize]
		public async Task<IActionResult> Index()
		{
			ApplicationUser currentUser = await userManager.GetUserAsync(User);

			return View(new UserRegisterModel(currentUser));
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		[Authorize]
	    public async Task<IActionResult> Index(UserRegisterModel model)
		{
			// Ignore properties
			ModelState.Remove(nameof(model.UserName));
			ModelState.Remove(nameof(model.Password));
			ModelState.Remove(nameof(model.PasswordConfirm));

			ApplicationUser currentUser = await userManager.GetUserAsync(User);

			if (ModelState.IsValid)
			{
				// Update user
				model.UserName = currentUser.UserName;
				model.ApplyToApplicationUser(currentUser);
				await dbContext.SaveChangesAsync();
			}

			return View(new UserRegisterModel(currentUser));
		}

		[HttpGet]
		public IActionResult Login()
		{
			if (signInManager.IsSignedIn(User))
				return RedirectToAction("Index");

			return View();
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
	    public async Task<IActionResult> Login(ViewLogin login)
		{
			if (signInManager.IsSignedIn(User))
				return RedirectToAction("Index");

			if (!ModelState.IsValid)
				return View(login);

			SignInResult result = await signInManager.PasswordSignInAsync(login.AnvandarNamn, login.Losenord, false, false);

			if (!result.Succeeded)
			{
				ModelState.AddModelError(nameof(login.AnvandarNamn), "Felaktig inloggning.");
				return View(login);
			}

			return RedirectToAction("Index");
		}

		[HttpGet]
		public IActionResult Register()
		{
			if (signInManager.IsSignedIn(User))
				return RedirectToAction("Index");

			return View();
		}

		[HttpPost]
	    [AutoValidateAntiforgeryToken]
	    public async Task<IActionResult> Register(UserRegisterModel register)
		{
		    if (signInManager.IsSignedIn(User))
			    return RedirectToAction("Index");

			if (!ModelState.IsValid)
				return View(register);

			var user = new ApplicationUser();
			register.ApplyToApplicationUser(user);

			IdentityResult result = await userManager.CreateAsync(user, register.Password);

			if (!result.Succeeded)
			{
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError(
						error.Description.IndexOf("password", StringComparison.CurrentCultureIgnoreCase) != -1
							? nameof(register.Password)
							: nameof(register.UserName), error.Description);
				}

				return View(register);
			}

			await signInManager.SignOutAsync();
			await signInManager.SignInAsync(user, false);

		    return RedirectToAction("Index");
	    }

	    public async Task<IActionResult> Logout()
	    {
		    await signInManager.SignOutAsync();

		    return RedirectToAction("Index", "Tomasos");
	    }
	}
}