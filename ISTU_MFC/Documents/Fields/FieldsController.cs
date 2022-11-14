using System.Collections.Generic;
using System.Linq;
using Repository;

namespace Documents.Fields
{
    public class FieldsController
    {
        private readonly IRepository _repository;
        private readonly SystemFieldsController _systemFieldsController;
        private readonly FieldsPath _path;

        public FieldsController(IRepository repository)
        {
            _repository = repository;
            _path = new FieldsPath();
            _systemFieldsController = new SystemFieldsController();
        }

        public string GetPathToField(string nameField)
        {
            return _path.GetValue(nameField);
        }

        public IDictionary<string,string> GetValueFields(IEnumerable<string> fields)
        {
            IDictionary<string, string> result = new Dictionary<string, string>(); // fieldName by Value
            IDictionary<string, string[]> databaseDictionary = new Dictionary<string, string[]>(); // fieldName by Path
            
            foreach (var fieldName in fields)
            {
                var pathByNameField = GetPathByNameField(fieldName);
                switch (pathByNameField[0])
                {
                    case "Database":
                        databaseDictionary[fieldName] =  GenerateLocalPath(pathByNameField);
                        break;
                    case "System":
                        var valueFields = _systemFieldsController.GetValueFields(fieldName);
                        result[fieldName] = valueFields;
                        break;
                }

                if (databaseDictionary.Count() > 0)
                {
                    var valueFields = _repository.GetValueFields(databaseDictionary);
                    foreach (var field in valueFields)
                    {
                        result[field.Key] = field.Value;
                    }
                }
            }

            return result;
        }

        private string[] SplitPath(string path)
        {
            return path.Split('/');
        }

        private string[] GetPathByNameField(string name)
        {
            return SplitPath(GetPathToField(name));
        }

        private string[] GenerateLocalPath(string[] path) // TODO: Костыль. Лучше будет переделать позже
        {
            var list = path.ToList();
            list.Remove(path[0]);
            return list.ToArray();
        } 
    }
}