using System;
using System.Drawing;
using System.IO;
using System.Net.Mime;
using OpenXmlPowerTools;
using Spire.Doc;
using Spire.Doc.Documents;
using DocumentBase = Documents.Documents.DocumentBase;

namespace Documents.View
{
    public class DocumentViewer
    {
        private readonly DocumentsController _controller;

        public DocumentViewer(DocumentsController controller)
        {
            _controller = controller;
        }
        public Byte[] GenerateBytesByFilePath(string path)
        {
             return File.ReadAllBytes(path);
        }
        public string GenerateAndSaveImage(DocumentBase documentReference)
        {
            Document document = new Document();
            document.LoadFromFile(documentReference.PatchToFile);
            Image image = document.SaveToImages(0, ImageType.Metafile);
            var pathByName = _controller.GetPathByName(_controller.Settings.TempPath,
                documentReference.Name, "jpg");
            document.Close();
            var path = pathByName;
            image.Save(path);
            return path;
        }
        public string GenerateAndSaveImage(DocumentBase documentReference, string pathToSave)
        {
            //Document document = new Document();
            //document.LoadFromFile(documentReference.PatchToFile);
            //Image image = document.SaveToImages(0, ImageType.Metafile);
            //document.Close();
            var path = pathToSave;
            //image.Save(path);
            return path;
        }
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