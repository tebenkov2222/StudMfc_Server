using System;
using System.IO;
using DocumentFormat.OpenXml.Office2013.Excel;
using Documents.Documents;
using Documents.Fields;
using Repository;

namespace Documents
{
    public class DocumentsController
    {
        private readonly IRepository _repository;
        private readonly DocumentsSettings _settings = DocumentsSettings.LocalHartmann;

        private FieldsController _fieldsController;

        public FieldsController FieldsController => _fieldsController;
        public DocumentsController(IRepository repository)
        {
            _repository = repository;
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
        public DocumentTemplate CopyToTempAndOpenDocument(DocumentTemplate documentTemplate, string name, bool isEditable = false)
        {
            return CopyAndOpenDocument(documentTemplate, GetPathByName(_settings.TempPath, name), isEditable);
        }

        public string GetPathByName(string directory, string docName, string extention = "docx")
        {
            return $"{_settings.RootPath}\\{directory}\\{docName}.{extention}";
        }
    }
}