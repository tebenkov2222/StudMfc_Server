using System;
using System.Globalization;

namespace ISTU_MFC.ModelsData
{
    public class InformationAboutRequestModel
    {
        // request_id   student_family  student_name    student_secondName  employee_family  employee_name
        // employee_secondName name_service subdivision_name create_date execution_date closing_date status
        public int RequestId { get; set; } // request_id 
        public int StudentUserId{ get; set; }//student_user_id
        public int StudentId{ get; set; }//stud_id
        public string StudentFamily{ get; set; }//student_family
        public string StudentName{ get; set; }//student_name
        public string StudentSecondName{ get; set; }//student_secondname
        public string GroupNumber{ get; set; }//group_number
        public int EmployeeUserId{ get; set; }//employee_user_id
        public string EmployeeFamily{ get; set; }//employee_family
        public string EmployeeName{ get; set; }//employee_name
        public string EmployeeSecondName{ get; set; }//employee_secondname
        public string NameService{ get; set; }//name_service
        public string SubdivisionName{ get; set; }//subdivision_name
        public string InstituteName{ get; set; }//institute_name
        public string DirectorFamily{ get; set; }//director_family
        public string DirectorName{ get; set; }//director_name
        public string DirectorSecondName{ get; set; }//director_secondname
        public string ChiefFamily{ get; set; }//chief_family
        public string ChiefName{ get; set; }//chief_name
        public string ChiefSecondName{ get; set; }//chief_secondname

        public string CreateDate//create_date
        {
            get
            {
                return CreateDateDAte.ToString("dd.MM.yyyy HH:mm:ss");
            }
            set
            {
                SetDate(value);
            }
        }

        public DateTime CreateDateDAte;

        public void SetDate(string input)
        {
            var dateTime = DateTime.ParseExact(input,
                "dd.MM.yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);
            CreateDateDAte = dateTime;
            
        }

        public string GetValueByName(string nameField)
        {
            return nameField switch
            {
                "NPSurnameDean" => $"{char.ToUpper(DirectorName[0])}. {char.ToUpper(DirectorSecondName[0])}. {DirectorFamily}",
                _ => "null"
            };
        }
    }
}