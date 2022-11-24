using System.Collections.Generic;
using ISTU_MFC.ModelsData;

namespace ISTU_MFC.Models
{
    public class UserRegModel
    {
        public string Serv_id { get; set; }
        public string Serv_name { get; set; }
        public Dictionary<string,FieldsModel> Fields { get; set; }
    }
}