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
		public static string labelText;
		public static string fileNameLocked;
		public static string fileNameUnlocked;
		public static string fileNameSaved;

		public static string nameToChange = null;
		public static int action = 3; //3 - do nothing

		public void socketIoManager()
		{
			var socket = IO.Socket("http://127.0.0.1:5000/ ");

			socket.On(Socket.EVENT_CONNECT, () =>
			 {
				Console.WriteLine("Dziala Eevent");
			 }
			);

			socket.Send("hi");
			socket.Emit("fileSaved", "hello"); //"fileSaved - typ komunikatu, "hello" - wartosc file

			socket.On("message", (data) => //message - def na servie, data - "hi"
			{
				Console.WriteLine("SocketIO: Odebrano message");
				//var value = JsonConvert.DeserializeObject<string>();
				//Console.WriteLine(value);

				labelText = data.ToString();
				//socket.Disconnect();

			});

			socket.On("fileLocked", (data) =>
			{
				fileNameLocked = data.ToString();
				Console.WriteLine("SocketIO: zablokowano plik {0}", fileNameLocked);

			});

			socket.On("fileUnlocked", (data) =>
			{
				fileNameUnlocked = data.ToString();
				Console.WriteLine("SocketIO: zablokowano plik {0}", fileNameUnlocked);
			});

			socket.On("fileSaved", (data) =>
			{
				fileNameSaved = data.ToString();
				Console.WriteLine("SocketIO: Zapisano plik {0}", fileNameSaved);
			});

			
			//if(nameToChange != null)
			//{
			//	if(action == 1) //dla 1 jest zablokowany
			//	{
			//		socket.Emit("fileLocked", nameToChange);
			//		nameToChange = null;
			//		action = 3;
			//	} else if (action == 0) // 0 - jest odblokowany
			//	{
			//		socket.Emit("fileUnlocked", nameToChange);
			//		nameToChange = null;
			//		action = 3;
			//	}
			//}

		}

		//public void socketIoEmit(string name, int locked)
		//{
		//	nameToChange = name;
		//	action = locked;
		//}


	}
}
