using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Aspose.Words;
using Documents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ISTU_MFC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ISTU_MFC.Models;
using ISTU_MFC.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ModelsData;
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

        [HttpGet]
        [Authorize(Roles = "Student")]
        public IActionResult Home()
        {
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
            var model = _repository.GetHomepageModel(userId);
            model.Requests = model.Requests.OrderByDescending(t => t.CreateDateTime).ToList();
            return View(model);
        }
        
        [HttpPost]
        public IActionResult Home(StudentHomePost model)
        {
            return RedirectToAction("About", new{sub_id = model.Sub_id, info = model.Info});
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public IActionResult Notifications()
        {
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
            var model = _repository.GetTableMessages(userId);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public IActionResult Notifications(AboutModel model)//прининимает случайную модель, нужно для того, чтобы
        //была разница в объявлении функции
        {
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
            _repository.ChangeMessagesStatus(userId);
            return RedirectToAction("Notifications");
        }

        [Authorize(Roles = "Student")]
        public IActionResult Profile()
        {
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
            var model = _repository.GetStudentProfileModel(userId);
            return View(model);
        }

        [Authorize(Roles = "Student")]
        public IActionResult About(string sub_id, string info)
        {
            var model = new AboutModel()
            {
                Sub_Id = sub_id,
                Info = info,
                Servises = _repository.GetSubdivisionInfo(Int32.Parse(sub_id))
            };
             return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public IActionResult ShowService(int servId, int subdivisionServiceId, string formLink)
        { 
            var model = _repository.GetServicesInfo(servId);
            model.Id = servId;
            model.SubdivisionServiceId = subdivisionServiceId;
            model.FormLink = formLink;
            return RedirectToAction("Service", new {servId = model.Id, subdivisionServiceId = model.SubdivisionServiceId, formLink = model.FormLink});
        }
        [HttpGet]
        public IActionResult Service(int servId, int subdivisionServiceId, string formLink)
        { 
            var model = _repository.GetServicesInfo(servId);
            model.Id = servId;
            model.SubdivisionServiceId = subdivisionServiceId;
            model.FormLink = formLink;
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Student")]
        public IActionResult OpenRegService (ServiceModel model)
        {
            return RedirectToAction("RegService", new { servId = model.Id, subdivisionServiceId = model.SubdivisionServiceId, name = model.Name });
        }
        
        [HttpPost]
        [Authorize(Roles = "Student")]
        public IActionResult RegService(int servId, int subdivisionServiceId, string name)
        {
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
            var documentsController = new DocumentsController(_repository); // вот работаем с документами
            //открываем шаблон
            var linkToDocument = _repository.GetLinkToDocumentByServiceId(servId);

            var template = documentsController.
                OpenDocumentAsTemplateByName(linkToDocument);
            // получаются поля для заполнения документа
            var fieldNames = template.GetFieldNames(); 
            //получаем значения для заполнения полей
            var valueFields = documentsController.FieldsController.GetValueFields(fieldNames, userId);
            // составление листа с модельками для отправки в бд
            var fields = valueFields.Select(field => new FieldsModel() { Name = field.Key, Value = field.Value, Malually_fiiled = false}).ToList();
            var fieldsOnView = new Dictionary<string,FieldsModel>(); // documentsController.FieldsController.GetFieldsOnViewByNames(fields);
            foreach (var field in fields)
            {
                var fieldName = documentsController.FieldsController.GetFieldName(field);
                fieldsOnView[fieldName] = field;
            }
            //_repository.CreateRequestWithFields(servId,fields);
            //закрываем шаблон
            template.Close();
            
            return View(new UserRegModel()
            {
                Serv_id = servId.ToString(),
                SubdivisionServiceId = subdivisionServiceId.ToString(),
                Serv_name = name,
                Fields = fieldsOnView
            });
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Authorize(Roles = "Student")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        [HttpPost]
        [Authorize(Roles = "Student")]
        public IActionResult RegServiceToHome(UserRegModel model)
        {
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
            var documentsController = new DocumentsController(_repository); 
            var linkToDocument = _repository.GetLinkToDocumentByServiceId(Int32.Parse(model.Serv_id));

            var template = documentsController.OpenDocumentAsTemplateByName(linkToDocument);
            var fieldNames = template.GetFieldNames();
            var valueFields = documentsController.FieldsController.GetValueFields(fieldNames, userId);
            var fields = valueFields.Select(field => new FieldsModel() { Name = field.Key, Value = field.Value, Malually_fiiled = false}).ToList();
            template.Close();
            
            //var results = model.Fields.Select(t => t.Value).ToList();
            var subdivisionServId = Int32.Parse(model.SubdivisionServiceId);
            _repository.CreateRequestWithFields(subdivisionServId, fields, userId);
            return RedirectToAction("Home");
        }

        public IActionResult DownloadForm(string name,string formLink)
        {
            var documentsController = new DocumentsController(_repository);
            var documentsSettings = documentsController.Settings;
            var fullPath = Path.Combine(documentsSettings.RootPath, documentsSettings.FormsInput, $"{formLink}.docx");
            string filePath = fullPath;
            string fileType = "application/docx";
            string fileName = $"Бланк {name}.docx";
            return PhysicalFile(filePath, fileType,fileName);
        }
    }
}