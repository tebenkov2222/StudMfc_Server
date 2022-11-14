namespace ModelsData
{
    public class DirectorInstituteModel: UserModel
    {
        public string GetValueByName(string nameField)
        {
            return nameField switch
            {
                "NPSurnameDean" => $"{char.ToUpper(Name[0])}. {char.ToUpper(SecondName[0])}. {Family}",
                _ => "null"
            };
        }
    }
}