using DocumentFormat.OpenXml.Wordprocessing;

namespace Documents.Fields
{
    public class FormFieldData
    {
        public Text Text;
        public string Name;
        public int StartIndex;
        public int EndIndex;
        private int _defaultCount;
        public string Value
        {
            get => Text.Text.Substring(StartIndex, EndIndex - StartIndex);
            private set
            {
                var start = Text.Text.Substring(0, StartIndex);
                var end = Text.Text.Substring(EndIndex+1);
                EndIndex = StartIndex + value.Length;
                Text.Text = start + value + end;
            }
        }

        public FormFieldData(Text text, string name, int startIndex, int endIndex)
        {
            Name = name;
            StartIndex = startIndex;
            EndIndex = endIndex;
            _defaultCount = EndIndex - StartIndex + 1;
            Text = text;
        }

        public void SetValue(string value)
        {
            Value = value;
        }

        public void ResetValue()
        {
            string result = "";
            for (int i = 0; i < _defaultCount; i++)
            {
                result += '_';
            }
            SetValue(result);
        }
    }
}