using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NackademinUppgift07.DataModels;

namespace NackademinUppgift07.Controllers
{
	public partial class AdminController
	{
		public IActionResult ListOrders()
		{
			return View();
		}

		[HttpPost]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> ToggleOrderLevererad(int id)
		{
			Bestallning bestallning = await dbContext.Bestallning.FirstOrDefaultAsync(b => b.BestallningId == id);

			if (bestallning == null)
				return Json(new {success = false});

			bestallning.Levererad = !bestallning.Levererad;
			await dbContext.SaveChangesAsync();

			return Json(new
			{
				success = true,
				bestallning.BestallningId,
				bestallning.Levererad,
			});
		}
	}
}