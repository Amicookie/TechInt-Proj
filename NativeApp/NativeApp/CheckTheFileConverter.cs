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
			try
			{
				foreach (sUser s in Login.lockedFilesList)
				{
					if (value.Equals(s.file_name + ".txt") && !s.file_name.Equals(Sockets.unlockf))
					{
						return true;
					}
					else if (value.Equals(Sockets.lockf + ".txt"))
					{
						return true;
					} else
					{
						return false;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			if (value.Equals(Sockets.lockf+".txt"))
			{
				//Console.WriteLine("dziala warunek: {0}", Sockets.lockf);
				return true;
			}
			else
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
