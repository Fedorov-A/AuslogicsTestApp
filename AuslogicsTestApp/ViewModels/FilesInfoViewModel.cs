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
using System.Threading;

using AuslogicsTestApp.Models;

namespace AuslogicsTestApp.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="FilesInfoModel"/>
    /// </summary>
    public class FilesInfoViewModel : INotifyPropertyChanged
    {
        public FilesInfoViewModel()
        {
            filesInfoModel = new FilesInfoModel();
            filesInfoModel.PropertyChanged += (sender, e) => { OnPropertyChanged(e.PropertyName); };
            updateCommand = new RelayCommand(obj => UpdateFilesInfo());
        }


        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
                
        #endregion


        #region Fields
        
        private RelayCommand updateCommand;
        private FilesInfoModel filesInfoModel;

        #endregion


        #region Properties

        /// <summary>
        /// Determines reading from egistry keys
        /// </summary>
        public bool ReadInRegistry
        {
            get => filesInfoModel.ReadInRegistry;
            set
            {
                filesInfoModel.ReadInRegistry = value;
                OnPropertyChanged("ReadInRegistry");
            }
        }

        /// <summary>
        /// Determines reading from startup menu folders
        /// </summary>
        public bool ReadInStartupMenu
        {
            get => filesInfoModel.ReadInStartupMenu;
            set
            {
                filesInfoModel.ReadInStartupMenu = value;
                OnPropertyChanged("ReadInStartupMenu");
            }
        }

        /// <summary>
        /// DockPanel is not enabled when reading in progress (to avoid pressing button)
        /// </summary>
        public bool ReadingInProgress
        {
            get => !filesInfoModel.ReadingInProgress;
        }
                
        /// <summary>
        /// Represents reading progress in percents
        /// </summary>
        public int ReadingProgress
        {
            get => filesInfoModel.ReadingProgress;
        }

        /// <summary>
        /// List of <see cref="FileInfoModel"/> that binded to View
        /// </summary>
        public List<FileInfoModel> FilesInfo
        {
            get => filesInfoModel.FilesInfo;
        }

        /// <summary>
        /// Command that invokes <see cref="UpdateFilesInfo"/>
        /// </summary>
        public RelayCommand UpdateCommand
        {
            get => updateCommand;
        }
        
        #endregion


        #region Methods

        /// <summary>
        /// Invokes <see cref="FilesInfoModel"/> updating method
        /// </summary>
        private void UpdateFilesInfo()
        {
            try
            {
                filesInfoModel.UpdateModel();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error", $"{e.Message}\n{e.StackTrace}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion


        #region Event Handlers

        private void OnPropertyChanged(string proptyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(proptyName));

        #endregion
    }

    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
