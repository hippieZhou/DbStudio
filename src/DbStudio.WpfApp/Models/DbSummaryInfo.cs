namespace DbStudio.WpfApp.Models
{
    public class DbSummaryInfo
    {
        public string DataSource { get; set; }
        public string InitialCatalog { get; set; }
        public string Version { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public int TableCount { get; set; }
    }
}