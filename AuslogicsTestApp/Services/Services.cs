using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using AuslogicsTestApp.Models;
using Microsoft.Win32;

namespace AuslogicsTestApp
{
    /// <summary>
    /// Static class with common methods
    /// </summary>
    public static class Services
    {
        /// <summary>
        /// Returns a list of <see cref="FileInfoModel"/> contained in <paramref name="registryKey"/>
        /// </summary>
        /// <param name="registryKey">Registry key</param>
        /// <returns>List of FileInfoModel</returns>
        public static List<FileInfoModel> GetFilesInfoFromRegistry(RegistryKey registryKey)
        {
            List<FileInfoModel> filesInfo = new List<FileInfoModel>();

            if (registryKey is null)
            {
                return filesInfo;
            }

            foreach (var registryValueName in registryKey.GetValueNames())
            {
                var registryValue = (string)registryKey.GetValue(registryValueName);

                var file = Regex.Match(registryValue, ".:\\\\.+exe").Value;
                var command = Regex.Match(registryValue, "\\\" .+$").Value.Replace("\" ", string.Empty);

                filesInfo.Add(GetFileInfo(file, command, "Registry"));
            }

            return filesInfo;
        }

        /// <summary>
        /// Returns a list of <see cref="FileInfoModel"/> contained in <paramref name="directory"/>
        /// </summary>
        /// <param name="directory">Directory</param>
        /// <returns>List of <see cref="FileInfoModel"/></returns>
        public static List<FileInfoModel> GetFilesInfoFromStartupMenu(string directory)
        {
            List<FileInfoModel> filesInfo = new List<FileInfoModel>();

            if (directory is null)
            {
                return filesInfo;
            }

            foreach (var file in Directory.GetFiles(directory))
            {
                var fileInfo = GetFileInfo(file, "", "StartupMenu");

                var link = File.ReadAllBytes(file);

                //using (var stream = new FileStream(file, FileMode.Open))
                //{
                //    while (stream.CanRead)
                //    {
                //        var newByte = stream.ReadByte();
                //    }
                //}

                // Unneccessary ini-files
                if (fileInfo.Info.Extension != ".ini")
                {
                    filesInfo.Add(fileInfo);
                }
            }

            return filesInfo;
        }

        /// <summary>
        /// Returns a <see cref="FileInfoModel"/> instance for <paramref name="file"/> with <paramref name="command"/> and <paramref name="autostartType"/>
        /// </summary>
        /// <param name="file"></param>
        /// <param name="command"></param>
        /// <param name="autostartType"></param>
        /// <returns><see cref="FileInfoModel"/> instance</returns>
        public static FileInfoModel GetFileInfo(string file, string command, string autostartType)
        {
            // Extract icon and convert into bitmap source
            var icon = Imaging.CreateBitmapSourceFromHIcon(Icon.ExtractAssociatedIcon(file).Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            icon.Freeze();

            var fileInfoModel = new FileInfoModel()
            {
                FileIcon = icon,
                Info = new FileInfo(file),
                Parameters = command,
                AutostartType = autostartType,
            };

            return fileInfoModel;
        }
    }
}
