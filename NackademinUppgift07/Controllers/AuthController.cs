using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NackademinUppgift07.Models;
using NackademinUppgift07.ViewModels;

namespace NackademinUppgift07.Controllers
{
    public class AuthController : Controller
    {
	    protected readonly TomasosContext context;

	    public AuthController(TomasosContext context)
	    {
		    this.context = context;
	    }

	    #region Actions
		public async Task<IActionResult> Index()
		{
			Kund kund = await AuthGetCurrentUser();
			ViewBag.Kund = kund;

			if (kund == null)
	            return RedirectToAction("Login");

			return View(new ViewKund(kund));
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
	    public async Task<IActionResult> Index(ViewKund changed)
		{
			Kund kund = await AuthGetCurrentUser();
			ViewBag.Kund = kund;

			if (kund == null)
				return RedirectToAction("Login");

			if (!ModelState.IsValid)
				return View(changed);

			if (kund.Losenord != changed.OldLosenord)
			{
				ModelState.AddModelError(nameof(changed.OldLosenord), "Felaktigt lösenord.");
				return View(changed);
			}

			if (!string.IsNullOrEmpty(changed.NewLosenord))
			{
				kund.Losenord = changed.NewLosenord;
			}

			kund.Namn = changed.Namn;
			kund.Email = changed.Email;
			kund.Postnr = changed.Postnr;
			kund.Postort = changed.Postort;
			kund.Gatuadress = changed.Gatuadress;
			kund.Telefon = changed.Telefon;

			await context.SaveChangesAsync();
			
			return View(new ViewKund(kund));
		}

		[HttpGet]
	    public async Task<IActionResult> Login()
	    {
		    Kund kund = await AuthGetCurrentUser();
		    if (kund != null)
			    return RedirectToAction("Index");

			return View();
	    }

		[HttpPost]
		[AutoValidateAntiforgeryToken]
	    public async Task<IActionResult> Login(ViewLogin login)
		{
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
			return RedirectToAction("Index");
		}

		[HttpGet]
	    public async Task<IActionResult> Register()
		{
			if (await AuthGetCurrentUser() != null)
				return RedirectToAction("Index");

			return View();
		}

	    [HttpPost]
	    [AutoValidateAntiforgeryToken]
	    public async Task<IActionResult> Register(Kund kund)
	    {
		    if (await AuthGetCurrentUser() != null)
			    return RedirectToAction("Index");

			if (!ModelState.IsValid)
				return View(kund);

		    context.Kund.Add(kund);
		    await context.SaveChangesAsync();

			AuthLogin(kund.KundId);

		    return RedirectToAction("Index");
	    }

	    public IActionResult Logout()
	    {
			AuthLogout();

		    return RedirectToAction("Index", "Tomasos");
	    }
		#endregion

	    internal void AuthLogin(int id)
	    {
			HttpContext.Session.SetInt32("Auth", id);
		}

	    internal void AuthLogout()
	    {
		    HttpContext.Session.Remove("Auth");
	    }

	    internal int? AuthGetCurrentID()
	    {
		    return AuthGetCurrentID(HttpContext);
	    }

	    internal static int? AuthGetCurrentID(HttpContext httpContext)
	    {
		    return httpContext.Session.GetInt32("Auth");
		}

	    internal async Task<Kund> AuthGetCurrentUser()
	    {
		    return await AuthGetCurrentUser(HttpContext, context);
	    }

	    internal static async Task<Kund> AuthGetCurrentUser(HttpContext httpContext, TomasosContext dbContext)
	    {
			int? id = AuthGetCurrentID(httpContext);
		    if (!id.HasValue) return null;

		    return await dbContext.Kund
			    .SingleOrDefaultAsync(k => k.KundId == id.Value);
		}
	}
}