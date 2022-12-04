using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Documents;
using Documents.Documents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ISTU_MFC.Models;
using ISTU_MFC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using ModelsData;
using Repository;
using Spire.Pdf.Exporting.XPS.Schema;
using Path = System.IO.Path;

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
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
            var requests = _repository.GetRequests(userId);
            return View(requests);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public IActionResult WorkWithDoc(EmployeeWorkWithDocPost model)
        {
            //с одной страницы может быть несколько постзапросов, для этого нужно делать переадресацию по его типу
            if(model.Type == "ChooseRequest")
                return RedirectToAction("RequestGenerator", new { req_id = model.Id });
            else if(model.Type == "ServiceConstructor")
                return RedirectToAction("ServiceConstructor", "Employees");
            else
            {
                if (model.Status != null)
                {
                    var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
                    var requests = _repository.GetFilteredRequests(userId, model.Status);
                    return View(requests);
                }
                else if (model.Number != null)
                {
                    var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
                    var requests = _repository.GetNumberedRequests(userId, Int32.Parse(model.Number));
                    return View(requests);
                }
                else if (model.Family != null)
                {
                    var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
                    var requests = _repository.GetNamedRequests(userId, model.Family);
                    return View(requests);
                }
                else 
                    return RedirectToAction("WorkWithDoc", "Employees");
            }
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

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public IActionResult RequestGenerator(string req_id)
        {
            var user = _repository.GetStudentByRequest(Int32.Parse(req_id));
            var documentsController = new DocumentsController(_repository);
            var model = new RequestGeneratorModel()
            {
                Req_id = req_id,
                Name = $"{user.Family} {user.Name} {user.SecondName}",
                Group = user.Group,
                StudId = user.StudId,
                Fields = documentsController.FieldsController.GetFieldsOnViewByNames(
                    _repository.GetRequestFields(Int32.Parse(req_id)))
            };
            
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
            _repository.ChangeRequestStateByFirst(Int32.Parse(req_id), userId);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public IActionResult RequestGenerator(EmployeeRequestGeneratorPost model)
        {
            //с одной страницы может быть несколько постзапросов, для этого нужно делать переадресацию по его типу
            if (model.Type == "DownloadGeneration")
                return RedirectToAction("DownloadGeneration", new { req_id = model.Req_Id });
            else
                return RedirectToAction("ChangeStatus", new { req_id = model.Req_Id });
            
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public IActionResult DownloadGeneration(string req_id)
        {
            var request_id = Int32.Parse(req_id);
// вот работаем с документами
            var documentsController = new DocumentsController(_repository);
            var linkToDocument = _repository.GetLinkToDocumentByRequestId(request_id);
            var copyToTempAndOpenDocument = documentsController.
                CopyToTempAndOpenDocument(linkToDocument, linkToDocument + $"_temp{DateTime.Now.ToString("ddMMyy_hhmmss")}", true);
            var valueFields = _repository.GetValueFieldsByIdRequest(request_id);
            copyToTempAndOpenDocument.SetFieldValues(valueFields);

            var requestModel = _repository.GetInformationAboutRequestByRequest(request_id);
            var docName =
                $"{requestModel.StudentFamily}{char.ToUpper(requestModel.StudentName[0])}{char.ToUpper(requestModel.StudentSecondName[0])}_{request_id}_{DateTime.Now.ToString("ddMMyy_hhmmss")}";
            var pathToDownloadDocument = documentsController.GetPathByName(documentsController.Settings.OutputPath, docName);

            //copyToTempAndOpenDocument.SaveAs(pathToDownloadDocument);
            copyToTempAndOpenDocument.Save();
            copyToTempAndOpenDocument.Close();
            System.IO.File.Copy(copyToTempAndOpenDocument.PatchToFile, pathToDownloadDocument, true);
            //copyToTempAndOpenDocument.Document.Dispose();
            
            /*var generateAndSaveImage = documentsController.DocumentViewer.GenerateAndSaveImage(
                copyToTempAndOpenDocument,
                documentsController.GetPathByName ( Path.Combine("wwwroot", "images"), copyToTempAndOpenDocument.Name, "jpg"));
            */
            //var relativePath = $"~/images/{copyToTempAndOpenDocument.Name}.jpg";
            var combine = Path.Combine("~", "images",copyToTempAndOpenDocument.Name);
            var relativePath = $"{combine}.jpg";
            var viewModel = new DownloadGenerationViewModel();
            viewModel.PathToDownloadDocument = pathToDownloadDocument;
            viewModel.PathToPreviewImage = relativePath;
            viewModel.RequestId = request_id;
            copyToTempAndOpenDocument.Dispose();
            copyToTempAndOpenDocument.Document.Dispose();
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public IActionResult DownloadGeneration(EmployeeDownloadGenerationPost model)
        {
            return RedirectToAction("Download", new {documentPath=model.DocumentPath});
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public IActionResult ChangeStatus(string req_id)
        {
            return View(new ChangeStatusModel()
            {
                request_id = req_id
            });
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
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
            _repository.ChangeRequestStatus(Int32.Parse(model.request_id), userId , model.status);
            if (model.message == "")
                model.message = $"Статус заявки изменен на \"{statuses[model.status]}\"";
            _repository.CreateMessage(Int32.Parse(model.request_id), userId, model.message);
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
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
            var model = _repository.GetTableMessages(userId);
            return View(model);
        }
        
        public IActionResult Download(string documentPath)
        {
            //documentPath.Replace('\\', '/');
            string filePath = documentPath;
            string fileType = "application/docx";
            var name = Path.GetFileName(documentPath);
            string fileName = name;

            return PhysicalFile(filePath, fileType,fileName);
        }

        public IActionResult ServiceList()
        {
            return View();
        }
    }
}
