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
			ViewData["Title"] = "Ny maträtt";
			ViewData["Legend"] = "Skapa ny maträtt";

			var model = new ViewMatrattModel();
		    await FillMatrattModelAsync(model);

		    return View(model);
	    }

		[HttpPost]
		[AutoValidateAntiforgeryToken]
	    public async Task<IActionResult> CreatePizza(ViewMatrattModel model)
		{
			ViewData["Title"] = "Ny maträtt";
			ViewData["Legend"] = "Skapa ny maträtt";

			if (model == null)
				model = new ViewMatrattModel();

			await FillMatrattModelAsync(model);

			// Valid validation?
			if (!ModelState.IsValid)
				return View("CreatePizza", model);

			// Valid MatrattTyp?
			if (!await dbContext.MatrattTyp.AnyAsync(t => t.MatrattTyp1 == model.MatrattTyp))
			{
				ModelState.AddModelError(nameof(model.MatrattTyp), "Var god ange en giltig kategori!");
				return View("CreatePizza", model);
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

			ViewData["Message"] = $"Maträtten {model.MatrattNamn} har sparats!";

			return View("CreatePizza", model);
		}
		
		public async Task<IActionResult> AlterPizza(int id)
		{
			Matratt maträtt = await dbContext.Matratt
				.Include(m => m.MatrattProdukt).ThenInclude(mp => mp.Produkt)
				.SingleOrDefaultAsync(m => m.MatrattId == id);

			if (maträtt == null)
				return RedirectToAction("Index");

			ViewData["Title"] = "Ändra " + maträtt.MatrattNamn;
			ViewData["Legend"] = "Ändra på " + maträtt.MatrattNamn;

			// Convert to viewmodel
			var model = new ViewMatrattModel
			{
				MatrattId = maträtt.MatrattId,
				Beskrivning = maträtt.Beskrivning,
				MatrattNamn = maträtt.MatrattNamn,
				MatrattTyp = maträtt.MatrattTyp,
				Pris = maträtt.Pris,
			};

			await FillMatrattModelAsync(model, maträtt.MatrattProdukt.Select(mp => mp.ProduktId)
				.ToList());

			return View("CreatePizza", model);
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> AlterPizza(ViewMatrattModel model)
		{
			if (model == null)
				return RedirectToAction("Index");

			Matratt maträtt = await dbContext.Matratt
				.Include(m => m.MatrattProdukt).ThenInclude(mp => mp.Produkt)
				.SingleOrDefaultAsync(m => m.MatrattId == model.MatrattId);

			if (maträtt == null)
				return RedirectToAction("Index");

			ViewData["Title"] = "Ändra " + maträtt.MatrattNamn;
			ViewData["Legend"] = "Ändra på " + maträtt.MatrattNamn;

			await FillMatrattModelAsync(model);

			// Valid validation?
			if (!ModelState.IsValid)
				return View("CreatePizza", model);

			// Valid MatrattTyp?
			if (!await dbContext.MatrattTyp.AnyAsync(t => t.MatrattTyp1 == model.MatrattTyp))
			{
				ModelState.AddModelError(nameof(model.MatrattTyp), "Var god ange en giltig kategori!");
				return View("CreatePizza", model);
			}

			maträtt.MatrattNamn = model.MatrattNamn;
			maträtt.Beskrivning = model.Beskrivning;
			maträtt.Pris = model.Pris;
			maträtt.MatrattTyp = model.MatrattTyp;
			
			maträtt.MatrattProdukt = await (from product in dbContext.Produkt
				where model.Produkts.Any(p => p.Selected && p.Value == product.ProduktId.ToString())
				select maträtt.MatrattProdukt.FirstOrDefault(mp => mp.ProduktId == product.ProduktId)
				?? new MatrattProdukt
				{
					Matratt = maträtt,
					Produkt = product,
				}).ToListAsync();

			await dbContext.SaveChangesAsync();

			ViewData["Message"] = $"Maträtten {model.MatrattNamn} har sparats!";

			return View("CreatePizza", model);
		}

		public async Task<IActionResult> RemovePizza(int id)
		{
			Matratt maträtt = await dbContext.Matratt
				.Include(m => m.MatrattProdukt).ThenInclude(mp => mp.Produkt)
				.SingleOrDefaultAsync(m => m.MatrattId == id);

			if (maträtt == null)
				return RedirectToAction("Index", "Tomasos");

			foreach (MatrattProdukt produkt in maträtt.MatrattProdukt)
			{
				dbContext.MatrattProdukt.Remove(produkt);
			}
			dbContext.Matratt.Remove(maträtt);
			await dbContext.SaveChangesAsync();

			return RedirectToAction("Index", "Tomasos");
		}

		private async Task FillMatrattModelAsync(ViewMatrattModel model, IReadOnlyList<int> selectedProdukts = null)
		{
			model.MatrattTypes = new SelectList(await dbContext.MatrattTyp.ToListAsync(),
				dataValueField: nameof(MatrattTyp.MatrattTyp1),
				dataTextField: nameof(MatrattTyp.Beskrivning),
				selectedValue: model.MatrattTyp);
			
			// Not my proudest works
			List<int> tmp = model.Produkts?
				.Where(p => int.TryParse(p.Value, out int _))
				.Select(p => int.Parse(p.Value))
				.ToList()
				?? new List<int>();

			selectedProdukts = (selectedProdukts?.Union(tmp) ?? tmp)
				.ToList();

			model.Produkts = await dbContext.Produkt
				.Select(produkt => new SelectListItem
			{
				Value = produkt.ProduktId.ToString(),
				Text = produkt.ProduktNamn,
				Selected = selectedProdukts.Contains(produkt.ProduktId),
			}).ToListAsync();
		}
    }
}