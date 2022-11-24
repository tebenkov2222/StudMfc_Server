using System;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Documents
{
    public class TemplateField
    {
        public readonly Text Text;
        public readonly int StartIndex;
        public readonly int EndIndex;
        public readonly string Name;

        public TemplateField(FindTextData findTextData)
        {
            Text = findTextData.Text;
            StartIndex = findTextData.StartIndex;
            EndIndex = findTextData.EndIndex;
            var substring = findTextData.Substring();
            Name = substring.Substring(2, substring.Length - 4);
        }

        public TemplateField(Text text, int startIndex, int endIndex, string name)
        {
            Text = text;
            StartIndex = startIndex;
            EndIndex = endIndex;
            Name = name;
        }

        public void SetValue(string value, int startOffset = 0, int endOffset = 0)
        {
            var textValue = Text.Text;
            Text.Text = textValue.Substring(0, StartIndex + startOffset) + value +
                        textValue.Substring(EndIndex + endOffset);
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
    }
}