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
using ISTU_MFC.Models;
using ISTU_MFC.ViewModels;
using Repository;


namespace ISTU_MFC.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ILogger<StudentsController> _logger;
        private UserContext db;
        private readonly IRepository _repository;

        public StudentsController(ILogger<StudentsController> logger, UserContext context, IRepository repository)
        {
            _logger = logger;
            db = context;
            _repository = repository;
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
            var res = _repository.GetStudentInfo(_repository.UserId);
            var dict = new Dictionary<string, string>(); 
            dict["name"] = res[1][3];
            dict["family"] = res[1][2];
            dict["secondName"] = res[1][4];;
            dict["numBil"] = res[1][1];
            dict["group"] = res[1][5];
            return View(dict);
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