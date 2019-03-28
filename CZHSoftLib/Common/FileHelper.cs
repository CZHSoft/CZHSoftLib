using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace CZHSoftLib.Common
{
    public class FileHelper
    {
        /// <summary>
        /// 获取程序根目录
        /// </summary>
        /// <returns></returns>
        public static string GetUpdateBasePath()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 获取程序所有子目录
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAppSubPathList()
        {
            List<string> temp = new List<string>();
            DirectoryInfo basePath = new DirectoryInfo(GetUpdateBasePath());
            temp.Add(basePath.FullName);
            temp.AddRange(GetDirectoryInfoPath(basePath));
            return temp;
        }

        /// <summary>
        /// 获取所有目录下的文件
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAppSubFileList()
        {
            List<string> temp = new List<string>();
            foreach (string s in GetAppSubPathList())
            {
                DirectoryInfo nowDic = new DirectoryInfo(s);
                FileInfo[] fileInfo = nowDic.GetFiles();
                foreach (FileInfo info in fileInfo)
                {
                    temp.Add(info.FullName);
                }
            }
            return temp;
        }

        /// <summary>
        /// 递归遍历目录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static List<string> GetDirectoryInfoPath(DirectoryInfo info)
        {
            List<string> temp = new List<string>();
            DirectoryInfo[] subDirectories = info.GetDirectories();
            foreach (DirectoryInfo dic in subDirectories)
            {
                temp.Add(dic.FullName);
                temp.AddRange(GetDirectoryInfoPath(dic));
            }
            return temp;
        }

        public static bool CheckFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public static void DelFile(string filePath)
        {
            File.Delete(filePath);
        }

        public bool CheckPathExist(string path)
        {
            return Directory.Exists(path);
        }

        public static void CreatePath(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// 获取文件的MD5
        /// </summary>
        public static byte[] GetFileMd5Byte(string filePath)
        {
            byte[] hashValueByte = null;

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider())
                {
                    hashValueByte = md5Hasher.ComputeHash(fileStream);
                }
            }

            return hashValueByte;
        }

        /// <summary>
        /// 获取流的MD5
        /// </summary>
        public static byte[] GetByteMd5Byte(byte[] data)
        {
            byte[] hashValueByte = null;

            using (MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider())
            {
                hashValueByte = md5Hasher.ComputeHash(data);
            }

            return hashValueByte;
        }

        /// <summary>
        /// 检查两个MD5是否一致
        /// </summary>
        public static bool CheckMd5Equals(byte[] md5_1, byte[] md5_2)
        {
            if (md5_1 == null || md5_2 == null)
            {
                return false;
            }
            if (md5_1.Length != md5_2.Length)
            {
                return false;
            }

            for (int i = 0; i < md5_1.Length; i++)
            {
                if (md5_1[i] != md5_2[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 获取文件的MD5的字符串形式
        /// </summary>
        public static string GetFileMd5String(string filePath)
        {
            string resString = string.Empty;
            string hashDataString = string.Empty;

            byte[] hashValueByte = null;

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider())
                {
                    hashValueByte = md5Hasher.ComputeHash(fileStream);
                }
            }

            if (hashValueByte != null)
            {
                hashDataString = BitConverter.ToString(hashValueByte);
                hashDataString = hashDataString.Replace("-", "");
                return resString = hashDataString;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取流的MD5的字符串形式
        /// </summary>
        public static string GetByteMd5String(byte[] data)
        {
            string resString = string.Empty;
            string hashDataString = string.Empty;
            byte[] hashValueByte = null;

            using (MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider())
            {
                hashValueByte = md5Hasher.ComputeHash(data);
            }

            if (hashValueByte != null)
            {
                hashDataString = BitConverter.ToString(hashValueByte);
                hashDataString = hashDataString.Replace("-", "");
                return resString = hashDataString;
            }
            else
            {
                return string.Empty;
            }
        }

        public static long GetFileSize(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            return fileInfo.Length;
        }

        public static bool CompressFile_2_0(string souFilePath, string desFilePath)
        {
            bool flag = false;

            if (CheckFileExist(desFilePath))
            {
                DelFile(desFilePath);
            }

            using (FileStream sourceFileStream = File.OpenRead(souFilePath))
            {
                using (FileStream destinationFileStream = File.Create(desFilePath))
                {
                    using (GZipStream gzStream = new GZipStream(destinationFileStream, CompressionMode.Compress))
                    {
                        //3.5
                        byte[] buffer = new byte[1024]; // fairly arbitrary size 

                        int bytesRead;

                        while ((bytesRead = sourceFileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            gzStream.Write(buffer, 0, bytesRead);
                        }
                        //4.0 
                        //sourceFileStream.CopyTo(gzStream);

                        flag = true;

                    }
                }
            }

            return flag;
        }

        public static byte[] GetFileByte(string filePath)
        {
            FileStream stream = new FileInfo(filePath).OpenRead();
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
            return buffer;
        }
    }
}
