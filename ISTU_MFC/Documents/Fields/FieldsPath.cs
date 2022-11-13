using System.Collections.Generic;

namespace Documents.Fields
{
    public class FieldsPath
    {
        public Dictionary<string, string> Path;

        public FieldsPath()
        {
            Path = new Dictionary<string, string>()
            {
                { "NameStudentField", "Database/StudentData/Name" },
                { "SurnameStudentField", "Database/StudentData/Family" },
                { "PatronymicStudentField", "Database/StudentData/SecondName" },
                { "GroupStudentField", "Database/StudentData/Group" },
                { "INN", "Database/Fields/INN" },
                { "Date", "System/Date" },
            };
        }

        public string GetValue(string nameField)
        {
            return Path[nameField];
        }
    }
}