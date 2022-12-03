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

        public WordprocessingDocument Document => _document;
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
           return GetAllText().Where(t => t.Text.LastIndexOf(pattern) != -1)
                .Select(t => new FindTextData(ref t, t.Text.LastIndexOf(pattern), t.Text.Length -1));
        }
        public IEnumerable<FindTextData> FindTextByFirstAndLatestEntry(string startPattern, string endPattern)
        {
            var enumerable = GetAllText().Where(t => t.Text.LastIndexOf(startPattern) != -1 && t.Text.LastIndexOf(endPattern) != -1); ;
            return enumerable.Select(t => new FindTextData(ref t, t.Text.LastIndexOf(startPattern),t.Text.LastIndexOf(endPattern) + endPattern.Length));
        }
        public IEnumerable<Paragraph> GetParagraphs()
        {
            return _body.Elements<Paragraph>();
        }
        public IEnumerable<Table> GetTable()
        {
            return _body.Elements<Table>();
        }
        public IEnumerable<TableRow> GetTableRow()
        {
            return GetTable().SelectMany(t => t.Elements<TableRow>());
        }
        public IEnumerable<TableCell> GetTableCell()
        {
            return GetTableRow().SelectMany(t => t.Elements<TableCell>());
        }
        public IEnumerable<Paragraph> GetParagraphsOnTable()
        {
            return GetTableCell().SelectMany(t => t.Elements<Paragraph>());
        }
        public IEnumerable<Run> GetRunsOnTable()
        {
            return GetParagraphsOnTable().SelectMany(t => t.Elements<Run>());
        }
        public IEnumerable<Text> GetTextOnTable()
        {
            return GetRunsOnTable().SelectMany(t => t.Elements<Text>());
        }
        public IEnumerable<Run> GetRuns()
        {
            return GetParagraphs().SelectMany(t => t.Elements<Run>());
        }
        public IEnumerable<Text> GetTexts()
        {
            return GetRuns().SelectMany(t => t.Elements<Text>());
        }

        public IEnumerable<Text> GetAllText()
        {
            var textsOnTable = GetTextOnTable().ToList();
            var texts = GetTexts().ToList();
            texts.AddRange(textsOnTable);
            return texts;
        }
    }
}