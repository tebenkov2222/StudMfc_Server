
using System;
using System.Linq;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Packaging;
using Documents.Documents;

namespace Documents
{
    public class DocumentConverter
    {
        public void ConvertDocToDocx(string docPath, string docxPath)
        {
            SautinSoft.UseOffice u = new SautinSoft.UseOffice();
            int ret = u.InitWord();
            if (ret == 1)
            {
                Console.WriteLine("Error! Can't load MS Word library in memory");
                return;
            }
            ret = u.ConvertFile(docPath, docxPath, SautinSoft.UseOffice.eDirection.DOC_to_DOCX);
            u.CloseWord();
            var documentTemplate = new DocumentTemplate(docxPath, true);
            var run1 = documentTemplate.GetAllText().ToList()[0].Parent;
            var run2 = documentTemplate.GetAllText().ToList()[1].Parent;
            run1.Parent.RemoveChild(run1);
            run2.Parent.RemoveChild(run2);
            documentTemplate.Save();
            documentTemplate.Close();
            documentTemplate.Dispose();

        }
        public void ConvertDocxToDocx(string docPath, string docxPath)
        {
            SautinSoft.UseOffice u = new SautinSoft.UseOffice();
            int ret = u.InitWord();
            if (ret == 1)
            {
                Console.WriteLine("Error! Can't load MS Word library in memory");
                return;
            }
            ret = u.ConvertFile(docPath, docxPath, SautinSoft.UseOffice.eDirection.DOCX_to_DOCX);
            u.CloseWord();
            
            var document = WordprocessingDocument.Open(docxPath, true);
            var body = document.MainDocumentPart?.Document.Body;
            var bodyChildElement = body.ChildElements[0];
            body.RemoveChild(bodyChildElement);
            bodyChildElement = body.ChildElements[0];
            body.RemoveChild(bodyChildElement);
            document.Save();
            document.Close();
            document.Dispose();
        }
    }
}