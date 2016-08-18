using System;

namespace CapsCollection.Desktop.Infrastructure.Extensions
{
    public static class ByteExtensions
    {
        public static string GetBytesSize(byte[] bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes.Length;
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }

            string result = String.Format("{0:0.##} {1}", len, sizes[order]);

            return result;
        }

        public static string ToReadaleSize(this byte[] bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes.Length;
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }

            string result = String.Format("{0:0.##} {1}", len, sizes[order]);

            return result;
        }
    }
}
