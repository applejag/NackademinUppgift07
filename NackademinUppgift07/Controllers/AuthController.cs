using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NackademinUppgift07.DataModels;
using NackademinUppgift07.Models;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace NackademinUppgift07.Controllers
{
    public partial class TomasosController
    {


		#region Actions
		[Authorize]
		public async Task<IActionResult> Account()
		{
			await Initialize();

			ApplicationUser currentUser = await userManager.GetUserAsync(User);

			return View(new UserRegisterModel(currentUser));
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		[Authorize]
	    public async Task<IActionResult> Account(UserRegisterModel model)
		{
			await Initialize();

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
				await context.SaveChangesAsync();
			}

			return View(new UserRegisterModel(currentUser));
		}

		[HttpGet]
		public async Task<IActionResult> Login()
		{
			await Initialize();

			if (signInManager.IsSignedIn(User))
				return RedirectToAction("Account");

			return View();
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
	    public async Task<IActionResult> Login(ViewLogin login)
		{
			await Initialize();

			if (signInManager.IsSignedIn(User))
				return RedirectToAction("Account");

			if (!ModelState.IsValid)
				return View(login);

			SignInResult result = await signInManager.PasswordSignInAsync(login.AnvandarNamn, login.Losenord, false, false);

			if (!result.Succeeded)
			{
				ModelState.AddModelError(nameof(login.AnvandarNamn), "Felaktig inloggning.");
				return View(login);
			}

			return RedirectToAction("Account");
		}

		[HttpGet]
		public async Task<IActionResult> Register()
		{
			await Initialize();

			if (signInManager.IsSignedIn(User))
				return RedirectToAction("Account");

			return View();
		}

		[HttpPost]
	    [AutoValidateAntiforgeryToken]
	    public async Task<IActionResult> Register(UserRegisterModel register)
		{
			await Initialize();

		    if (signInManager.IsSignedIn(User))
			    return RedirectToAction("Account");

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

		    return RedirectToAction("Account");
	    }

	    public async Task<IActionResult> Logout()
	    {
		    await signInManager.SignOutAsync();

		    return RedirectToAction("Index", "Tomasos");
	    }
		#endregion
	}
}