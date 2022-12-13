using System;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Net.Mime;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Spire.Doc;
using Spire.Doc.Documents;
using DocumentBase = Documents.Documents.DocumentBase;

namespace Documents.View
{
    public class DocumentViewer
    {
        //private readonly DocumentsController _controller;

        public DocumentViewer()
        {
        }
        public Byte[] GenerateBytesByFilePath(string path)
        {
             return File.ReadAllBytes(path);
        }

        public void GenerateAndSavePdf(string pathDocxFile, string pathPdfFile)
        {
            var document = new Spire.Doc.Document();
            document.LoadFromFile(pathDocxFile);
            document.SaveToFile(pathPdfFile, FileFormat.PDF);
            //var toPdfParameterList = new ToPdfParameterList();
            //toPdfParameterList.
            //document.SaveToFile(pathPdfFile, toPdfParameterList);
            document.Dispose();
        }
        /*public void GenerateAndSavePdf(string pathDocxFile, string pathPdfFile)
        {
            var document = new Document();
            document.SaveToFile(pathPdfFile, FileFormat.PDF);
            document.Dispose();
        }*/
        /*public void GenerateAndSavePdf(string pathDocxFile, string pathPdfFile)
        {
            var source = Package.Open(@"test.docx");
            var document = WordprocessingDocument.Open(source);
            
            HtmlConverterSettings settings = new HtmlConverterSettings();
            XElement html = HtmlConverter.ConvertToHtml(document, settings);

            Console.WriteLine(html.ToString());
            var writer = File.CreateText("test.html");
            writer.WriteLine(html.ToString());
            writer.Dispose();
            Console.ReadLine();
        }*/
        /*public void GenerateAndSavePdf(string pathDocxFile, string pathPdfFile)
        {
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
            ret = u.ConvertFile(pathDocxFile, pathPdfFile, SautinSoft.UseOffice.eDirection.DOCX_to_PDF);
            u.CloseWord();
        }*/
        /*public byte[] GenerateAndSaveImage(DocumentBase documentReference)
        {
            /*Document document = new Document();
            document.LoadFromFile(documentReference.PatchToFile);
            Image image = document.SaveToOnlineBin();
            document.Close();
            var imageConverter = new ImageConverter();
            var byteArray = (byte[]) imageConverter.ConvertTo(image, typeof(byte[]));
            return byteArray;*
        }*/
        //DocGeneration:2
        /*public void GenerateAndSaveImage(DocumentBase documentReference)
        {
            var converter = new GroupDocs.Conversion.Converter(documentReference.PatchToFile);
            // Prepare conversion options for target format JPG
            var convertOptions = converter.GetPossibleConversions()["jpg"].ConvertOptions;
            // Convert to JPG format
            var pathByName = _controller.GetPathByName(_controller.Settings.TempPath,documentReference.Name,"jpg");

            converter.Convert(pathByName, convertOptions);
            DocumentBuilder.
        }*/

    }
}