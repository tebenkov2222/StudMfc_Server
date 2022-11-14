using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ISTU_MFC.Models;
using ISTU_MFC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Repository;

namespace ISTU_MFC.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly IRepository _repository;
        private readonly IWebHostEnvironment _appEnvironment;
        
        public EmployeesController(ILogger<EmployeesController> logger, IRepository repository, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _repository = repository;
            _appEnvironment = appEnvironment;
        }
        
        [Authorize(Roles = "Employee")]
        [HttpGet]
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
        public IActionResult RequestGenerator(string req_id)
        {
            ViewData["req_id"] = req_id;
            var user = _repository.GetStudentByRequest(Int32.Parse(req_id));
            ViewData["name"] = $"{user.Family} {user.Name} {user.SecondName}";
            ViewData["group"] = user.Group;
            ViewData["studId"] = user.StudId;
            var model = _repository.GetRequestFeelds(Int32.Parse(req_id));
            _repository.ChangeRequestState(Int32.Parse(req_id), _repository.UserId , "processed");
            return View(model);
        }

        [Authorize(Roles = "Employee")]
        public IActionResult DownloadGeneration(int request_id)
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public IActionResult ChangeStatus(string request_id)
        {
            ViewData["request_id"] = request_id;
            return View();
        }
        
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public IActionResult ChangeStatus(ChangeStatusModel model)
        {
            _repository.ChangeRequestState(Int32.Parse(model.request_id), _repository.UserId , model.status);
            _repository.CreateMessage(Int32.Parse(model.request_id), _repository.UserId, model.message);
            return View();
        }
        
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Authorize(Roles = "Employee")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        [Authorize(Roles = "Employee")]
        public IActionResult Notifications()
        {
            var model = _repository.GetTableMessages(_repository.UserId);
            return View(model);
        }
        
        public IActionResult Download()
        {
            string file_path = Path.Combine(_appEnvironment.ContentRootPath, "File/Test.txt");
            string file_type = "application/txt";
            string file_name = "Test.txt";
            return PhysicalFile(file_path, file_type, file_name);
        }
    }
}
