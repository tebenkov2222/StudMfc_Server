using System;

namespace ModelsData
{
    public class RequestModel
    {
        public string Caption { get; set; }
        public string Id { get; set; }
        public string FamylyNS { get; set; }

        public string CreationDate
        {
            get
            {
                return CreateDateTime.ToString("dd.MM.yyyy");
            }
            set
            {
                CreateDateTime = DateTime.Parse(value);
            }
        }

        public DateTime CreateDateTime;
        public string State{ get; set; }
    }
}