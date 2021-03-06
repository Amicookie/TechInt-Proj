﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows.Forms;
using ToggleSwitch;
using NativeApp.ViewModels;
using System.Drawing;
using System.Reflection;

namespace NativeApp.Models
{
	class Sockets
	{

		public string lockedFile, unlockedFile, savedFile, modifiedFile, modifiedBy;
		public string receivedMsg, receivedFrom;

		public static string lockf, unlockf, userLogged;

		public string nameToChange = null;
		public  int action = 4; //4 - do nothing

		private NotifyIcon _notifyIcon;

		public void showIcon()
		{
			_notifyIcon = new NotifyIcon();
			_notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
			_notifyIcon.BalloonTipClosed += (s, e) => _notifyIcon.Visible = false;
		}

		public void isSocketConnected(Socket socket, int _user)
		{
            sUserType uType = new sUserType
            {
                connectionType = "desktop",
                user_id = _user
            };

            socket.Emit("connectionType", JsonConvert.SerializeObject(uType));
            Console.WriteLine("Socket connect");
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

			sUser drugi = new sUser
			{
				username = user,
				file_name = name
			};


			if (nameToChange != null)
			{
				if (action == 1) // 1 - jest zablokowany
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
				else if (action == 2) // 2 - jest nowy zapisany
				{
					socket.Emit("fileSaved", JsonConvert.SerializeObject(drugi));
					Console.WriteLine("Saved -> Name: {0}, ID_file: {1}, user: {2}, ID_user: {3}", nameToChange, fileId, user, userId);
				}
				else if(action == 3) // 3- jest zmodyfikowany
				{
					socket.Emit("fileUpdated", JsonConvert.SerializeObject(nowy));
					Console.WriteLine("Updated -> Name: {0}, ID_file: {1}, user: {2}, ID_user: {3}", nameToChange, fileId, user, userId);
				}
			}

			nameToChange = null;
			action = 4;

			socket.On(Socket.EVENT_DISCONNECT, () =>
			{
				Console.WriteLine("Rozlaczono");
			}
			);
		}



		public void socketIoManager(Socket socket, String user)
		{
			userLogged = user;
			sUser nowyUser = new sUser();
			showIcon();

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
					//MessageBox.Show("File " + nowyUser.file_name + " has been locked by " + nowyUser.username);

					lockedFile = nowyUser.file_name;
					lockf = nowyUser.file_name;

					
					Login.main.Toggle = false;

					_notifyIcon.Visible = true;
					_notifyIcon.ShowBalloonTip(3000, "File has been locked", nowyUser.file_name + ", locked by " + nowyUser.username, ToolTipIcon.Info);

					if (lockedFile.Equals(unlockedFile)) {
						returnUnlockedFile();
					}
				}
				
			});

			socket.On("fileUpdated", (data) =>
			{
				var saved = JsonConvert.DeserializeObject<sUser>(data.ToString());
				nowyUser.username = saved.username;
				nowyUser.file_name = saved.file_name;

				if (!user.Equals(nowyUser.username))
				{
					//MessageBox.Show("File " + nowyUser.file_name + " has been modified.\nPlease, refresh.");

					modifiedFile = nowyUser.file_name;
					modifiedBy = nowyUser.username;

					Login.main.refreshWindow = "";

					_notifyIcon.Visible = true;
					_notifyIcon.ShowBalloonTip(3000, "A file has been modified", nowyUser.file_name + ", modified by " + nowyUser.username, ToolTipIcon.Info);

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
					//MessageBox.Show("File " + nowyUser.file_name + " has been unlocked " + nowyUser.username);

					unlockedFile = nowyUser.file_name;
					unlockf = nowyUser.file_name;

					_notifyIcon.Visible = true;
					_notifyIcon.ShowBalloonTip(3000, "File has been unlocked", nowyUser.file_name + ", unlocked by " + nowyUser.username, ToolTipIcon.Info);

					if (nowyUser.file_name.Equals(lockf))
					{
						lockf = null;
					}

					Login.main.Toggle = true;

					if (unlockedFile.Equals(lockedFile))
					{
						returnLockedFile();
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
					//MessageBox.Show("File " + nowyUser.file_name + " hase been saved.\nPlease, refresh.");

					savedFile = nowyUser.file_name;

					Login.main.CleanFiles = true;
					Login.main.UploadFiles = true;

					_notifyIcon.Visible = true;
					_notifyIcon.ShowBalloonTip(3000, "New file has been saved", nowyUser.file_name, ToolTipIcon.Info);
				}
				
			});



		}

		public void returnLockedFile()
		{
			lockedFile = null;
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

					Login.main.Status = receivedFrom + ": " + receivedMsg + "\n";
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
