﻿using System;
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
using ISTU_MFC.ModelsData;
using ISTU_MFC.Repository;


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
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
            var model = _repository.GetDevisionsList(userId);
            return View(model);
        }

        [Authorize(Roles = "Student")]
        public IActionResult Notifications()
        {
            var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
            var model = _repository.GetTableMessages(userId);
            return View(model);
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

        [Authorize(Roles = "Student")]
        public IActionResult Servise(int servId)
        {
            var model = _repository.GetServisesInfo(servId);
            model.Id = servId;
            return View(model);
        }

        [Authorize(Roles = "Student")]
        public IActionResult RegService(int servId, string name)
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
        public IActionResult RegService(UserRegModel model)
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
            _repository.CreateRequestWithFields(Int32.Parse(model.Serv_id), fields, userId);
            return RedirectToAction("Home");
        }
    }
}