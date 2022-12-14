using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ISTU_MFC.ViewModels
{
    public class DocGenerationViewModel
    {
        public string PathToPreviewDoc { get; set; }
        public string PathToOutputDoc { get; set; }
        public string PathToFormDoc { get; set; }
        public bool IsHasDoc { get; set; }
        public bool IsNotHasDoc => !IsHasDoc;
        public List<FormFieldViewModel> FormFields { get; set; }

        public static List<SelectListItem> DefaultSelectList()
        {
            var groupStudent = new SelectListGroup() {Name = "Студент"};
            return new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "Пустое поле",
                    Value = "FieldDefault"
                },
                new SelectListItem()
                {
                    Text = "Имя студента",
                    Value = "NameStudentField",
                    Group = groupStudent
                },
                new SelectListItem()
                {
                    Text = "Фамилия студента",
                    Value = "SurnameStudentField",
                    Group = groupStudent
                },
                new SelectListItem()
                {
                    Text = "Группа студента",
                    Value = "GroupStudentField",
                    Group = groupStudent,
                },
            };
        }
    }
}