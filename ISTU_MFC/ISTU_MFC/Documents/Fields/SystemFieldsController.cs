using System;

namespace Documents.Fields
{
    public class SystemFieldsController
    {
        public string GetValueFields(string nameField)
        {
            switch (nameField)
            {
                case "Date":
                    return DateTime.Now.ToString("dd.MM.yy");
            }

            return "";
        }
    }
}