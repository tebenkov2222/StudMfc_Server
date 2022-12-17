using System.Collections.Generic;
using ModelsData;

namespace ISTU_MFC.Models
{
    public class AboutModel
    {
        public string Sub_Id { get; set; }
        public string Info { get; set; }
        public List<ServiceModel> Servises { get; set; }
        
    }
}