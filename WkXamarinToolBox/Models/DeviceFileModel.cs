namespace WkXamarinToolBox.Models
{
    public class DeviceFileModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public byte[] ByteArray { get; set; }

        public DeviceFileModel() { }

        public DeviceFileModel(string filePath, string fileName)
        {
            FilePath = filePath;
            FileName = fileName;
        }
    }
}
