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
                .Select(t => new FindTextData(t, t.Text.LastIndexOf(pattern), t.Text.Length -1));
        }
        private List<(int startIndex, int endIndex)> CheckText(Text text, string patternStart, string patternEnd)
        {
            var list = new List<(int startIndex, int endIndex)>();
            var textValue = text.Text;
            bool isPatternStarted = false;
            int startIndex = 0;
            string textAfter = "", textBefore = "";
            textAfter = textValue;
            for (var i = 0; i < textValue.Length; i++)
            {
                char c = textValue[i];
                    
                if (textAfter.StartsWith(patternStart))
                {
                    if (!isPatternStarted)
                    {
                        startIndex = i;
                    }
                    isPatternStarted = true;
                }
                else
                if (textBefore.EndsWith(patternEnd))
                {
                    if (isPatternStarted)
                    {
                        isPatternStarted = false;
                        list.Add((startIndex, i));
                    }
                }
                if (i == textValue.Length - 1)
                {
                    if (isPatternStarted)
                    {
                        list.Add((startIndex, i));
                    }
                }

                textBefore += textAfter.Length > 0 ? textAfter[0] : "";
                textAfter = textAfter.Length > 0 ? textAfter.Substring(1) : "";
            }
            return list;
        }
        public IEnumerable<FindTextData> FindTextByFirstAndLatestEntry(string startPattern, string endPattern)
        {
            List<FindTextData> res = new List<FindTextData>();
            foreach (var text in GetAllText())
            {
                var valueTuples = CheckText(text, startPattern, endPattern);
                foreach (var fieldCoord in valueTuples)
                {
                    res.Add(new FindTextData(text,fieldCoord.startIndex, fieldCoord.endIndex) );
                }
            }

            return res;
            var enumerable = GetAllText().Where(t => t.Text.LastIndexOf(startPattern) != -1 && t.Text.LastIndexOf(endPattern) != -1);

            return enumerable.Select(t => new FindTextData(t, t.Text.LastIndexOf(startPattern),t.Text.LastIndexOf(endPattern) + endPattern.Length));
        }
        public IEnumerable<Paragraph> GetParagraphs()
        {
            return _body.Elements<Paragraph>();
        }
        public IEnumerable<Table> GetTable()
        {
            return _body.Elements<Table>();
        }
        public IEnumerable<TableRow> GetTableRow(Table table)
        {
            return table.Elements<TableRow>();
        }
        public IEnumerable<TableCell> GetTableCell(Table table)
        {
            return GetTableRow(table).SelectMany(t => t.Elements<TableCell>());
        }
        public IEnumerable<Paragraph> GetParagraphsOnTable(Table table)
        {
            return GetTableCell(table).SelectMany(t => t.Elements<Paragraph>());
        }
        public IEnumerable<Run> GetRunsOnTable(Table table)
        {
            return GetParagraphsOnTable(table).SelectMany(t => t.Elements<Run>());
        }
        public IEnumerable<Text> GetTextOnTable(Table table)
        {
            return GetRunsOnTable(table).SelectMany(t => t.Elements<Text>());
        }
        public IEnumerable<Run> GetRuns()
        {
            return GetParagraphs().SelectMany(t => t.Elements<Run>());
        }
        public IEnumerable<Text> GetTexts()
        {
            return GetRuns().SelectMany(t => t.Elements<Text>());
        }
        public IEnumerable<Run> GetRunsOnParagraph(Paragraph paragraph)
        {
            return paragraph.Elements<Run>();
        }
        public IEnumerable<Text> GetTextsOnParagraph(Paragraph paragraph)
        {
            return GetRunsOnParagraph(paragraph).SelectMany(t => t.Elements<Text>());
        }

        public IEnumerable<Text> GetAllText()
        {
            List<Text> result = new();
            foreach (var element in _body.ChildElements)
            {
                if (element.GetType() == typeof(Table))
                {
                    var textOnTable = GetTextOnTable(element as Table);
                    result.AddRange(textOnTable);
                }
                else if (element.GetType() == typeof(Paragraph))
                {
                    result.AddRange(GetTextsOnParagraph(element as Paragraph));
                }
            }

            return result;
            /*var textsOnTable = GetTextOnTable().ToList();
            var texts = GetTexts().ToList();
            texts.AddRange(textsOnTable);
            return texts;*/
        }
    }
}