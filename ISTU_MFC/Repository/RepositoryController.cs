using System;
using System.Collections.Generic;

namespace Repository
{
    public class RepositoryController: IRepository
    {
        public RepositoryController()
        {
        }

        public void GetDataByFieldsName(IEnumerable<string> fieldsName)
        {
            
        }

        public void WriteMessage(string message)
        {
            Console.WriteLine($"Test message {message}");
        }
    }
}