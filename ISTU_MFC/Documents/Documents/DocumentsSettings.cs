using System.IO;

namespace Documents.Documents
{
    public class DocumentsSettings
    {
        public string RootPath;
        public string TempPath;
        public string InputPath;
        public string OutputPath;
        public string FormsInput;
        public string FormsTemp;

        public static DocumentsSettings LocalHartmann = new DocumentsSettings()
        {
            RootPath = "E:\\Projects\\ISTU Projects\\MFC",
            TempPath = "Temp",
            InputPath = "Input",
            OutputPath = "Output",
            FormsInput = "FormsInput",
            FormsTemp = "FormsTemp"
        };

        public static DocumentsSettings Directory;

        public static DocumentsSettings SetDirectoryByEnvironment(string environmentPath)
        {
            Path.GetFullPath(environmentPath);
            //var replace = environmentPath.Replace('\\','/');
            var replace = Path.GetFullPath(environmentPath);
            Directory = new DocumentsSettings()
            {
                RootPath = replace,
                TempPath =  Path.Combine("File", "Temp"),
                InputPath = Path.Combine("File", "Input"),
                OutputPath = Path.Combine("File", "Output"),
                FormsInput = Path.Combine("File", "FormsInput"),
                FormsTemp = Path.Combine("File", "FormsTemp")
            };
            return Directory;
        }
    }
}