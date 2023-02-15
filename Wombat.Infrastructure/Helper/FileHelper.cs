using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Wombat.Infrastructure
{
    public class FileHelper
    {
        public static bool ExistFile(string path)
        {
            return File.Exists(path);
        }

        public static string GetFilePath(string path)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        public static string GetParentFilePath(string path)
        {
            return Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, path);
        }

        public static string GetParentPath()
        {
            return Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
        }

        public static string GetFileName(string path)
        {
            string name = Path.GetFileName(path);
            return System.Net.WebUtility.UrlDecode(name);
        }

        public static string GetFileVersion(string path)
        {
            try
            {
                FileInfo file = new FileInfo(path);
                if (file != null && file.Exists)
                {
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(file.FullName);
                    return versionInfo.FileVersion;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }
        }

        public static string ReadAllText(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }
            try
            {
                return File.ReadAllText(path);
            }
            catch
            {
                return "";
            }
        }

        public static void WriteAllText(string path, string content)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            try
            {
                File.WriteAllText(path, content);
            }
            catch
            {

            }
        }
    }
}
