using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows.Forms;
using ToggleSwitch;

namespace NativeApp.Models
{
	class Sockets
	{

		public string lockedFile, unlockedFile, savedFile, modifiedFile;
		public string receivedMsg, receivedFrom;

		public static string lockf;

		public string nameToChange = null;
		public  int action = 3; //3 - do nothing

		public void isSocketConnected(Socket socket)
		{
			socket.On(Socket.EVENT_CONNECT, () =>
			{
				Console.WriteLine("Is Connected ");
			}
);
		}

		public void socketIoEmit(string name, int locked, String user, Socket socket, int userId, int fileId)
		{
			nameToChange = name;
			action = locked;

			sUser nowy = new sUser {
				user_id = userId,
				username = user,
				file_name = name,
				file_id = fileId
			};


			if (nameToChange != null)
			{
				if (action == 1) //dla 1 jest zablokowany
				{
					socket.Emit("fileLocked", JsonConvert.SerializeObject(nowy));
					//Console.WriteLine(nowy);
					Console.WriteLine("Locked -> Name: {0}, ID_file: {1}, user: {2}, ID_user: {3}", nameToChange, fileId, user, userId);

				}
				else if (action == 0) // 0 - jest odblokowany
				{
					socket.Emit("fileUnlocked", JsonConvert.SerializeObject(nowy));
					Console.WriteLine("Unlocked -> Name: {0}, ID_file: {1}, user: {2}, ID_user: {3}", nameToChange, fileId, user, userId);

				}
				//else if(action == 2)
				//{
				//	socket.Emit("fileSaved", JsonConvert.SerializeObject(nowy));
				//	Console.WriteLine("Saved -> Name: {0}, ID_file: {1}, user: {2}, ID_user: {3}", nameToChange, fileId, user, userId);
				//}
			}

			nameToChange = null;
			action = 3;

			socket.On(Socket.EVENT_DISCONNECT, () =>
			{
				Console.WriteLine("Rozlaczono");
			}
			);
		}



		public void socketIoManager(Socket socket, String user)
		{
			sUser nowyUser = new sUser();

			socket.On(Socket.EVENT_DISCONNECT, () =>
			{
				Console.WriteLine("Rozlaczono");
			}
			);


			socket.On("fileLocked", (data) =>
			{
				var locked = JsonConvert.DeserializeObject<sUser>(data.ToString());
				nowyUser.username = locked.username;
				nowyUser.file_name = locked.file_name;

				if (!user.Equals(nowyUser.username))
				{
					Console.WriteLine("SocketIO: zablokowano plik {0}, {1}", nowyUser.username, nowyUser.file_name);
					MessageBox.Show("Zablokowano plik " + nowyUser.file_name + " przez " + nowyUser.username);

					lockedFile = nowyUser.file_name;
					lockf = nowyUser.file_name;
				}
				
			});

			socket.On("fileUnlocked", (data) =>
			{
				var unlocked = JsonConvert.DeserializeObject<sUser>(data.ToString());
				nowyUser.username = unlocked.username;
				nowyUser.file_name = unlocked.file_name;

				if (!user.Equals(nowyUser.username))
				{ 
					Console.WriteLine("SocketIO: odblokowano plik {0}, {1}", nowyUser.username, nowyUser.file_name);
					MessageBox.Show("Odblokowano plik " + nowyUser.file_name + " przez " + nowyUser.username);

					unlockedFile = nowyUser.file_name;
					if(nowyUser.file_name.Equals(lockf))
					{
						lockf = null;
					}
				}
				
			});

			socket.On("fileSaved", (data) =>
			{
				var saved = JsonConvert.DeserializeObject<sUser>(data.ToString());
				nowyUser.username = saved.username;
				nowyUser.file_name = saved.file_name;

				if (!user.Equals(nowyUser.username))
				{
					MessageBox.Show("Zapisano nowy plik " + nowyUser.file_name + "\nOdśwież stroę.");

					savedFile = nowyUser.file_name;
				}
				
			});

			socket.On("fileUpdated", (data) =>
			{
				var saved = JsonConvert.DeserializeObject<sUser>(data.ToString());
				nowyUser.username = saved.username;
				nowyUser.file_name = saved.file_name;

				if (!user.Equals(nowyUser.username))
				{
					MessageBox.Show("Zmodyfikowano plik " + nowyUser.file_name + "\nOdśwież stroę.");

					modifiedFile = nowyUser.file_name;
				}

			});

		}

		public string returnLockedFile()
		{
			return lockedFile;
		}

		public void returnUnlockedFile()
		{
			unlockedFile = null;
		}

		public void returnSavedFile()
		{
			savedFile = null;
		}

		// CHAT SOCKETY
		public void chatGetMsg(Socket socket, String user)
		{
			sChat chat = new sChat();

			socket.On("chat", (data) =>
			{
				var sent = JsonConvert.DeserializeObject<sChat>(data.ToString());
				chat.username = sent.username;
				chat.message = sent.message;

				if (!user.Equals(chat.username))
				{
					Console.WriteLine("{0}: {1}", chat.username, chat.message);
					receivedMsg = chat.message;
					receivedFrom = chat.username;
				}
			});

		}

		public void sendMsg(Socket socket, String _user, string _msg)
		{
			sChat nowyChat = new sChat
			{
				username = _user,
				message = _msg
			};

			socket.Emit("chat", JsonConvert.SerializeObject(nowyChat));
			Console.WriteLine("Wyslano {0} : {1}", nowyChat.username, nowyChat.message);
		}

	}


}
