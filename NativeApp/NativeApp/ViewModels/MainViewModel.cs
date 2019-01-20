namespace NativeApp.ViewModels
{
	using System.Collections.ObjectModel;
	using Prism.Events;
	using Models;
	using Events;
	using Prism.Commands;
	using Quobject.SocketIoClientDotNet.Client;
	using System.ComponentModel;

	public class MainViewModel //:ViewModelBase
	{
		//private readonly EventAggregator eventAggregator;
		//private readonly Sockets sockets;
		//private ObservableCollection<sChat> messages;
		//public Socket socket;
		//public string userName;

		public MainViewModel()
		{
			//var appStatus = new AppStatus();
			//sockets = new Sockets();
			//SendMessageCommand = new DelegateCommand<string>(SendMessage, value => true);
			//eventAggregator = new EventAggregator();
			//eventAggregator.GetEvent<MessageEvent>().Subscribe(OnMessageReceived);


		}


		//public ObservableCollection<sChat> Messages
		//{
		//	get => messages;
		//	set
		//	{
		//		if (value == messages)
		//		{
		//			return;
		//		}

		//		messages = value;
		//		OnPropertyChanged();
		//	}
		//}

		//public DelegateCommand<string> SendMessageCommand
		//{
		//	get;
		//	set;
		//}

		//private void OnMessageReceived(sChat message)
		//{
		//	if (message != null)
		//	{
		//		Messages.Add(message);
		//	}
		//}

		//private void SendMessage(string message)
		//{
		//	sockets.sendMsg(socket, userName, message);
		//}
	}
}