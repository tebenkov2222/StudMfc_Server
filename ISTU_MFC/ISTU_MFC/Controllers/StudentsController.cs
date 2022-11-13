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
        private readonly IRepository _repository;

        public StudentsController(ILogger<StudentsController> logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [Authorize(Roles = "Student")]
        public IActionResult Home()
        {
            var model = _repository.GetDevisionsList(_repository.UserId);
            return View(model);
        }

        [Authorize(Roles = "Student")]
        public IActionResult Notifications()
        {
            var model = _repository.GetTableMessages(_repository.UserId);
            return View(model);
        }

        [Authorize(Roles = "Student")]
        public IActionResult Profile()
        {
            var model = _repository.GetStudentProfileModel(_repository.UserId);
            return View(model);
        }

        [Authorize(Roles = "Student")]
        public IActionResult About(string sub_id, string info)
        {
            ViewData["Information"] = info;
            var model = _repository.GetSubdivisionInfo(Int32.Parse(sub_id));
            return View(model);
        }

        [Authorize(Roles = "Student")]
        public IActionResult Servise(int servId)
        {
            var model = _repository.GetServisesInfo(servId);
            ViewData["id"] = servId;
            return View(model);
        }

        [Authorize(Roles = "Student")]
        public IActionResult RegService(int servId, string name)
        {
            var model = _repository.GetStudentProfileModel(_repository.UserId);
            ViewData["name"] = name;
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Authorize(Roles = "Student")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}