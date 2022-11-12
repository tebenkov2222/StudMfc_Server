using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ISTU_MFC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ISTU_MFC.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(ILogger<StudentsController> logger)
        {
            _logger = logger;
        }

        [Authorize(Roles = "Student")]
        public IActionResult Home()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult Notifications()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult About()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult Servise()
        {
            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult RegService()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Authorize(Roles = "Student")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}