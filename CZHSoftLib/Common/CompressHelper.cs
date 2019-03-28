using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

namespace CZHSoft.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class CompressHelper
    {
        public static string Compress(string str)
        {
            //因输入的字符串不是Base64所以转换为Base64,因为HTTP如果不传递Base64则会发生http 400错误
            return System.Convert.ToBase64String(Compress(System.Convert.FromBase64String(System.Convert.ToBase64String(Encoding.Default.GetBytes(str)))));
        }

        public static string Decompress(string str)
        {
            return Encoding.Default.GetString(Decompress(System.Convert.FromBase64String(str)));
        }

        public static byte[] Compress(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                GZipStream Compress = new GZipStream(ms, CompressionMode.Compress);
                Compress.Write(bytes, 0, bytes.Length);
                Compress.Close();
                return ms.ToArray();
            }
        }

        public static byte[] Decompress(Byte[] bytes)
        {
            using (MemoryStream tempMs = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    GZipStream Decompress = new GZipStream(ms, CompressionMode.Decompress);
                    Decompress.CopyTo(tempMs);
                    Decompress.Close();
                    return tempMs.ToArray();
                }
            }
        }
    }
}
