using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ISTU_MFC.Models;
using Microsoft.AspNetCore.Authorization;
using Repository;

namespace ISTU_MFC.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly IRepository _repository;
        public EmployeesController(ILogger<EmployeesController> logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [Authorize(Roles = "Employee")]
        public IActionResult WorkWithDoc()
        {
            var requests = _repository.GetRequests(_repository.UserId);
            return View(requests);
        }

        [Authorize(Roles = "Employee")]
        public IActionResult DocGenerator()
        {
            return View();
        }

        [Authorize(Roles = "Employee")]
        public IActionResult ServiceConstructor()
        {
            return View();
        }

        [Authorize(Roles = "Employee")]
        public IActionResult RequestGenerator()
        {
            return View();
        }

        [Authorize(Roles = "Employee")]
        public IActionResult DownloadGeneration()
        {
            return View();
        }

        [Authorize(Roles = "Employee")]
        public IActionResult ChangeStatus()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Authorize(Roles = "Employee")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
