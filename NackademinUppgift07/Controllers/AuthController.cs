using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NackademinUppgift07.Models;
using NackademinUppgift07.ViewModels;

namespace NackademinUppgift07.Controllers
{
    public class AuthController : Controller
    {
	    private readonly TomasosContext context;

	    public AuthController(TomasosContext context)
	    {
		    this.context = context;
	    }

	    #region Actions
		public IActionResult Index()
        {
            return View();
        }

		[HttpGet]
	    public IActionResult Login()
	    {
		    return View();
	    }

		[HttpPost]
		[AutoValidateAntiforgeryToken]
	    public IActionResult Login(ViewLogin user)
		{
			if (ModelState.IsValid)
				return RedirectToAction("Index");

			return View();
		}

		[HttpGet]
	    public IActionResult Register()
	    {
		    return View();
	    }

	    [HttpPost]
	    [AutoValidateAntiforgeryToken]
	    public IActionResult Register(Kund kund)
	    {
		    return View();
		}

	    public IActionResult Logout()
	    {
		    return RedirectToAction("Index", "Tomasos");
	    }
		#endregion
	}
}