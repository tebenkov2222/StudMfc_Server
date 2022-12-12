using System;
using System.IO;
using DocumentFormat.OpenXml.Packaging;

namespace Documents.Converter
{
    public class SautinDocConverter: IDocumentConverter
    {
        public void ConvertToDocx(string docPath, string docxPath)
        {
            // Convert DOC file to DOCX file.
            // If you need more information about UseOffice .Net email us at:
            // support@sautinsoft.com.
            SautinSoft.UseOffice u = new SautinSoft.UseOffice();

            // Prepare UseOffice .Net, loads MS Word in memory
            int ret = u.InitWord();

            // Return values:
            // 0 - Loading successfully
            // 1 - Can't load MS Word library in memory

            if (ret == 1)
            {
                Console.WriteLine("Error! Can't load MS Word library in memory");
                return;
            }

            // Perform the conversion.
            ret = u.ConvertFile(docPath, docxPath, SautinSoft.UseOffice.eDirection.DOC_to_DOCX);

            // Release MS Word from memory
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