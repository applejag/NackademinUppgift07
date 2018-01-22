using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NackademinUppgift07.DataModels;
using NackademinUppgift07.Models;
using NackademinUppgift07.ViewModels;

namespace NackademinUppgift07.Controllers
{
	[AuthorizeRoles(UserRole.Admin)]
    public class AdminController : Controller
	{

		private readonly TomasosContext dbContext;

	    public AdminController(TomasosContext dbContext)
	    {
		    this.dbContext = dbContext;
	    }

        public IActionResult Index()
        {
            return View();
        }

	    public async Task<IActionResult> CreatePizza()
	    {
			var model = new ViewMatrattModel();
		    await FillMatrattModel(model);

		    return View(model);
	    }

		[HttpPost]
	    public async Task<IActionResult> CreatePizza(ViewMatrattModel model)
		{
			if (model == null)
				model = new ViewMatrattModel();

			await FillMatrattModel(model);

			// Valid validation?
			if (!ModelState.IsValid)
				return View(model);

			// Valid MatrattTyp?
			if (!await dbContext.MatrattTyp.AnyAsync(t => t.MatrattTyp1 == model.MatrattTyp))
			{
				ModelState.AddModelError(nameof(model.MatrattTyp), "Var god ange en giltig kategori!");
				return View(model);
			}

			var matratt = new Matratt
			{
				MatrattNamn = model.MatrattNamn,
				Beskrivning = model.Beskrivning,
				Pris = model.Pris,
				MatrattTyp = model.MatrattTyp,
				MatrattProdukt = null,
			};

			matratt.MatrattProdukt = await (from product in dbContext.Produkt
				where model.Produkts.Any(p => p.Selected && p.Value == product.ProduktId.ToString())
				select new MatrattProdukt
				{
					Matratt = matratt,
					Produkt = product,
				}).ToListAsync();

			await dbContext.Matratt.AddAsync(matratt);
			await dbContext.SaveChangesAsync();

			ViewData["Message"] = $"Maträtten {model.MatrattNamn} har lagts till!";

			return View(model);
		}

		private async Task FillMatrattModel(ViewMatrattModel model)
		{
			model.MatrattTypes = new SelectList(await dbContext.MatrattTyp.ToListAsync(),
				dataValueField: nameof(MatrattTyp.MatrattTyp1),
				dataTextField: nameof(MatrattTyp.Beskrivning),
				selectedValue: model.MatrattTyp);

			model.Produkts = await dbContext.Produkt.Select(produkt => new SelectListItem
			{
				Value = produkt.ProduktId.ToString(),
				Text = produkt.ProduktNamn,
				Selected = model.Produkts != null && model.Produkts.Any(p =>
					p.Selected && p.Value.ToString() == produkt.ProduktId.ToString()),
			}).ToListAsync();
		}
    }
}