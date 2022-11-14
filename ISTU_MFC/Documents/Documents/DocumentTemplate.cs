using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Documents.Fields;

namespace Documents.Documents
{
    public class DocumentTemplate: DocumentBase
    {
        
        private const string _patternStart = "<$";
        private const string _patternEnd = "\\>";
        private IEnumerable<TemplateField> _foundFields;

        public IEnumerable<TemplateField> FoundFields => _foundFields;

        public DocumentTemplate(string patchToFile, bool isEditable = false) : base(patchToFile, isEditable)
        {
            Open();
            _foundFields = FindAllFields();
        }

        public IEnumerable<string> GetFieldNames()
        {
            return FindAllFields().Select(t => t.Name);
        }

        
        public void SetFieldValues(IDictionary<string, string> nameValuesDictionary)
        {
            foreach (var field in _foundFields)
            {
                if (nameValuesDictionary.TryGetValue(field.Name, out string value))
                {
                    field.SetValue(value);
                }
                else
                {
                    throw new WarningException($"Field {field.Name} does not exist in nameValuesDictionary");
                }
            }
        }

        private IEnumerable<TemplateField> FindAllFields()
        {
           return FindTextByFirstAndLatestEntry(_patternStart, _patternEnd).Select(t => new TemplateField(t));
        }
    }
}