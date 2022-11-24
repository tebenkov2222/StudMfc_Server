using System.Collections.Generic;
using System.Linq;
using ISTU_MFC.ModelsData;
using ISTU_MFC.Repository;

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
            return _path.GetPath(nameField);
        }

        public IDictionary<string,string> GetValueFields(IEnumerable<string> fields, int studentId)
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
                    var valueFields = _repository.GetValueFieldsByPath(databaseDictionary, studentId);
                    foreach (var field in valueFields)
                    {
                        result[field.Key] = field.Value;
                    }
                }
            }

            return result;
        }

        public List<FieldsModel> GetFieldsOnViewByNames(IList<FieldsModel> fieldsModels)
        {
            var result = new List<FieldsModel>();
            foreach (var field in fieldsModels)
            {
                var fieldsModel = new FieldsModel();
                fieldsModel.Name = _path.GetNameView(field.Name);
                fieldsModel.Value = field.Value;
                fieldsModel.Malually_fiiled = field.Malually_fiiled;
                result.Add(fieldsModel);
            }

            return result;
        }
        public List<FieldsModel> GetFieldsNameByView(IList<FieldsModel> fieldsModels)
        {
            var result = new List<FieldsModel>();
            foreach (var field in fieldsModels)
            {
                var fieldsModel = new FieldsModel();
                fieldsModel.Name = _path.GetNameField(field.Name);
                fieldsModel.Value = field.Value;
                fieldsModel.Malually_fiiled = field.Malually_fiiled;
                result.Add(fieldsModel);
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

        public string GetFieldName(FieldsModel field)
        {
            return _path.GetNameView(field.Name);
        }
    }
}