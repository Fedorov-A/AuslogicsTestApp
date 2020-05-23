using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace AuslogicsTestApp.Models
{
    /// <summary>
    /// A Model for list of infomation of files
    /// </summary>
    class FilesInfoModel : INotifyPropertyChanged
    {
        public FilesInfoModel()
        {
            FilesInfo = new List<FileInfoModel>();

            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        #region Constansts

        private string HKLM_RUN = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private string HKLM_RUN_ONCE = @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce";
        private string HKLM_32_RUN = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run";
        private string HKLM_32_RUN_ONCE = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\RunOnce";
        private string HKCU_RUN = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private string HKCU_RUN_ONCE = @"Software\Microsoft\Windows\CurrentVersion\RunOnce";

        #endregion


        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region Fields

        private bool readingInProgress;
        private bool readInRegistry;
        private bool readInStartupMenu;
        private int readingProgress;
        private List<FileInfoModel> filesInfo;
        private List<FileInfoModel> newFilesInfo;
        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        #endregion


        #region Properties

        /// <summary>
        /// List of <see cref="FileInfoModel"/> 
        /// </summary>
        public List<FileInfoModel> FilesInfo
		{
			get => filesInfo;
			private set
			{
				filesInfo = value;
				OnPropertyChanged("FilesInfo");
			}
		}
        
        /// <summary>
        /// Determines reading from egistry keys
        /// </summary>
        public bool ReadInRegistry 
        { 
            get => this.readInRegistry;
            set
            {
                readInRegistry = value;
                OnPropertyChanged("ReadInRegistry");
            }
        }
        
        /// <summary>
        /// Determines reading from startup menu folders
        /// </summary>
        public bool ReadInStartupMenu 
        { 
            get => this.readInStartupMenu;
            set
            {
                readInStartupMenu = value;
                OnPropertyChanged("ReadInStartupMenu");
            }
        }

        /// <summary>
        /// Shows state of reading process 
        /// </summary>
        public bool ReadingInProgress
        {
            get => readingInProgress;
            private set
            {
                readingInProgress = value;
                OnPropertyChanged("ReadingInProgress");
            }
        }

        /// <summary>
        /// Shows reading progress
        /// </summary>
        public int ReadingProgress 
        { 
            get => this.readingProgress;
            private set
            {
                readingProgress = value;
                OnPropertyChanged("ReadingProgress");
            }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Updates model according <see cref="ReadInRegistry"/> and <see cref="ReadInStartupMenu"/> flags
        /// </summary>
        public void UpdateModel()
        {
            backgroundWorker.RunWorkerAsync();
        }
        
        /// <summary>
        /// Reads and returns list of <see cref="FileInfoModel"/> from registry keys and startup folders
        /// </summary>
        /// <returns></returns>
        private List<FileInfoModel> GetFilesInfo()
        {
            var localFilesInfo = new List<FileInfoModel>();

            // We don't know how many files in these directories exactly, so progress value will be based on directory number
            Func<int, int> calcProgress = arg => 100 * arg / ((readInRegistry ? 6 : 0) + (readInStartupMenu ? 2 : 0));

            var i = 1;

            if (readInRegistry)
            {
                // Registry keys for Local Machine
                localFilesInfo.AddRange(Services.GetFilesInfoFromRegistry(Registry.LocalMachine.OpenSubKey(HKLM_RUN)));
                backgroundWorker.ReportProgress(calcProgress(i++));

                localFilesInfo.AddRange(Services.GetFilesInfoFromRegistry(Registry.LocalMachine.OpenSubKey(HKLM_RUN_ONCE)));
                backgroundWorker.ReportProgress(calcProgress(i++));

                localFilesInfo.AddRange(Services.GetFilesInfoFromRegistry(Registry.LocalMachine.OpenSubKey(HKLM_32_RUN)));
                backgroundWorker.ReportProgress(calcProgress(i++));

                localFilesInfo.AddRange(Services.GetFilesInfoFromRegistry(Registry.LocalMachine.OpenSubKey(HKLM_32_RUN_ONCE)));
                backgroundWorker.ReportProgress(calcProgress(i++));

                // Registry keys for Current User
                localFilesInfo.AddRange(Services.GetFilesInfoFromRegistry(Registry.CurrentUser.OpenSubKey(HKCU_RUN)));
                backgroundWorker.ReportProgress(calcProgress(i++));

                localFilesInfo.AddRange(Services.GetFilesInfoFromRegistry(Registry.CurrentUser.OpenSubKey(HKCU_RUN_ONCE)));
                backgroundWorker.ReportProgress(calcProgress(i++));
            }

            if (readInStartupMenu)
            {
                // Startup Folder for Local Machine
                var systemStartupDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
                localFilesInfo.AddRange(Services.GetFilesInfoFromStartupMenu(systemStartupDirectory));
                backgroundWorker.ReportProgress(calcProgress(i++));

                // Startup Folder for Current User
                var userStartupDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                localFilesInfo.AddRange(Services.GetFilesInfoFromStartupMenu(userStartupDirectory));
                backgroundWorker.ReportProgress(calcProgress(i++));
            }

            return localFilesInfo;
        }

        #endregion


        #region Event Handlers

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ReadingInProgress = true;
            newFilesInfo = GetFilesInfo();
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ReadingProgress = e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FilesInfo = newFilesInfo;
            ReadingInProgress = false;
        }

        private void OnPropertyChanged(string proptyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(proptyName));
		
		#endregion
	}
}
