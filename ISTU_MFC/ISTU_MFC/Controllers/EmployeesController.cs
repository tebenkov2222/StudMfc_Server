using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Documents;
using Documents.Documents;
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
            var documentsController = new DocumentsController(_repository);
            var model =documentsController.FieldsController.GetFieldsOnViewByNames(_repository.GetRequestFeelds(Int32.Parse(req_id)));
            
            _repository.ChangeRequestStateByFirst(Int32.Parse(req_id), _repository.UserId);
            return View(model);
        }

        [Authorize(Roles = "Employee")]
        public IActionResult DownloadGeneration(int request_id)
        {
// вот работаем с документами
            var documentsController = new DocumentsController(_repository);
            var linkToDocument = _repository.GetLinkToDocumentByRequestId(request_id);
            var copyToTempAndOpenDocument = documentsController.
                CopyToTempAndOpenDocument(linkToDocument, linkToDocument + $"_temp{DateTime.Now.ToString("dd-MM-yy_hh-mm-ss")}", true);
            var valueFields = _repository.GetValueFieldsByIdRequest(request_id);
            copyToTempAndOpenDocument.SetFieldValues(valueFields);

            var requestModel = _repository.GetInformationAboutRequestByRequest(request_id);
            var docName =
                $"{requestModel.StudentFamily}{char.ToUpper(requestModel.StudentName[0])}{char.ToUpper(requestModel.StudentSecondName[0])}_{request_id}";
            var pathToDownloadDocument = documentsController.GetPathByName(documentsController.Settings.OutputPath, docName);

            copyToTempAndOpenDocument.SaveAs(pathToDownloadDocument);
            copyToTempAndOpenDocument.Close();
            var generateAndSaveImage = documentsController.DocumentViewer.GenerateAndSaveImage(
                copyToTempAndOpenDocument,
                documentsController.GetPathByName("wwwroot\\images", copyToTempAndOpenDocument.Name, "jpg"));
            var relativePath = $"~/wwwroot/images/{copyToTempAndOpenDocument.Name}.jpg";
            var viewModel = new DownloadGenerationViewModel();
            viewModel.PathToDownloadDocument = pathToDownloadDocument;
            viewModel.PathToPreviewImage = relativePath;
            return View(viewModel);
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
            var statuses = new Dictionary<string, string>()
            {
                { "not processed", "Не обработана" },
                { "processing", "В работе" },
                { "processed", "Обработана" },
                { "closed", "Закрыта" }
            };
            _repository.ChangeRequestState(Int32.Parse(model.request_id), _repository.UserId , model.status);
            if (model.message != "")
                model.message = $"Статус заявки изменен на \"{statuses[model.status]}\"";
            _repository.CreateMessage(Int32.Parse(model.request_id), _repository.UserId, model.message);
            return RedirectToAction("RequestGenerator", new{req_id = model.request_id});
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
        
        public IActionResult Download(string documentPath)
        {
            //documentPath.Replace('\\', '/');
            string filePath = documentPath;
            string fileType = "application/docx";
            string fileName = documentPath.Split('\\').Last();
            return PhysicalFile(filePath, fileType,fileName);
        }
    }
}
