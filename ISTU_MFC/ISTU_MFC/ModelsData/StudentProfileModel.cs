namespace ISTU_MFC.ModelsData
{
    public class StudentProfileModel: UserModel
    {
        public string Group{ get; set; }
        public string StudId{ get; set; }
        public string Department{ get; set; } // 7
        public string Faculty{ get; set; } // 6

        public string GetValueByName(string nameField)
        {
            return nameField switch
            {
                "Group" => Group,
                "Name" => Name,
                "Family" => Family,
                "SecondName" => SecondName,
                "StudId" => StudId,
                "Department" => Department,
                "Faculty" => Faculty,
                _ => "null"
            };
        }
    }
}