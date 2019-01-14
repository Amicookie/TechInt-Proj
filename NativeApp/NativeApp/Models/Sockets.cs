using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;
using System.Windows.Controls;


namespace NativeApp.Models
{
	class Sockets
	{
 		public string fileNameLocked;
		public string fileNameUnlocked;
		public string fileNameSaved;

		public string nameToChange = null;
		public  int action = 3; //3 - do nothing

		public bool isConnected;


		public void socketIoEmit(string name, int locked, Socket socket)
		{
			nameToChange = name;
			action = locked;

		socket.On(Socket.EVENT_CONNECT, () =>
			{
				Console.WriteLine("Is Connected ");
				isConnected = true;
			}
			);

			if (nameToChange != null)
				{
					if (action == 1) //dla 1 jest zablokowany
					{
						socket.Emit("fileLocked", nameToChange);
						Console.WriteLine("Locked -> Name: {0}, action: {1}", nameToChange, action);

					}
					else if (action == 0) // 0 - jest odblokowany
					{
						socket.Emit("fileUnlocked", nameToChange);
						Console.WriteLine("Unlocked -> Name: {0}, action: {1}", nameToChange, action);

					}
			}

			nameToChange = null;
			action = 3;

			socket.On(Socket.EVENT_DISCONNECT, () =>
			{
				Console.WriteLine("Rozlaczono");
				isConnected = false;
			}
			);
		}



		public void socketIoManager(Socket socket)
		{

			//socket.On(Socket.EVENT_CONNECT, () =>
			//{
			//	Console.WriteLine("Dziala Eevent");
			//	isConnected = true;
			//}
			//);

			socket.On(Socket.EVENT_DISCONNECT, () =>
			{
				Console.WriteLine("Rozlaczono");
				isConnected = false;
			}
			);

				socket.On("fileLocked", (data) =>
				{
					fileNameLocked = data.ToString();
					Console.WriteLine("SocketIO: zablokowano plik {0}", fileNameLocked);

				});

				socket.On("fileUnlocked", (data) =>
				{
					fileNameUnlocked = data.ToString();
					Console.WriteLine("SocketIO: odblokowano plik {0}", fileNameUnlocked);
				});

				socket.On("fileSaved", (data) =>
				{
					fileNameSaved = data.ToString();
					Console.WriteLine("SocketIO: Zapisano plik {0}", fileNameSaved);
				});
			}



	}
}
