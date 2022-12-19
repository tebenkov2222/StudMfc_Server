using System.Collections.Generic;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ISTU_MFC.ViewModels
{
    public class ServiceConstructorViewModel
    {
        public string State { get; set; }
        public string TemplateName { get; set; }
        public string FormName { get; set; }
        public string NameService { get; set; }
        public string DescriptionService { get; set; }
        public string PathToPreviewDoc { get; set; }
        public string PathToOutputDoc { get; set; }
        public string PathToFormDoc { get; set; }
        public bool IsHasDoc { get; set; }
        public bool IsNotHasDoc => !IsHasDoc;
        public List<FormFieldViewModel> FormFields { get; set; }

        public static List<SelectListItem> DefaultSelectList()
        {
            var groupStudent = new SelectListGroup() {Name = "Студент"};
            var groupSystem = new SelectListGroup() {Name = "Система"};
            return new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "Пустое поле",
                    Value = "FieldDefault"
                },
                new SelectListItem()
                {
                    Text = "Спрятать поле",
                    Value = "DeleteField"
                },
                new SelectListItem()
                {
                    Text = "Имя ",
                    Value = "NameStudentField",
                    Group = groupStudent
                },
                new SelectListItem()
                {
                    Text = "Фамилия",
                    Value = "SurnameStudentField",
                    Group = groupStudent
                },
                new SelectListItem()
                {
                    Text = "Отчество",
                    Value = "PatronymicStudentField",
                    Group = groupStudent
                },
                new SelectListItem()
                {
                    Text = "Группа студента",
                    Value = "GroupStudentField",
                    Group = groupStudent
                },
                new SelectListItem()
                {
                    Text = "Номер студенческого билета",
                    Value = "StudIdStudentField",
                    Group = groupStudent
                },
                new SelectListItem()
                {
                    Text = "Институт обучения",
                    Value = "DepartamentStudent",
                    Group = groupStudent
                },
                new SelectListItem()
                {
                    Text = "Директор института обучения",
                    Value = "NPSurnameDean",
                    Group = groupStudent
                },
                new SelectListItem()
                {
                    Text = "Дата: дд:мм:гггг",
                    Value = "Date",
                    Group = groupSystem,
                },
            };
        }

        public List<string> RequiredDocs { get; set; }
    }
}