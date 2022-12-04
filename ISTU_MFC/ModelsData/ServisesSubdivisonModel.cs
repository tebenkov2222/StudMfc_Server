using System.Collections.Generic;

namespace ModelsData
{
    public class ServisesSubdivisonModel
    {
        public List<ServiseModel> Awalible { get; set; }
        public List<ServiseModel> ForAdd { get; set; }
        public string SubdivisionId { get; set; }
        
    }
}