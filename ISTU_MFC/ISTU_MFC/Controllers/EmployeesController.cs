using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using DocumentFormat.OpenXml.Wordprocessing;
using Documents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ISTU_MFC.Models;
using ISTU_MFC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repository;
using Path = System.IO.Path;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Packaging;
using ModelsData;
using FormFieldData = Documents.Fields.FormFieldData;


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
            if (model.Type == "ChooseRequest")
                return RedirectToAction("RequestGenerator", new { req_id = model.Id });
            if (model.Type == "ServiceConstructor")
                return RedirectToAction("ServiceConstructor", "Employees");
            if (model.Status != null)
            {
                var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
                var requests = _repository.GetFilteredRequests(userId, model.Status);
                return View(requests);
            }

            if (model.Number != null)
            {
                var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
                var requests = _repository.GetNumberedRequests(userId, Int32.Parse(model.Number));
                return View(requests);
            }

            if (model.Family != null)
            {
                var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
                var requests = _repository.GetNamedRequests(userId, model.Family);
                return View(requests);
            }

            return RedirectToAction("WorkWithDoc", "Employees");
        }

        [Authorize(Roles = "Employee")]
        public IActionResult DocGenerator()
        {
            var docGenerationViewModel = new ServiceConstructorViewModel();
            docGenerationViewModel.PathToFormDoc = "";
            docGenerationViewModel.RequiredDocs = new List<string>() { "Паспорт", "СНИЛС", "ИНН" };
            return View(docGenerationViewModel);
        }

        [Authorize(Roles = "Employee")]
        public IActionResult ServiceConstructor()
        {
            var serviceConstructorModelView = new ServiceConstructorViewModel();
            serviceConstructorModelView.State = "Info";
            return View(serviceConstructorModelView);
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
            var copyToTempAndOpenDocument = documentsController.CopyToTempAndOpenDocument(linkToDocument,
                linkToDocument + $"_temp{DateTime.Now.ToString("ddMMyy_hhmmss")}", true);
            var valueFields = _repository.GetValueFieldsByIdRequest(request_id);
            copyToTempAndOpenDocument.SetFieldValues(valueFields);

            var requestModel = _repository.GetInformationAboutRequestByRequest(request_id);
            var docName =
                $"{requestModel.StudentFamily}{char.ToUpper(requestModel.StudentName[0])}{char.ToUpper(requestModel.StudentSecondName[0])}_{request_id}_{DateTime.Now.ToString("ddMMyy_hhmmss")}";
            var docViewName =
                $"DocView_{request_id}_{DateTime.Now.ToString("ddMMyy_hhmmss")}";
            var pathToDownloadDocument = documentsController.GetPathByName(documentsController.Settings.OutputPath, docName);
            var pathToViewDocument = Path.Combine(documentsController.Settings.RootPath,documentsController.Settings.TempPath, $"{docViewName}.docx"); 

            //copyToTempAndOpenDocument.SaveAs(pathToDownloadDocument);
            copyToTempAndOpenDocument.Save();
            copyToTempAndOpenDocument.Close();
            var patchToFile = copyToTempAndOpenDocument.PatchToFile;
            System.IO.File.Copy(patchToFile, pathToDownloadDocument, true);
            System.IO.File.Copy(patchToFile, pathToViewDocument, true);
            
            //documentsController.DocumentViewer.GenerateAndSavePdf(patchToFile, pathToViewDocument);
            var viewModel = new DownloadGenerationViewModel();
            viewModel.PathToDownloadDocument = pathToDownloadDocument;
            //var replace = pathToViewDocument.Replace("/", "\\").Replace("\\", "$").Split("$");
            //var wwwrootPathView = $"~/temp/{Path.GetFileName(pathToViewDocument)}";
            viewModel.PathToPreviewDoc = Path.GetFileName(pathToViewDocument);
            viewModel.RequestId = request_id;
            copyToTempAndOpenDocument.Dispose();
            copyToTempAndOpenDocument.Document.Dispose();
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public IActionResult DownloadGeneration(EmployeeDownloadGenerationPost model)
        {
            return RedirectToAction("Download", new { documentPath = model.DocumentPath });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public ActionResult GetWordDocument(string path)
        {
            var documentsController = new DocumentsController(_repository);
            var pathToViewDocument = Path.Combine(documentsController.Settings.RootPath,documentsController.Settings.TempPath, path);

            byte[] bytes;
            using (FileStream fstream = new FileStream(pathToViewDocument, FileMode.Open))
            {
                byte[] array = new byte[fstream.Length];
                fstream.Read(array, 0, array.Length);
                bytes = array;
            }
            return Json(bytes);
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
        
        [Authorize(Roles = "Employee")]
        public IActionResult Download(string documentPath)
        {
            //documentPath.Replace('\\', '/');
            string filePath = documentPath;
            string fileType = "application/docx";
            var name = Path.GetFileName(documentPath);
            string fileName = name;

            return PhysicalFile(filePath, fileType,fileName);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public IActionResult ServiceList()
        {
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
            var model = _repository.GetSubdivisionServices(userId);
            model.Awalible.Reverse();
            model.ForAdd.Reverse();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public IActionResult ServiceList(ServiseListPostModel Model)
        {
            switch (Model.Type)
            {
                case "Add":
                    _repository.InsertSubdivisionsService(Int32.Parse(Model.Id),
                        Int32.Parse(Model.SubdivisonId));
                    break;
                case "Delete":
                    _repository.DeleteSubdivisionsService(Int32.Parse(Model.Id),
                        Int32.Parse(Model.SubdivisonId));
                    break;
                default:
                    _repository.ChangeSubdivisionsServiceStatus(Int32.Parse(Model.Id), 
                        Int32.Parse(Model.SubdivisonId), Model.Type);
                    break;
            }
            return RedirectToAction("ServiceList");
        }
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public IActionResult ServiceConstructorOnAddFile(IFormFile uploadedFile, ServiceConstructorViewModel viewModel)
        {
            var documentsController = new DocumentsController(_repository);
            var documentsSettings = new DocumentsController(_repository).Settings;
            var fileNameWithoutExtension = TranslitController.ToEn(Path.GetFileNameWithoutExtension(uploadedFile.FileName));
            var fileExtenstion = Path.GetExtension(uploadedFile.FileName);
            string filePath = Path.Combine(documentsSettings.RootPath,documentsSettings.FormsTemp, $"{fileNameWithoutExtension}_{DateTime.Now.ToString("ddMMyy_hhmmss")}{fileExtenstion}");

            if (uploadedFile.Length > 0) {
                using (Stream fileStream = new FileStream(filePath, FileMode.Create)) {
                    uploadedFile.CopyTo(fileStream);
                }
            }
            viewModel.PathToFormDoc = filePath;
            var document = WordprocessingDocument.Open(filePath, false);
            var pathToViewDocument = Path.Combine(documentsController.Settings.RootPath,documentsController.Settings.TempPath, Path.GetFileName(filePath));
            var pathToOutputDocument = Path.Combine(documentsController.Settings.RootPath,documentsController.Settings.FormsTemp,  $"{fileNameWithoutExtension}_Output_{DateTime.Now.ToString("ddMMyy_hhmmss")}{fileExtenstion}");

            System.IO.File.Copy(filePath, pathToViewDocument);
            System.IO.File.Copy(filePath, pathToOutputDocument);
            viewModel.PathToPreviewDoc = Path.GetFileName(filePath);
            viewModel.PathToFormDoc = filePath;
            viewModel.PathToOutputDoc = pathToOutputDocument;
            viewModel.IsHasDoc = true;
            viewModel.FormFields = new List<FormFieldViewModel>();
            var form = documentsController.OpenDocumentAsFormByPath(pathToViewDocument, true);
            var formFields = form.GetAllFormFields().ToList();
            for (int i = 0; i < formFields.Count; i++)
            {
                var formField = formFields[i];
                if (formField.Name.StartsWith("Empty_"))
                {
                    formField.SetValue("");
                }
                else
                {
                    viewModel.FormFields.Add(new FormFieldViewModel()
                    {
                        Name = formField.Name,
                        Text = formField.Text.Text,
                        SelectedType = "FieldDefault",
                        SelectList = ServiceConstructorViewModel.DefaultSelectList()
                    }); 
                }
            }
            form.Save();
            form.Close();
            viewModel.State = "ChangeFile";
            return View("ServiceConstructor", viewModel);
        }
        private string GetPathViewDoc(string fileName)
        {
            DocumentsController documentsController = new DocumentsController(_repository);
            return Path.Combine(documentsController.Settings.RootPath,documentsController.Settings.TempPath, Path.GetFileName(fileName));
        }
        [HttpPost]
        public IActionResult ServiceConstructorOnViewDoc(string[] names, string[] fields, string pathToPreviewDoc, string pathToOutputDoc, string pathToFormDoc)
        {
            var model = GenerateDocumentForm(names, fields, pathToPreviewDoc, pathToOutputDoc, pathToFormDoc);
            model.State = "ChangeFile";
            var documentsController = new DocumentsController(_repository);
            var pathToViewDocument = GetPathViewDoc(pathToPreviewDoc);
            var documentTemplate = documentsController.OpenDocumentAsTemplateByPath(pathToViewDocument, true);
            Dictionary<string, string> valueFields = new Dictionary<string, string>()
            {
                {"NameStudentField", "Иван"},
                {"SurnameStudentField", "Иванов"},
                {"PatronymicStudentField", "Иванович"},
                {"GroupStudentField", "Б20-191-1"},
                {"StudIdStudentField", "12345678"},
                {"DepartamentStudent", "Институт «Информатика и вычислительная техника»"},
                {"NPSurnameDean", "И. О. Архипов"},
                {"Date", DateTime.Now.ToString("dd.MM.yy")},
            };
            documentTemplate.SetFieldValues(valueFields);
            documentTemplate.Save();
            documentTemplate.Close();
            return View("ServiceConstructor", model);
        }
        [HttpPost]
        public IActionResult ServiceConstructorOnSaveDoc(string[] names, string[] fields, string pathToPreviewDoc, string pathToOutputDoc, string pathToFormDoc)
        {
            var model = GenerateDocumentForm(names, fields, pathToPreviewDoc, pathToOutputDoc, pathToFormDoc);
            model.State = "FilesInfo";
            return View("ServiceConstructor", model);
        }

        private ServiceConstructorViewModel GenerateDocumentForm(string[] names, string[] fields, string pathToPreviewDoc, string pathToOutputDoc, string pathToFormDoc)
        {
            DocumentsController documentsController = new DocumentsController(_repository);
            var pathToViewDocument = GetPathViewDoc(pathToPreviewDoc);
            System.IO.File.Copy(pathToFormDoc,pathToOutputDoc, true);
            var outputDoc = documentsController.OpenDocumentAsFormByPath(pathToOutputDoc, true);
            var formFieldResult = outputDoc.GetAllFormFields();
            List<FormFieldData> formFieldIEnumerable = new List<FormFieldData>();
            foreach (var field in formFieldResult)
            {
                if (field.Name.StartsWith("Empty_"))
                {
                    field.SetValue("");
                }
                else
                {
                    formFieldIEnumerable.Add(field);
                }
            }
            var formFields = formFieldIEnumerable.ToDictionary(t => t.Name);
            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i];
                if (name == null) name = "";
                var field = fields[i];
                formFields[name].SetValueByFieldName(field);
            }
            
            outputDoc.Save();
            outputDoc.Close();
            //outputDoc.SaveAs(pathToViewDocument);
            System.IO.File.Copy(pathToOutputDoc,pathToViewDocument, true);
            
            var model = new ServiceConstructorViewModel();
            model.FormFields = new List<FormFieldViewModel>();
            var formFieldList = formFieldIEnumerable.ToList();
            for (var i = 0; i < formFieldList.Count; i++)
            {
                var field = formFieldList[i];
                model.FormFields.Add(new FormFieldViewModel()
                {
                    Name = names[i],
                    Text = formFields[names[i]].Text.Text,
                    SelectedType = fields[i]
                });
                model.FormFields[i].SelectList = new List<SelectListItem>();
                foreach (var listItem in  ServiceConstructorViewModel.DefaultSelectList())
                {
                    model.FormFields[i].SelectList.Add(
                        new SelectListItem()
                        {
                            Text = listItem.Text,
                            Group = listItem.Group,
                            Value = listItem.Value,
                            Selected =(bool)(listItem.Value == fields[i])
                        }
                    );
                }
            }

            model.IsHasDoc = true;
            model.PathToFormDoc = pathToFormDoc;
            model.PathToOutputDoc = pathToOutputDoc;
            model.PathToPreviewDoc = pathToPreviewDoc;
            return model;
        }
        [HttpPost]
        public IActionResult ServiceConstructorOnStart()
        {
            var model = new ServiceConstructorViewModel();
            model.State = "SelectFile";
            return View("ServiceConstructor", model);
        }

        [HttpPost]
        public IActionResult GetRequiredDocs(string inpChosenDocs)
        {
            Console.WriteLine(inpChosenDocs);
            return RedirectToAction("WorkWithDoc");
        }

        [HttpPost]
        public IActionResult ServiceConstructorSendService(string templateName, string formName, string nameService, string descriptionService)
        {
            var model = new ServiceConstructorViewModel();
            model.State = "Final";
            _repository.CreateService(nameService, descriptionService, templateName, formName);
            return View("ServiceConstructor", model);
        }
        [HttpPost]
        public IActionResult ServiceConstructorSaveFiles(string nameTemplate, string nameForm, string pathToPreviewDoc, string pathToOutputDoc, string pathToFormDoc)
        {
            var nameTemplateEn = TranslitController.ToEn(nameTemplate);
            var nameFormEn = TranslitController.ToEn(nameForm);
            var documentsController = new DocumentsController(_repository);
            var documentsSettings = documentsController.Settings;
            var date = DateTime.Now.ToString("yyMMdd_hhmm");
            var templateFileName = $"{nameTemplateEn}_{date}";
            var formFileName = $"{nameFormEn}_{date}";
            var formPath = Path.Combine(documentsSettings.RootPath, documentsController.Settings.FormsInput, $"{formFileName}.docx");
            var templatePath = Path.Combine(documentsSettings.RootPath, documentsController.Settings.InputPath, $"{templateFileName}.docx");
            var previewPath = Path.Combine(documentsSettings.RootPath, documentsController.Settings.TempPath, pathToPreviewDoc);
            System.IO.File.Copy(pathToOutputDoc, templatePath, true);
            System.IO.File.Copy(pathToFormDoc, formPath, true);

            var model = new ServiceConstructorViewModel();
            model.State = "ServiceInfo";
            model.TemplateName = templateFileName;
            model.FormName = formFileName;
            
            return View("ServiceConstructor", model);
        }
    }
}
