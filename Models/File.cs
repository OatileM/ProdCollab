namespace ProdCollab.Models
{
    public class File
    {

        public string FileName { get; set; }
        public int FileSize { get; set; }
        public DateOnly? DateUploaded { get; set; }
        public string? FilePath { get; set; }

        public File(string fileName, int fileSize)
        {
            FileName = fileName;
            FileSize = fileSize;

        }
    }
}
