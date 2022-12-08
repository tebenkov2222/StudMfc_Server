using System.Collections.Generic;
using ModelsData;

namespace ModelsData
{
    public class StudentHomeModel
    {
        public List<SubdivisionModel> Subdevisons { get; set; }
        public List<RequestModel> Requests { get; set; }
    }
}