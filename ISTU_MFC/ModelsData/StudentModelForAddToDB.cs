using System;
using System.Collections.Generic;

namespace ModelsData
{
    public class StudentModelForAddToDB
    {
        public int id { get; set; }
        public Fio fio_full { get; set; }
        public Student[] student { get; set; }
    }

    public class Fio
    {
        public string family { get; set; }
        public string name { get; set; }
        public string patronymic { get; set; }
    }

    public class Student
    {
        public int id { get; set; }
        public string group { get; set; }
    }
}