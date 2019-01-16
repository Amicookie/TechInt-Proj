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
		string getName, setName,name;

		public void setFileLocked(string fileName)
		{
			getName = fileName+".txt";
			//Console.WriteLine("Name: {0}", name);
		}

		public string returnFileLocekd()
		{
			//Console.WriteLine("wartosc: {0}", setName);
			return getName;

		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{

			if (value.Equals(Sockets.lockf+".txt"))
			{
				Console.WriteLine("dziala warunek: {0}", setName);
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
