using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
namespace Documents.Converter
{
    public class GroupDocsDocumentConverter: IDocumentConverter
    {
        public void ConvertToDocx(string docPath, string docxPath)
        {
            var converter = new GroupDocs.Conversion.Converter(docPath);
            // Prepare conversion options for target format DOCX
            var convertOptions = converter.GetPossibleConversions()["docx"].ConvertOptions;
            // Convert to DOCX format
            converter.Convert(docxPath, convertOptions);
            var document = WordprocessingDocument.Open(docxPath, true);
            var body = document.MainDocumentPart?.Document.Body;
            var bodyChildElement = body.ChildElements[0];
            body.RemoveChild(bodyChildElement);
            FixRuns(body);
            document.Save();
            document.Close();
            document.Dispose();
        }

        private void FixRuns(Body body)
        {
            var enumerable = body.Elements<Paragraph>();
            foreach (var p in enumerable) 
            {
                Console.WriteLine("---");
                var elements = p.Elements<Run>();
                var text = elements.SelectMany(r => r.Elements<Text>());
                var count = text.Count();
                string resultText = "";
                List<Run> runsToDelete = new();
                Console.WriteLine("Texts >>");
                Text lastText = new Text();
                foreach (var t in text)
                {
                    resultText += t.Text;
                    var runProperties = t.Parent.ChildElements[0] as RunProperties;
                    Console.WriteLine($"Text = [{t.Text}], last children = {lastText.Text} runProp = {runProperties != null}");
                    if (runProperties == null)
                    {
                        lastText.Text += t.Text;
                        runsToDelete.Add(t.Parent as Run);
                    }
                    else
                    {
                        Console.WriteLine($"fontSize = {runProperties.FontSize.Val}");
                    }

                    lastText = t;
                }
                Console.WriteLine("Texts >>");
                foreach (var run in runsToDelete)
                {
                    p.RemoveChild(run);
                }

                Console.WriteLine($"count = {count}, Text = [{resultText}]");
                Console.WriteLine("!---");
            }
        }
    }
}