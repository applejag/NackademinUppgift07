using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NackademinUppgift07.DataModels;
using NackademinUppgift07.ViewModels;

namespace NackademinUppgift07.Controllers
{
	public partial class AdminController
	{
		[HttpGet]
		public async Task<IActionResult> AlterProducts(int? id)
		{
			Produkt model = id.HasValue
				? await dbContext.Produkt.SingleOrDefaultAsync(p => p.ProduktId == id.Value)
				: null;

			ViewData["Message"] = TempData["Message"];

			return View(model);
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> AlterProducts(Produkt model)
		{
			if (!ModelState.IsValid)
				return View(model);

			Produkt product = await dbContext.Produkt.SingleOrDefaultAsync(p => p.ProduktId == model.ProduktId);

			if (product == null)
			{
				await dbContext.Produkt.AddAsync(model);
				ViewData["Message"] = $"Ingrediensen {model.ProduktNamn} har lagts till!";
			}
			else
			{
				product.ProduktNamn = model.ProduktNamn;
				ViewData["Message"] = "Ingrediensens namn har ändrats!";
			}

			await dbContext.SaveChangesAsync();

			return View(product);
		}

		[HttpGet]
		public async Task<IActionResult> RemoveProduct(int id)
		{
			Produkt product = await dbContext.Produkt
				.Include(p => p.MatrattProdukt)
				.SingleOrDefaultAsync(p => p.ProduktId == id);

			if (product == null)
				return RedirectToAction("AlterProducts");

			int count = product.MatrattProdukt.Count;
			if (count > 0)
			{
				TempData["Message"] = $"Kan ej ta bort {product.ProduktNamn}. Den används på {count}st maträtter.";
				return RedirectToAction("AlterProducts", new {id});
			}

			dbContext.Produkt.Remove(product);
			await dbContext.SaveChangesAsync();

			TempData["Message"] = $"Ingrediensen {product.ProduktNamn} har tagits bort!";
			return RedirectToAction("AlterProducts");
		}
	}
}