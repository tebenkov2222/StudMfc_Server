using System;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Documents
{
    public class TemplateField
    {
        public TemplateField LastTemplateField;
        public readonly Text Text;

        public int StartIndex
        {
            get
            {
                if (_isLastTemplateField)
                {
                    return _startIndex + LastTemplateField.EndIndex;
                }
                return _startIndex;
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
                if (_isLastTemplateField)
                {
                    return _endIndex + LastTemplateField.EndIndex;
                }
                return _endIndex;
            }
            set
            {
                _endIndex = value;
            }
        }
        private int _startIndex;
        private int _endIndex;
        public readonly string Name;
        private bool _isLastTemplateField;

        public TemplateField(FindTextData findTextData)
        {
            Text = findTextData.Text;
            StartIndex = findTextData.StartIndex;
            EndIndex = findTextData.EndIndex;
            var substring = findTextData.Substring();
            Name = substring.Substring(2, substring.Length - 4);
        }

        public void SetValue(string value, int startOffset = 0, int endOffset = 0)
        {
            var textValue = Text.Text;
            Text.Text = textValue.Substring(0, StartIndex + startOffset) + value +
                        textValue.Substring(EndIndex + endOffset);
            EndIndex = _startIndex + value.Length;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TemplateField);
        }
        private bool Equals(TemplateField that)
        {
            if (that == null)
            {
                return false;
            }
            return Equals(this.Name, that.Name);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public void JointField(TemplateField templateField)
        {
            LastTemplateField = templateField;
            _isLastTemplateField = true;
            _startIndex -= templateField.EndIndex;
            _endIndex -= templateField.EndIndex;
        }
    }
}