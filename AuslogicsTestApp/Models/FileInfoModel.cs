using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AuslogicsTestApp.Models
{
	/// <summary>
	/// A Model for file information
	/// </summary>
    public class FileInfoModel : INotifyPropertyChanged
    {
		public FileInfoModel()
		{
				
		}


		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion


		#region Fields

		private BitmapSource fileIcon;
		private FileInfo info;
		private string parameters;
		private string autoStartType;

		#endregion


		#region Properties

		/// <summary>
		/// File icon
		/// </summary>
		public BitmapSource FileIcon
		{
			get 
			{ 
				return fileIcon;
			}
			set
			{ 
				fileIcon = value;
				OnPropertyChanged("Image");
			}
		}

		/// <summary>
		/// File info
		/// </summary>
		public FileInfo Info
		{
			get
			{
				return info;
			}
			set
			{
				info = value;
				OnPropertyChanged("ExecutableFile");
			}
		}

		/// <summary>
		/// File name with extention
		/// </summary>
		public string ExecutableFile
		{
			get 
			{ 
				return info.Name; 
			}
		}

		/// <summary>
		/// Command line commands for starting file
		/// </summary>
		public string Parameters
		{
			get
			{
				return parameters; 
			}
			set 
			{
				parameters = value;
				OnPropertyChanged("CliParameters");
			}
		}
		
		/// <summary>
		/// Directory with file
		/// </summary>
		public string Path
		{
			get 
			{
				return info.DirectoryName; 
			}
		}
		
		/// <summary>
		/// Autostart type (Registry or Startup Menu)
		/// </summary>
		public string AutostartType
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

		#endregion


		#region Handlers

		private void OnPropertyChanged(string proptyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(proptyName));
		}
		
		#endregion
	}
}
