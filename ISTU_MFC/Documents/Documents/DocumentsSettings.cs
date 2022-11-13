namespace Documents.Documents
{
    public class DocumentsSettings
    {
        public string RootPath;
        public string TempPath;
        public string InputPath;
        public string OutputPath;

        public static DocumentsSettings LocalHartmann = new DocumentsSettings()
        {
            RootPath = "E:\\Projects\\ISTU Projects\\MFC",
            TempPath = "Temp",
            InputPath = "Input",
            OutputPath = "Output"
        };
    }
}