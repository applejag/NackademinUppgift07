using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NackademinUppgift07.Controllers
{
    public class TomasosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}