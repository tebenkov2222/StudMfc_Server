using Spire.Doc;

namespace Documents.Converter
{
    public class SpireDocumentConverter: IDocumentConverter
    {
        public void ConvertToDocx(string docPath, string docxPath)
        {
            var document = new Document();
            document.LoadFromFile(docPath,FileFormat.Doc);
            document.SaveToFile(docxPath, FileFormat.Docx2010);
        }
    }
}