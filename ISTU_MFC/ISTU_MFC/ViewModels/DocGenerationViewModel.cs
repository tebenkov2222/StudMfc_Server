using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ISTU_MFC.ViewModels
{
    public class DocGenerationViewModel
    {
        public string PathToPreviewDoc { get; set; }
        public string RelativePathToPreviewDoc { get; set; }
        public string PathToFormDoc { get; set; }
        public bool IsHasDoc { get; set; }
        public bool IsNotHasDoc => !IsHasDoc;
        public List<FormFieldViewModel> FormFields { get; set; }
        public SelectList SelectList { get; set; }
    }
}