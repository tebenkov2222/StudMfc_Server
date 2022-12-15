using DocumentFormat.OpenXml.Wordprocessing;

namespace Documents.Fields
{
    public class FormFieldData
    {
        public Text Text;
        public string Name;
        public int StartIndex;
        public int EndIndex;
        private string _defaultValue;
        private bool _isChanged;
        public string Value
        {
            get => Text.Text.Substring(StartIndex, EndIndex - StartIndex);
            private set
            {
                var start = Text.Text.Substring(0, StartIndex);
                var end = Text.Text.Substring(EndIndex+1);
                EndIndex = StartIndex + value.Length-1;
                Text.Text = start + value + end;
            }
        }

        public FormFieldData(Text text, string name, int startIndex, int endIndex)
        {
            Name = name;
            StartIndex = startIndex;
            EndIndex = endIndex;
            Text = text;
            _defaultValue = Value;
        }

        public void SetValue(string value)
        {
            _isChanged = true;
            Value = value;
        }

        public void SetValueByFieldName(string fieldName)
        {
            switch (fieldName)
            {
                case "FieldDefault":
                    ResetValue();
                    break;
                case "NameStudentField":
                    SetValue(GetFieldValueByName("NameStudentField"));
                    break;
                case "SurnameStudentField":
                    SetValue(GetFieldValueByName("SurnameStudentField"));
                    break;
                case "GroupStudentField":
                    SetValue(GetFieldValueByName("GroupStudentField"));
                    break;
            }
        }

        private string GetFieldValueByName(string nameField)
        {
            string patternStart = " <$";
            string patternEnd = "$>";
            return $"{patternStart}{nameField}{patternEnd}";
        }

        public void ResetValue()
        {
            /*if(_isChanged)
                SetValue(_defaultValue);*/
        }
    }
}