using System;

namespace ModelsData
{
    public class MessageModel
    {
        public string Text { get; set; }
        public string RequestId { get; set; }
        public string Status { get; set; }

        public string Date
        {
            get => this.DateTime.ToString("dd.MM.yy HH:mm:ss");
            set
            {
                DateTime = DateTime.Parse(value);
            }
        }

        public DateTime DateTime;
    }
}