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

		public void socketIoManager()
		{
			var socket = IO.Socket("http://127.0.0.1:5000/ ");

			socket.On(Socket.EVENT_CONNECT, () =>
			 {
				//socket.Emit("hi");
				Console.WriteLine("Dziala Eevent");
			 }
			);

			//socket.On("hi", (data) =>
			//{
			//	Console.WriteLine("Dziala socket hi");
			//	Console.WriteLine(data);
			//	socket.Disconnect();
			//});

			socket.Send("message");
			socket.Send("Wysylam wiadomosc hello there");

			socket.On("message", (data) =>
			{
				//socket.Emit("Here is my message");
				Console.WriteLine("Dziala socket message");
				//var value = JsonConvert.DeserializeObject<string>((string)data);
				//Console.WriteLine(value);
				Console.WriteLine(data);

				labelText = data.ToString();
				socket.Disconnect();

			});

		}


	}
}
