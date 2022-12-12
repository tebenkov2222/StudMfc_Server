namespace Documents.Converter
{
    public interface IDocumentConverter
    {
        public void ConvertToDocx(string docPath, string docxPath);
    }
}