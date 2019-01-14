using NativeApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NativeApp
{
	class CheckTheFileConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Sockets newSocket = new Sockets();
			string lockedFile = newSocket.fileNameLocked + ".txt";

			if(value.Equals(lockedFile))
			{
				return true;
			} else
			{
				return false;
			}

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
