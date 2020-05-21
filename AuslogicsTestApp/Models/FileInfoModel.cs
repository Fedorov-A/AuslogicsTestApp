using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuslogicsTestApp.Models
{
    public class FileInfoModel : INotifyPropertyChanged
    {
		private string image;
		private string executableFile;
		private string cliParameters;
		private string path;
		private string autoStartType;

		public string Image
		{
			get 
			{ 
				return image;
			}
			set
			{ 
				image = value;
				OnPropertyChanged("Image");
			}
		}

		public string ExecutableFile
		{
			get 
			{ 
				return executableFile; 
			}
			set 
			{
				executableFile = value;
				OnPropertyChanged("ExecutableFile");
			}
		}

		public string CliParameters
		{
			get
			{
				return cliParameters; 
			}
			set 
			{
				cliParameters = value;
				OnPropertyChanged("CliParameters");
			}
		}
		
		public string Path
		{
			get 
			{
				return path; 
			}
			set 
			{ 
				path = value;
				OnPropertyChanged("Path");
			}
		}
		
		public string AutoStartType
		{
			get 
			{ 
				return autoStartType;
			}
			set
			{ 
				autoStartType = value;
				OnPropertyChanged("AutoStartParameters");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string proptyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(proptyName));
		}
	}
}
