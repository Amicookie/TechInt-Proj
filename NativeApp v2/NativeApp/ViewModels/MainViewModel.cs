using NativeApp.Models;
using System.Collections.ObjectModel;

namespace NativeApp.ViewModels
{
    public class MainViewModel
    {
		public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();

		public MainViewModel()
        {
            AppStatus appStatus = new AppStatus();
        }

		internal void AddMessage(string message)
		{
			//Messages.Add(Sockets.receivedFrom + ": " + Sockets.receivedMsg);
		}

    }
}
