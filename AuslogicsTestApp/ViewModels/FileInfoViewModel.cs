using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AuslogicsTestApp.Models;

namespace AuslogicsTestApp.ViewModels
{
    public class FileInfoViewModel : INotifyPropertyChanged
    {
        #region Fields

        private bool findInRegistry;
        private bool findInStartMenu;
        private FileInfoModel selectedFileInfo;
        private ObservableCollection<FileInfoModel> filesInfo;
        private RelayCommand updateCommand;

        #endregion

        #region Properties

        public bool FindInRegistry
        {
            get
            {
                return findInRegistry;
            }
            set
            {
                findInRegistry = value;
            }
        }

        public bool FindInStartMenu
        {
            get
            {
                return findInStartMenu;
            }
            set
            {
                findInStartMenu = value;
            }
        }

        public FileInfoModel SelectedFileInfo
        {
            get
            {
                return selectedFileInfo;
            }
            set
            {
                selectedFileInfo = value;
            }
        }

        public ObservableCollection<FileInfoModel> FilesInfo
        {
            get
            {
                return filesInfo;
            }
            set
            {
                filesInfo = value;
            }
        }

        public RelayCommand UpdateCommand
        {
            get
            {
                return updateCommand;
            }
        }

        public string Image
        {
            get
            {
                return selectedFileInfo.Image;
            }
            set
            {
                selectedFileInfo.Image = value;
                OnPropertyChanged("Image");
            }
        }

        public string ExecutableFile
        {
            get
            {
                return selectedFileInfo.ExecutableFile;
            }
            set
            {
                selectedFileInfo.ExecutableFile = value;
                OnPropertyChanged("ExecutableFile");
            }
        }

        public string CliParameters
        {
            get
            {
                return selectedFileInfo.CliParameters;
            }
            set
            {
                selectedFileInfo.CliParameters = value;
                OnPropertyChanged("CliParameters");
            }
        }

        public string Path
        {
            get
            {
                return selectedFileInfo.Path;
            }
            set
            {
                selectedFileInfo.Path = value;
                OnPropertyChanged("Path");
            }
        }

        public string AutoStartParameters
        {
            get
            {
                return selectedFileInfo.AutoStartType;
            }
            set
            {
                selectedFileInfo.AutoStartType = value;
                OnPropertyChanged("AutoStartParameters");
            }
        }

        #endregion

        public FileInfoViewModel()
        {
            FilesInfo = new ObservableCollection<FileInfoModel>();

            updateCommand = new RelayCommand(obj => UpdateFilesInfo());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string proptyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(proptyName));
        }

        #region Logic

        private void UpdateFilesInfo()
        {
            FilesInfo.Clear();

            if (findInRegistry)
            {
                var fileInfo = new FileInfoModel()
                {
                    Image = "Image",
                    ExecutableFile = "File.exe",
                    CliParameters = "cls",
                    Path = "path",
                    AutoStartType = "Registry",
                };

                FilesInfo.Add(fileInfo);
            }

            if (findInStartMenu)
            {
                var fileInfo = new FileInfoModel()
                {
                    Image = "Image",
                    ExecutableFile = "File.exe",
                    CliParameters = "cls",
                    Path = "path",
                    AutoStartType = "StartMenu",
                };

                FilesInfo.Add(fileInfo);
            }
        }

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
