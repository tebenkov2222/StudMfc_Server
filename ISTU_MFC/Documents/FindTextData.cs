using DocumentFormat.OpenXml.Wordprocessing;

namespace Documents
{
    public class FindTextData
    {
        public Text Text;
        public int StartIndex = -1;
        public int EndIndex = -1;
        public string Substring() => Text.Text.Substring(StartIndex, EndIndex - StartIndex);

        public FindTextData(Text text, int startIndex, int endIndex)
        {
            Text = text;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }
    }
}