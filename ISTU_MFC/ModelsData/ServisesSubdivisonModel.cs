using System.Collections.Generic;

namespace ModelsData
{
    public class ServisesSubdivisonModel
    {
        public List<ServiceModel> Awalible { get; set; }
        public List<ServiceModel> ForAdd { get; set; }
        public string SubdivisionId { get; set; }
        
    }
}