using System;
using System.IO;

namespace DbStudio.Infrastructure.Shared.Helpers
{
    public static class FileSizeFormatter
    {
        // Load all suffixes in an array  
        static readonly string[] Suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        public static string FormatSize(long bytes)
        {
            var counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            return $"{number:n1}{Suffixes[counter]}";
        }

        public static string FormatSize(string file) => File.Exists(file) ? FileSizeFormatter.FormatSize(new FileInfo(file).Length) : default;
    }
}