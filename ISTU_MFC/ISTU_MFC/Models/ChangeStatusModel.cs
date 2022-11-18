using System.ComponentModel.DataAnnotations;

namespace ISTU_MFC.ViewModels
{
    public class ChangeStatusModel
    {
        public string message { get; set; }
        public string status { get; set; }
        public string request_id { get; set; }
    }
}