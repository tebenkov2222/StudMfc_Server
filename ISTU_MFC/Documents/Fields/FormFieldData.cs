using DocumentFormat.OpenXml.Wordprocessing;

namespace Documents.Fields
{
    public class FormFieldData
    {
        public Text Text;
        public FormFieldData JoinedLastFormField;
        public string Name;

        public int StartIndex
        {
            get
            {
                if (_isJoinedText)
                {
                    return _startIndex + JoinedLastFormField.EndIndex;
                }
                else return _startIndex;
            }
            set
            {
                _startIndex = value;
            }
        }

        public int EndIndex
        {
            get
            {
                if (_isJoinedText)
                {
                    return _endIndex + JoinedLastFormField.EndIndex;
                }
                else return _endIndex;
            }
            set
            {
                _endIndex = value;
            }
        }
        private int _startIndex;
        private int _endIndex;
        private string _defaultValue;
        private bool _isChanged;
        private bool _isJoinedText;
        public string Value
        {
            get => Text.Text.Substring(StartIndex, EndIndex - StartIndex);
            private set
            {
                var start = Text.Text.Substring(0, StartIndex);
                var end = Text.Text.Substring(EndIndex+1);
                EndIndex = _startIndex + value.Length-1;
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

        public void JoinLastField(FormFieldData field)
        {
            _isJoinedText = true;
            JoinedLastFormField = field;
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
                case "DeleteField":
                    SetValue("");
                    break;
                case "NameStudentField":
                    SetValue(GetFieldValueByName("NameStudentField"));
                    break;
                case "SurnameStudentField":
                    SetValue(GetFieldValueByName("SurnameStudentField"));
                    break;
                case "PatronymicStudentField":
                    SetValue(GetFieldValueByName("PatronymicStudentField"));
                    break;
                case "GroupStudentField":
                    SetValue(GetFieldValueByName("GroupStudentField"));
                    break;
                case "StudIdStudentField":
                    SetValue(GetFieldValueByName("StudIdStudentField"));
                    break;
                case "DepartamentStudent":
                    SetValue(GetFieldValueByName("DepartamentStudent"));
                    break;
                case "NPSurnameDean":
                    SetValue(GetFieldValueByName("NPSurnameDean"));
                    break;
                case "Date":
                    SetValue(GetFieldValueByName("Date"));
                    break;
            }
        }

        private string GetFieldValueByName(string nameField)
        {
            string patternStart = " <$";
            string patternEnd = "$> ";
            return $"{patternStart}{nameField}{patternEnd}";
        }

        public void ResetValue()
        {
            /*if(_isChanged)
                SetValue(_defaultValue);*/
        }
    }
}