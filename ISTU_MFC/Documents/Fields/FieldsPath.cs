using System.Collections.Generic;

namespace Documents.Fields
{
    public class FieldsPath
    {
        public Dictionary<string, string> Path;
        public Dictionary<string, string> Names;

        public FieldsPath()
        {
            Path = new Dictionary<string, string>()
            {
                { "NameStudentField", "Database/StudentData/Name" },
                { "SurnameStudentField", "Database/StudentData/Family" },
                { "PatronymicStudentField", "Database/StudentData/SecondName" },
                { "GroupStudentField", "Database/StudentData/Group" },
                { "StudIdStudentField", "Database/StudentData/StudId" },
                { "DepartamentStudent", "Database/StudentData/Department" },
                { "NPSurnameDean", "Database/RequestInfo/NPSurnameDean" },
                { "INN", "User/INN" },
                { "Date", "System/Date" },
            };
            Names = new Dictionary<string, string>()
            {
                { "NameStudentField", "Имя студента" },
                { "SurnameStudentField", "Фамилия студента" },
                { "PatronymicStudentField", "Отчество студентов" },
                { "GroupStudentField", "Группа студента" },
                { "StudIdStudentField", "Номер студенческого " },
                { "DepartamentStudent", "Институт, где учится студент" },
                { "NPSurnameDean", "Директор института" },
                { "INN", "Инн" },
                { "Date", "Дата" },
            };
        }

        public string GetPath(string nameField)
        {
            return Path[nameField];
        }
        public string GetName(string nameField)
        {
            return Names[nameField];
        }
    }
}