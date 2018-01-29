using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NackademinUppgift07.DataModels;
using NackademinUppgift07.Models;
using NackademinUppgift07.Utility;

namespace NackademinUppgift07.Controllers
{
	public partial class AdminController
	{
		public IActionResult ListUsers()
		{
			return View();
		}

		public async Task<IActionResult> PromoteUser(string id)
		{
			ApplicationUser user = await dbContext.Users.SingleOrDefaultAsync(u => u.Id == id);

			if (user == null)
				return RedirectToAction("ListUsers");

			if (await userManager.IsInRoleAsync(user, UserRole.Admin))
			{
				TempData["Message"] = "Du saknar behörighet för att ändra rollen på en Admin.";
				return RedirectToAction("ListUsers");
			}

			if (await userManager.IsInRoleAsync(user, UserRole.PremiumUser))
			{
				TempData["Message"] = "Användaren är redan Premium.";
				return RedirectToAction("ListUsers");
			}

			IdentityResult result = await userManager.AddToRoleAsync(user, UserRole.PremiumUser);

			TempData["Message"] = result.Succeeded
				? "Användaren är nu Premium."
				: $"Något gick fel vid ändring av roll, \"{string.Join("\", \"", result.Errors.Select(e => e.Description))}\".";

			return RedirectToAction("ListUsers");
		}

		public async Task<IActionResult> DemoteUser(string id)
		{
			ApplicationUser user = await dbContext.Users.SingleOrDefaultAsync(u => u.Id == id);

			if (user == null)
				return RedirectToAction("ListUsers");

			if (await userManager.IsInRoleAsync(user, UserRole.Admin))
			{
				TempData["Message"] = "Du saknar behörighet för att ändra rollen på en Admin.";
				return RedirectToAction("ListUsers");
			}

			if (!await userManager.IsInRoleAsync(user, UserRole.PremiumUser))
			{
				TempData["Message"] = "Användaren är redan bara Regular.";
				return RedirectToAction("ListUsers");
			}

			IdentityResult result = await userManager.RemoveFromRoleAsync(user, UserRole.PremiumUser);

			TempData["Message"] = result.Succeeded
				? "Användaren är nu Regular."
				: $"Något gick fel vid ändring av roll, \"{string.Join("\", \"", result.Errors.Select(e => e.Description))}\".";

			return RedirectToAction("ListUsers");
		}

		public async Task<IActionResult> RemoveUser(string id)
		{
			ApplicationUser user = await dbContext.Users.SingleOrDefaultAsync(u => u.Id == id);

			if (user == null)
				return RedirectToAction("ListUsers");

			if (await userManager.IsInRoleAsync(user, UserRole.Admin))
			{
				TempData["Message"] = "Du saknar behörighet för att ta bort en Admin.";
				return RedirectToAction("ListUsers");
			}

			IdentityResult result = await userManager.DeleteAsync(user);

			TempData["Message"] = result.Succeeded
				? "Användaren borttagen."
				: $"Något gick fel vid borttagning av användare, \"{string.Join("\", \"", result.Errors.Select(e => e.Description))}\".";

			return RedirectToAction("ListUsers");
		}
	}
}