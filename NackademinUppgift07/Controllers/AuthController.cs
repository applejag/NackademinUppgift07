using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NackademinUppgift07.Models;
using NackademinUppgift07.ViewModels;

namespace NackademinUppgift07.Controllers
{
    public abstract class AuthController : Controller
    {
	    protected readonly TomasosContext context;

	    private Kund _currentKund;
	    public Kund CurrentKund
	    {
		    get => _currentKund;
		    set => _currentKund = 
			    ViewBag.Kund = value;
	    }

		public bool IsLoggedIn => CurrentKund != null;


	    protected AuthController(TomasosContext context)
	    {
		    this.context = context;
	    }

	    protected virtual async Task Initialize()
	    {
		    CurrentKund = await AuthGetCurrentUser();
	    }

		#region Actions
		public async Task<IActionResult> Account()
		{
			await Initialize();

			if (!IsLoggedIn)
				return RedirectToAction("Login");

			return View(new ViewKund(CurrentKund));
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
	    public async Task<IActionResult> Account(ViewKund changed)
		{
			await Initialize();

			if (!IsLoggedIn)
				return RedirectToAction("Login");

			if (!ModelState.IsValid)
				return View(changed);

			if (CurrentKund.Losenord != changed.OldLosenord)
			{
				ModelState.AddModelError(nameof(changed.OldLosenord), "Felaktigt lösenord.");
				return View(changed);
			}

			if (!string.IsNullOrEmpty(changed.NewLosenord))
			{
				CurrentKund.Losenord = changed.NewLosenord;
			}

			CurrentKund.Namn = changed.Namn;
			CurrentKund.Email = changed.Email;
			CurrentKund.Postnr = changed.Postnr;
			CurrentKund.Postort = changed.Postort;
			CurrentKund.Gatuadress = changed.Gatuadress;
			CurrentKund.Telefon = changed.Telefon;

			await context.SaveChangesAsync();
			
			return View(new ViewKund(CurrentKund));
		}

		[HttpGet]
		public async Task<IActionResult> Login()
		{
			await Initialize();

			if (IsLoggedIn)
				return RedirectToAction("Account");

			return View();
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
	    public async Task<IActionResult> Login(ViewLogin login)
		{
			await Initialize();

			if (IsLoggedIn)
				return RedirectToAction("Account");

			if (!ModelState.IsValid)
				return View(login);

			Kund kund = await context.Kund.SingleOrDefaultAsync(k =>
				k.AnvandarNamn == login.AnvandarNamn);

			if (kund == null)
			{
				// Login failed : username
				ModelState.AddModelError(nameof(login.AnvandarNamn), "Felaktigt användarnamn.");
				AuthLogout();
				return View(login);
			}

			if (kund.Losenord != login.Losenord)
			{
				// Login failed : password
				ModelState.AddModelError(nameof(login.Losenord), "Felaktigt lösenord.");
				AuthLogout();
				return View(login);
			}

			// Login success
			AuthLogin(kund.KundId);
			return RedirectToAction("Account");
		}

		[HttpGet]
		public async Task<IActionResult> Register()
		{
			await Initialize();

			if (IsLoggedIn)
				return RedirectToAction("Account");

			return View();
		}

		[HttpPost]
	    [AutoValidateAntiforgeryToken]
	    public async Task<IActionResult> Register(Kund kund)
		{
			await Initialize();

		    if (IsLoggedIn)
			    return RedirectToAction("Account");

			if (!ModelState.IsValid)
				return View(kund);

		    context.Kund.Add(kund);
		    await context.SaveChangesAsync();

			AuthLogin(kund.KundId);

		    return RedirectToAction("Account");
	    }

	    public IActionResult Logout()
	    {
			AuthLogout();

		    return RedirectToAction("Index", "Tomasos");
	    }
		#endregion

	    protected void AuthLogin(int id)
	    {
			HttpContext.Session.SetInt32("Auth", id);
		}

	    protected void AuthLogout()
	    {
		    HttpContext.Session.Remove("Auth");
	    }

	    protected int? AuthGetCurrentID()
	    {
		    return HttpContext.Session.GetInt32("Auth");
	    }

	    protected async Task<Kund> AuthGetCurrentUser()
	    {
		    int? id = AuthGetCurrentID();
		    if (!id.HasValue) return null;

		    return await context.Kund
			    .SingleOrDefaultAsync(k => k.KundId == id.Value);
		}
	}
}