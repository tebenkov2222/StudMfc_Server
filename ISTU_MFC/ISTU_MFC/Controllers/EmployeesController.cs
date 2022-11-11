using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ISTU_MFC.Models;

namespace ISTU_MFC.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(ILogger<EmployeesController> logger)
        {
            _logger = logger;
        }

        public IActionResult WorkWithDoc()
        {
            return View();
        }

        public IActionResult DocGenerator()
        {
            return View();
        }

        public IActionResult ServiceConstructor()
        {
            return View();
        }

        public IActionResult RequestGenerator()
        {
            return View();
        }

        public IActionResult DownloadGeneration()
        {
            return View();
        }

        public IActionResult ChangeStatus()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
