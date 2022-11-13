using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Path = DocumentFormat.OpenXml.Drawing.Path;

namespace Documents.Documents
{
    public class DocumentBase: IDisposable
    {
        protected string _patchToFile;

        protected bool _isEditable;
        protected WordprocessingDocument _document;
        protected Body _body => _document.MainDocumentPart?.Document.Body;
        protected bool _isOpened;
        public string PatchToFile => _patchToFile;
        private string _name;
        private string _extenstion;
        public string Name => _name;

        public string Extenstion => _extenstion;
        public DocumentBase(string patchToFile, bool isEditable = false)
        {
            _patchToFile = patchToFile;
            _isEditable = isEditable;
        }

        public void Open()
        {
            var strings = _patchToFile.Split('\\').Last().Split('.');
            _name = strings[0];
            _extenstion = strings[1];
            _document = WordprocessingDocument.Open(_patchToFile, _isEditable);
            _isOpened = _document != null;
        }

        public void Save()
        {
            if(!_isOpened) return;
            _document.Save();
        }

        public void SaveAs(string path)
        {
            if(!_isOpened) return;
            _document.SaveAs(path);

        }
        public void Close()
        {
            if(!_isOpened) return;
            _isOpened = false;
            _document.Close();
        }

        public void Dispose()
        {
            Close();
        }

        public IEnumerable<FindTextData> FindTextByFirstEntry(string pattern)
        {
           return GetTexts().Where(t => t.Text.LastIndexOf(pattern) != -1)
                .Select(t => new FindTextData(ref t, t.Text.LastIndexOf(pattern), t.Text.Length -1));
        }
        public IEnumerable<FindTextData> FindTextByFirstAndLatestEntry(string startPattern, string endPattern)
        {
            var enumerable = GetTexts().Where(t => t.Text.LastIndexOf(startPattern) != -1 && t.Text.LastIndexOf(endPattern) != -1); ;
            return enumerable.Select(t => new FindTextData(ref t, t.Text.LastIndexOf(startPattern),t.Text.LastIndexOf(endPattern) + endPattern.Length));
        }
        protected IEnumerable<Paragraph> GetParagraphs()
        {
            return _body.Elements<Paragraph>();
        }
        protected IEnumerable<Run> GetRuns()
        {
            return GetParagraphs().SelectMany(t => t.Elements<Run>());
        }
        protected IEnumerable<Text> GetTexts()
        {
            return GetRuns().SelectMany(t => t.Elements<Text>());
        }
    }
}