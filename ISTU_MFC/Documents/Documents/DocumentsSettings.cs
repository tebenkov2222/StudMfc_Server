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

        public static DocumentsSettings Directory;

        public static DocumentsSettings SetDirectoryByEnvironment(string environmentPath)
        {
            var replace = environmentPath.Replace('\\','/');
            Directory = new DocumentsSettings()
            {
                RootPath = replace,
                TempPath = "File/Temp",
                InputPath = "File/Input",
                OutputPath = "File/Output"
            };
            return Directory;
        }
    }
}