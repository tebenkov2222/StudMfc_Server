using System.Collections.Generic;
using ModelsData;

namespace ISTU_MFC.Models
{
    public class RequestGeneratorModel
    {
        public string Req_id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string StudId { get; set; }
        public List<FieldsModel> Fields { get; set; }
        
    }
}