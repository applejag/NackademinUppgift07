using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NackademinUppgift07.DataModels;
using NackademinUppgift07.Models;
using NackademinUppgift07.ViewModels;

namespace NackademinUppgift07.Controllers
{
	[AuthorizeRoles(UserRole.Admin)]
	public partial class AdminController : Controller
	{
		private readonly TomasosContext dbContext;
		private readonly UserManager<ApplicationUser> userManager;

		public AdminController(TomasosContext dbContext,
			UserManager<ApplicationUser> userManager)
		{
			this.dbContext = dbContext;
			this.userManager = userManager;
		}

		public IActionResult Index()
		{
			return View();
		}
	}
}