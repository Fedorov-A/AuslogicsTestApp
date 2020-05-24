using AuslogicsTestApp.Models;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

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
                // Unneccessary ini-files
                if ((new FileInfo(file)).Extension != ".ini")
                {
                    var link = WindowsShortcutParser(file);

                    filesInfo.Add(link);
                }
            }

            return filesInfo;
        }

        /// <summary>
        /// Very basic windows shortcut parser
        /// <para>Based on https://github.com/libyal/liblnk/blob/master/documentation/Windows%20Shortcut%20File%20(LNK)%20format.asciidoc</para>
        /// </summary>
        /// <param name="path">Path to windows shortcut file</param>
        /// <returns>FileInfoModel of executable file</returns>
        public static FileInfoModel WindowsShortcutParser(string path)
        {
            string executableFilePath = "";
            var parameters = "";

            using (var binaryReader = new BinaryReader(File.OpenRead(path), Encoding.ASCII))
            {
                // Header
                var headerSize = binaryReader.ReadUInt32();
                var guid = binaryReader.ReadBytes(16);
                var dataFlags = binaryReader.ReadUInt32();
                var fileAttributes = binaryReader.ReadUInt32();
                var creationDateAndTime = binaryReader.ReadUInt64();
                var lastAccessDateAndtime = binaryReader.ReadUInt64();
                var lastModificationDateAndTime = binaryReader.ReadUInt64();
                var fileSizeInBytes = binaryReader.ReadUInt32();
                var iconIndexValue = binaryReader.ReadUInt32();
                var showWindowValue = binaryReader.ReadUInt32();
                var hotKey = binaryReader.ReadUInt16();
                var unknown1 = binaryReader.ReadUInt16();
                var unknown2 = binaryReader.ReadUInt32();
                var unknown3 = binaryReader.ReadUInt32();

                var dataFlagsBits = new BitArray(new int[] { (int) dataFlags });
                               
                // Link target identifier
                if (dataFlagsBits.Get(0))
                {
                    // Skip uneccessary data
                    var bytesToSkip = binaryReader.ReadUInt16();
                    binaryReader.BaseStream.Seek(bytesToSkip, SeekOrigin.Current);
                }

                // Location information
                if (dataFlagsBits.Get(1))
                {
                    var sizeOfLocationInfo = binaryReader.ReadUInt32();
                    var locationInformationHeaderSize = binaryReader.ReadUInt32();
                    var locationFlags= binaryReader.ReadUInt32();
                    var offsetVolume= binaryReader.ReadUInt32();
                    var offsetLocalPath = binaryReader.ReadUInt32();
                    var offsetNetwork = binaryReader.ReadUInt32();
                    var offsetCommonPath = binaryReader.ReadUInt32();
                    
                    // Jump to local path
                    binaryReader.BaseStream.Seek(offsetLocalPath - locationInformationHeaderSize, SeekOrigin.Current);

                    List<byte> charList = new List<byte>();

                    byte newChar = binaryReader.ReadByte();

                    while (newChar != '\0')
                    {
                        charList.Add(newChar);
                        newChar = binaryReader.ReadByte();
                    }

                    executableFilePath = Encoding.GetEncoding("Windows-1251").GetString(charList.ToArray());

                    newChar = binaryReader.ReadByte();

                    while (newChar != '\0')
                    {
                        charList.Add(newChar);
                        newChar = binaryReader.ReadByte();
                    }
                }

                // Description
                if (dataFlagsBits.Get(2))
                {
                    // Unneccessary
                    var charsNumber = binaryReader.ReadUInt16();
                    var description = readDataStrings(binaryReader, charsNumber);
                }

                // Relative path
                if (dataFlagsBits.Get(3))
                {
                    // Unneccessary
                    var charsNumber = binaryReader.ReadUInt16();
                    var relativePath = readDataStrings(binaryReader, charsNumber);
                }

                // Working directory
                if (dataFlagsBits.Get(4))
                {
                    // Unneccessary
                    var charsNumber = binaryReader.ReadUInt16();
                    var workingDirectory = readDataStrings(binaryReader, charsNumber);
                }

                // Command line parameters
                if (dataFlagsBits.Get(5))
                {
                    var charsNumber = binaryReader.ReadUInt16();
                    parameters = readDataStrings(binaryReader, charsNumber);
                }
            }

            var fileInfo = GetFileInfo(executableFilePath, parameters, "Startup Menu");

            return fileInfo;
        }

        /// <summary>
        /// Method for reading strings according to 
        /// <para>https://github.com/libyal/liblnk/blob/master/documentation/Windows%20Shortcut%20File%20(LNK)%20format.asciidoc#5-data-strings</para>
        /// </summary>
        /// <param name="binaryReader">Binary reader of windows shortcut file</param>
        /// <returns></returns>
        private static string readDataStrings(BinaryReader binaryReader, UInt16 charsNumber)
        {
            byte[] chars = new byte[charsNumber];

            for (var i = 0; i < charsNumber; i++)
            {
                chars[i] = binaryReader.ReadByte();

                // Offset
                binaryReader.ReadByte();
            }

            var decodedString = Encoding.GetEncoding("Windows-1251").GetString(chars);

            return decodedString;
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
