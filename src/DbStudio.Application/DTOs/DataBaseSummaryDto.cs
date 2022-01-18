using System.Collections.Generic;

namespace DbStudio.Application.DTOs
{
    public class DataBaseSummaryDto
    {
        public string Version { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public IEnumerable<string> Tables { get; set; }
        public IEnumerable<string> Jobs { get; set; }
    }
}