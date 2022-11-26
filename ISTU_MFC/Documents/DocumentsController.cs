using System;
using System.IO;
using DocumentFormat.OpenXml.Office2013.Excel;
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
        private readonly DocumentsSettings _settings = DocumentsSettings.Directory;

        public DocumentsSettings Settings => _settings;
        private FieldsController _fieldsController;

        public FieldsController FieldsController => _fieldsController;
        public DocumentsController(IRepository repository)
        {
            _repository = repository;
            _documentViewer = new DocumentViewer(this);
            _fieldsController = new FieldsController(repository);
        }
        //public void 
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

        public DocumentTemplate CopyAndOpenDocument(DocumentTemplate documentTemplate, string pathTo, bool isEditable = false) 
        {
            documentTemplate.Close();
            File.Copy(documentTemplate.PatchToFile, pathTo, true);
            return new DocumentTemplate (pathTo, isEditable);
        }
        public DocumentTemplate CopyToTempAndOpenDocument(DocumentTemplate documentTemplate, string nameNewFile, bool isEditable = false)
        {
            return CopyAndOpenDocument(documentTemplate, GetPathByName(_settings.TempPath, nameNewFile), isEditable);
        }
        public DocumentTemplate CopyToTempAndOpenDocument(string nameTemplateFile, string nameNewFile, bool isEditable = false)
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