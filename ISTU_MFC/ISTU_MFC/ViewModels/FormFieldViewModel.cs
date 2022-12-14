using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ISTU_MFC.ViewModels
{
    public class FormFieldViewModel
    {
        public string Name { get; set; }
        public string SelectedType { get; set; }
        public List<SelectListItem> SelectList { get; set; }

    }
}