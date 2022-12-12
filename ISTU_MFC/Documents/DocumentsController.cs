using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using Documents.Converter;
using Documents.Documents;
using Documents.Fields;
using Documents.View;
using Repository;

namespace Documents
{
    public class DocumentsController
    {
        private DocumentViewer _documentViewer;

        public DocumentViewer DocumentViewer => _documentViewer;
        private readonly IRepository _repository;
        private readonly DocumentsSettings _settings = DocumentsSettings.LocalHartmann;

        public DocumentsSettings Settings => _settings;
        private FieldsController _fieldsController;

        public FieldsController FieldsController => _fieldsController;
        public DocumentsController(IRepository repository)
        {
            _repository = repository;
            _documentViewer = new DocumentViewer(this);
            _fieldsController = new FieldsController(repository);
        }
        public DocumentTemplate OpenDocumentAsTemplateByName(string docName, bool isEditable = false)
        {
            var documentTemplate = new DocumentTemplate( GetPathByName(_settings.InputPath, docName), isEditable);
            return documentTemplate;
        }
        public DocumentTemplate OpenDocumentAsTemplateByPath(string docPath, bool isEditable = false)
        {
            var documentTemplate = new DocumentTemplate(docPath, isEditable);
            return documentTemplate;
        }
        public DocumentForm OpenDocumentAsFormByPathWithValidate(string docPath, bool isEditable = false)
        {
            string name = Path.GetFileNameWithoutExtension(docPath);
            var extension = Path.GetExtension(docPath);
            var documentConverter = new DocumentConverter();
            var documentOutput = GetPathByName(_settings.TempPath, name, "docx");
            if (extension == ".doc")
            {
                documentConverter.ConvertDocToDocx(docPath, documentOutput);
            }
            else if (extension == ".docx")
            {
                documentConverter.ConvertDocToDocx(docPath, documentOutput);
            }
            var documentTemplate = new DocumentForm(documentOutput, isEditable);
            Text lastText = new Text();
            foreach (var text in documentTemplate.GetAllText())
            {
                if (text.Text.Length > 0 && lastText.Text.Length > 0)
                {
                    if (lastText.Text.Last() == '_' && text.Text.First() == '_')
                    {
                        lastText.Text += text.Text;
                        text.Text = "";
                    }
                }
                lastText = text;
            }
            return documentTemplate;
        }
        public DocumentForm OpenDocumentAsFormByPath(string docPath, bool isEditable = false)
        {
            var documentTemplate = new DocumentForm(docPath, isEditable);
            Text lastText = new Text();
            foreach (var text in documentTemplate.GetAllText())
            {
                if (text.Text.Length > 0 && lastText.Text.Length > 0)
                {
                    if (lastText.Text.Last() == '_' && text.Text.First() == '_')
                    {
                        lastText.Text += text.Text;
                        text.Text = "";
                    }
                }
                lastText = text;
            }
            return documentTemplate;
        }
        public DocumentBase CopyAndOpenDocument(DocumentBase documentTemplate, string pathTo, bool isEditable = false) 
        {
            documentTemplate.Close();
            File.Copy(documentTemplate.PatchToFile, pathTo, true);
            return new DocumentBase (pathTo, isEditable);
        }
        public DocumentBase CopyToTempAndOpenDocument(DocumentBase documentTemplate, string nameNewFile, bool isEditable = false)
        {
            return CopyAndOpenDocument(documentTemplate, GetPathByName(_settings.TempPath, nameNewFile), isEditable);
        }
        public DocumentBase CopyToTempAndOpenDocument(string nameTemplateFile, string nameNewFile, bool isEditable = false)
        {
            return CopyToTempAndOpenDocument(OpenDocumentAsTemplateByName(nameTemplateFile), nameNewFile, isEditable);
        }

        public string GetPathByName(string directory, string docName, string extention = "docx")
        {
            return Path.Combine(_settings.RootPath, directory, $"{docName}.{extention}");
            return $"{_settings.RootPath}/{directory}/{docName}.{extention}";
        }
    }
}