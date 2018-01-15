using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NackademinUppgift07.Models;

namespace NackademinUppgift07.Controllers
{
    public class TomasosController : Controller
    {
	    private readonly TomasosContext context;

	    public TomasosController(TomasosContext context)
	    {
		    this.context = context;
	    }

        public async Task<IActionResult> Index()
        {
	        List<Matratt> maträtter = await context.Matratt
				.Include(m => m.MatrattTypNavigation)
				.Include(m => m.MatrattProdukt).ThenInclude(m => m.Produkt)
				.ToListAsync();

            return View(maträtter);
        }

	    public async Task<IActionResult> AddToCart(int id)
	    {
		    Matratt maträtt = await context.Matratt
				.SingleOrDefaultAsync(m => m.MatrattId == id);

			if (maträtt != null)
				SessionAddToCart(maträtt);

		    return RedirectToAction("Index");
	    }

	    private void SessionAddToCart(Matratt maträtt)
	    {
		    
	    }

	    //private async Task<List<Matratt>> SessionLoadCart()
	    //{ 
	    //}
    }
}