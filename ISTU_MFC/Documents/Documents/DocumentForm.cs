using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using FormFieldData = Documents.Fields.FormFieldData;

namespace Documents.Documents
{
    public class DocumentForm: DocumentBase
    {
        private const char _pattern = '_';
        private Text _lastText = new Text();

        public DocumentForm(string patchToFile, bool isEditable = false) : base(patchToFile, isEditable)
        {
            Open();
        }

        private List<(int startIndex, int endIndex)> CheckText(Text text)
        {
            var list = new List<(int startIndex, int endIndex)>();
            var textValue = text.Text;
            bool isPatternStarted = false;
            int startIndex = 0;
            for (var i = 0; i < textValue.Length; i++)
            {
                char c = textValue[i];
                    
                if (c == _pattern)
                {
                    if (!isPatternStarted)
                    {
                        startIndex = i;
                    }
                    isPatternStarted = true;
                }
                else
                {
                    if (isPatternStarted)
                    {
                        isPatternStarted = false;
                        list.Add((startIndex, i - 1));
                    }
                }
                if (i == textValue.Length - 1)
                {
                    if (isPatternStarted)
                    {
                        list.Add((startIndex, i));
                    }
                }
                    
            }
            return list;
        }

        private int _countEmpty = 0;
        private List<FormFieldData> CheckFormFields(Text text)
        {
            List<FormFieldData> result = new();
            var foundedFields = CheckText(text);
            var textValue = text.Text;
            var maxIndex = textValue.Length - 1;
            if (foundedFields.Count == 0) return result;
            else if (foundedFields.Count == 1)
            {
                var foundedField = foundedFields[0];
                var startIndex = foundedField.startIndex;
                var endIndex = foundedField.endIndex;
                FormFieldData fieldData;
                if (startIndex == 0 && endIndex == maxIndex)
                {
                    fieldData = new FormFieldData(text,_lastText.Text, startIndex, endIndex);
                }
                else if (startIndex == 0)
                {
                    fieldData = new FormFieldData(text,textValue.Substring(endIndex+1), startIndex, endIndex);
                }
                else
                {
                    fieldData = new FormFieldData(text,textValue.Substring(0,startIndex), startIndex, endIndex);
                }
                if (fieldData.Name == "" || fieldData.Name == " ")
                {
                    _countEmpty++;
                    fieldData.Name = $"Empty_{_countEmpty}";
                }
                result.Add(fieldData);

            }
            else
            {
                int curStartIndex = 0;
                foreach (var field in foundedFields)
                {
                    var fieldData = new FormFieldData(text,textValue.Substring(curStartIndex,field.startIndex - curStartIndex), field.startIndex, field.endIndex);
                    if (fieldData.Name == "" || fieldData.Name == " ")
                    {
                        _countEmpty++;
                        fieldData.Name = $"Empty_{_countEmpty}";
                    }
                    result.Add(fieldData);
                    curStartIndex = field.endIndex+1;

                }
            }
            return result;
        }
        public IEnumerable<FormFieldData> GetAllFormFields()
        {
            List<FormFieldData> result = new();
            var allText = GetAllText();
            int startIndex = 0, endIndex = 0;
            foreach (var text in allText)
            {
                //Console.WriteLine($"Text = [{text.Text}]");
                var checkFormFields = CheckFormFields(text);
                foreach (var field in checkFormFields)
                {
                    result.Add(field);
                }
                _lastText = text;
            }

            return result;
        }

        public void Test()
        {
            Console.WriteLine("Test");
        }
    }
}