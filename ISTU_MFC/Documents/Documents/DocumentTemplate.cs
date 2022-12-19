using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Documents.Documents
{
    public class DocumentTemplate: DocumentBase
    {
        
        private const string _patternStart = "<$";
        private const string _patternEnd = "$>";
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
                    //throw new WarningException($"Field {field.Name} does not exist in nameValuesDictionary");
                }
            }
        }

        private IEnumerable<TemplateField> FindAllFields()
        {
            List<TemplateField> result = new List<TemplateField>();
            var fondedTexts = FindTextByFirstAndLatestEntry(_patternStart, _patternEnd).ToList();
            for (int i = 0; i < fondedTexts.Count; i++)
            {
                var findTextData = fondedTexts[i];
                var templateField = new TemplateField(findTextData);
                if (i > 0)
                {
                    var lastField = result.Last();
                    if (lastField.Text == findTextData.Text)
                    {
                        templateField.JointField(lastField);
                    }
                }
                result.Add( templateField);
                
            }

            return result;
            //return fondedTexts.Select(t => new TemplateField(t));
        }
    }
}