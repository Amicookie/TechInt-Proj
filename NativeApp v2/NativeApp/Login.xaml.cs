using NativeApp.Models;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace NativeApp
{
	/// <summary>
	/// Logika interakcji dla klasy Login.xaml
	/// </summary>
	public partial class Login : Window
	{
		private User user;
		public Socket socket;

		public static DataGrid chatGrid;
		private String login, password;

		String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Files";
		AppStatus appStatus = new AppStatus();

		Sockets newSocket = new Sockets();
		sUser suser = new sUser();

		public Login()
		{
			InitializeComponent();

			if (Directory.Exists(path))
			{
				listOfFiles.ItemsSource = new DirectoryInfo(path).GetFiles();
			}
			else
			{
				Directory.CreateDirectory(path);
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			appStatus.isServerOnline = AppStatus.CheckForServerConnection();
			appStatus.isOnline = AppStatus.CheckForInternetConnection();

			mainGrid.Visibility = Visibility.Hidden;
			filesListGrid.Visibility = Visibility.Hidden;
			newFileGrid.Visibility = Visibility.Hidden;

			if (appStatus.isServerOnline == true && appStatus.isOnline == true)
			{
				loginBtn.Visibility = Visibility.Visible;
				passBox.IsEnabled = true;
				loginBox.IsEnabled = true;
				workOffBtn.Visibility = Visibility.Hidden;
			}
			else
			{
				loginBtn.Visibility = Visibility.Hidden;
				passBox.IsEnabled = false;
				loginBox.IsEnabled = false;
				workOffBtn.Visibility = Visibility.Visible;
			}

			LoginGrid.Visibility = Visibility.Visible;
			welcomeGrid.Visibility = Visibility.Hidden;
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			LoginGrid.Visibility = Visibility.Hidden;
			mainGrid.Visibility = Visibility.Visible;
			welcomeGrid.Visibility = Visibility.Hidden;
		}

		private void loginBtn_Click(object sender, RoutedEventArgs e)
		{
			appStatus.isServerOnline = AppStatus.CheckForServerConnection();
			appStatus.isOnline = AppStatus.CheckForInternetConnection();
			filesListGrid.Visibility = Visibility.Hidden;

			login = loginBox.Text;
			password = passBox.Password.ToString();
			if (login.Count() > 0 && password.Count() > 0)
			{
				user = new User(login, password);
				user.Get2();

				if (user.user_exists == true && user.logged_in)
				{
					appStatus.isUserLogged = true;
					MenuGrid.Visibility = Visibility.Hidden;
					loginMenuGrid.Visibility = Visibility.Visible;
					LoginGrid.Visibility = Visibility.Hidden;
					welcomeGrid.Visibility = Visibility.Visible;
					welcomeLabel.Content = $"Logged in as \n{user.user_login}";

					Documents documents = new Documents();
					documents.Get2(true);
					listOfFiles.ItemsSource = new DirectoryInfo(path).GetFiles();

					loginBttn.Visibility = Visibility.Hidden;
					logoutBtn.Visibility = Visibility.Visible;

					socket = IO.Socket(adresIP.adres);

					newSocket.isSocketConnected(socket);
					newSocket.socketIoManager(socket, login);
					newSocket.chatGetMsg(socket, login);
				}
				else
				{
					appStatus.isUserLogged = false;
					MessageBox.Show("Wrong Credentials");
				}

				loginBox.Clear();
				passBox.Clear();
			}
		}

		private void workOffBtn_Click(object sender, RoutedEventArgs e)
		{
			appStatus.isUserLogged = false;
			filesListGrid.Visibility = Visibility.Visible;
			LoginGrid.Visibility = Visibility.Hidden;
			newFileGrid.Visibility = Visibility.Hidden;
			welcomeGrid.Visibility = Visibility.Hidden;
			logoutBtn.Visibility = Visibility.Hidden;
			loginBttn.Visibility = Visibility.Visible;

			listOfFiles.ItemsSource = null;
			listOfFiles.ItemsSource = new DirectoryInfo(path).GetFiles();

			loginMenuGrid.Visibility = Visibility.Visible;

		}

		private void filesBtn_Click(object sender, RoutedEventArgs e)
		{
			Documents documents = new Documents();
			if (appStatus.isServerOnline == true && appStatus.isOnline == true && appStatus.isUserLogged)
			{
				documents.Get2(true);

			}
			else
			{
				listOfFiles.ItemsSource = null;
				listOfFiles.ItemsSource = new DirectoryInfo(path).GetFiles();

				filesListGrid.Visibility = Visibility.Visible;
				newFileGrid.Visibility = Visibility.Hidden;
				welcomeGrid.Visibility = Visibility.Hidden;
				LoginGrid.Visibility = Visibility.Hidden;

			}

			if (Documents.currentDocuments != null)
			{
				listOfFiles.ItemsSource = null;
				listOfFiles.ItemsSource = new DirectoryInfo(path).GetFiles();

				filesListGrid.Visibility = Visibility.Visible;
				newFileGrid.Visibility = Visibility.Hidden;
				welcomeGrid.Visibility = Visibility.Hidden;
				LoginGrid.Visibility = Visibility.Hidden;

			}

			toggleSwitch.IsChecked = false;
			saveBtn.IsEnabled = false;
			TitleBox.IsReadOnly = true;
			contentBox.IsReadOnly = true;
		}

		private void newFileBtn_Click(object sender, RoutedEventArgs e)
		{
			filesListGrid.Visibility = Visibility.Hidden;
			newFileGrid.Visibility = Visibility.Visible;
			welcomeGrid.Visibility = Visibility.Hidden;

			TitleBox.Clear();
			contentBox.Clear();

			toggleSwitch.IsChecked = true;
			toggleSwitch.IsEnabled = true;
			TitleBox.IsReadOnly = false;
			contentBox.IsReadOnly = false;
			saveBtn.IsEnabled = true;
		}


		private void logoutBtn_Click(object sender, RoutedEventArgs e)
		{
			MenuGrid.Visibility = Visibility.Visible;
			loginMenuGrid.Visibility = Visibility.Hidden;
			mainGrid.Visibility = Visibility.Visible;
			filesListGrid.Visibility = Visibility.Hidden;
			newFileGrid.Visibility = Visibility.Hidden;
			welcomeGrid.Visibility = Visibility.Hidden;
			ChatGrid.Visibility = Visibility.Hidden;
			chatBorder.Visibility = Visibility.Hidden;

			socket.Disconnect();

			toggleSwitch.IsChecked = false;
			saveBtn.IsEnabled = false;
			TitleBox.IsReadOnly = true;
			contentBox.IsReadOnly = true;

		}

		private void Row_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			//toggleSwitch.IsChecked = false;
			//saveBtn.IsEnabled = false;
			//TitleBox.IsReadOnly = true;
			//contentBox.IsReadOnly = true;

			String title = listOfFiles.SelectedItems[0].ToString();
			string text = System.IO.File.ReadAllText(System.IO.Path.Combine(path, title));

			filesListGrid.Visibility = Visibility.Hidden;
			newFileGrid.Visibility = Visibility.Visible;

			TitleBox.Text = title.Substring(0, title.Length - 4);
			contentBox.Text = text;

			if(TitleBox.Text.Equals(newSocket.lockedFile))
			{
				toggleSwitch.IsChecked = false;
				toggleSwitch.IsEnabled = false;
			} else
			{
				toggleSwitch.IsEnabled = true;
			}

			if(TitleBox.Text.Equals(newSocket.unlockedFile))
			{
				toggleSwitch.IsEnabled = true;
			}

			if(toggleSwitch.IsChecked == true)
			{
				TitleBox.IsReadOnly = false;
				contentBox.IsReadOnly = false;
				saveBtn.IsEnabled = true;
			}
			else if(toggleSwitch.IsChecked == false)
			{
				TitleBox.IsReadOnly = true;
				contentBox.IsReadOnly = true;
				saveBtn.IsEnabled = false;
			}
		}


		private void backBtn_Click(object sender, RoutedEventArgs e)
		{
			Documents documents = new Documents();
			if (appStatus.isServerOnline == true && appStatus.isOnline == true && appStatus.isUserLogged)
			{
				documents.Get2(true);
				if(toggleSwitch.IsChecked)
				{
					toggleSwitch.IsChecked = false;
					saveBtn.IsEnabled = false;
					TitleBox.IsReadOnly = true;
					contentBox.IsReadOnly = true;
					//newSocket.socketIoEmit(TitleBox.Text, 0, login, socket);
				}
				
			}
			else
			{
				listOfFiles.ItemsSource = null;
				listOfFiles.ItemsSource = new DirectoryInfo(path).GetFiles();

				filesListGrid.Visibility = Visibility.Visible;
				newFileGrid.Visibility = Visibility.Hidden;
				welcomeGrid.Visibility = Visibility.Hidden;
				LoginGrid.Visibility = Visibility.Hidden;

			}

			if (Documents.currentDocuments != null)
			{
				listOfFiles.ItemsSource = null;
				listOfFiles.ItemsSource = new DirectoryInfo(path).GetFiles();

				filesListGrid.Visibility = Visibility.Visible;
				newFileGrid.Visibility = Visibility.Hidden;
				welcomeGrid.Visibility = Visibility.Hidden;
				LoginGrid.Visibility = Visibility.Hidden;
			}
		}

		private void saveBtn_Click(object sender, RoutedEventArgs e)
		{
			appStatus.isServerOnline = AppStatus.CheckForServerConnection();
			appStatus.isOnline = AppStatus.CheckForInternetConnection();
			Documents documents = new Documents();
			documents.Get2(true);

			String title = TitleBox.Text;
			String content = contentBox.Text;
			string filenameNoExtension = title;
			string filename = String.Format("{0}.txt", title);
			string allPath = System.IO.Path.Combine(path, filename);

			if (!File.Exists(allPath))
			{
				if (appStatus.isServerOnline == true && appStatus.isOnline == true && appStatus.isUserLogged == true)
				{
					Document document = new Document(filenameNoExtension, content, DateTime.Now, DateTime.Now, 1, 0);
					if (document.checkIfNoDuplicated())
					{
						document.PostDocument(document);
						MessageBox.Show("File has been saved");
					}
					else
					{
						MessageBox.Show("File name exsist!");
					}
				}
				else if (appStatus.isServerOnline == true && appStatus.isOnline == true &&
						 appStatus.isUserLogged == false)
				{
					using (StreamWriter str = File.CreateText(allPath))
					{
						str.WriteLine(content);
						str.Flush();
						MessageBox.Show("Login to sync document!");
						Button_Click(sender, e);
					}
				}
				else if ((appStatus.isServerOnline == false || appStatus.isOnline == false) &&
						 appStatus.isUserLogged == true)
				{
					appStatus.isUserLogged = false;
					using (StreamWriter str = File.CreateText(allPath))
					{
						str.WriteLine(content);
						str.Flush();
						MessageBox.Show("You`ve lost connection! Document will be saved locally");
						//Button_Click(sender, e);
					}
				}
				else
				{
					using (StreamWriter str = File.CreateText(allPath))
					{
						str.WriteLine(content);
						str.Flush();
						MessageBox.Show("File has been saved");
					}
				}
			}
			else if (File.Exists(allPath))
			{
				if (appStatus.isServerOnline == true && appStatus.isOnline == true && appStatus.isUserLogged == true)
				{
					Document exsistingDoc = Documents.currentDocuments
						.Where(a => a.file_name == Path.GetFileNameWithoutExtension(allPath))
						.FirstOrDefault();
					exsistingDoc.file_content = content;
					//temporary hardcode ;c
					exsistingDoc.user_id = 1;
					var localWriteTime =
						File.GetLastWriteTime(Path.Combine(Documents.path, exsistingDoc.file_name + ".txt"));
					if (localWriteTime < exsistingDoc.file_update_date)
					{
						using (StreamWriter str =
							File.CreateText(Path.Combine(Documents.path, exsistingDoc.file_name + "_local.txt")))
						{
							str.WriteLine(content);
							str.Flush();
							MessageBox.Show($"{exsistingDoc.file_name}_local file has been created locally");
						}
					}
					else
					{
						exsistingDoc.CallUpdateDoc();
						MessageBox.Show("File has been updated!");
					}
				}
				else if (appStatus.isServerOnline == true && appStatus.isOnline == true && appStatus.isUserLogged == false)
				{
					using (var str = new StreamWriter(allPath))
					{
						str.WriteLine(content);
						str.Flush();
						MessageBox.Show("Login to sync document!");
						Button_Click(sender, e);

					}
				}
				else if ((appStatus.isServerOnline == false || appStatus.isOnline == false) &&
						 appStatus.isUserLogged == true)
				{
					appStatus.isUserLogged = false;
					using (var str = new StreamWriter(allPath))
					{
						str.WriteLine(content);
						str.Flush();
						MessageBox.Show("You`ve lost connection! Document will be saved locally");
						Button_Click(sender, e);

					}
				}
				else
				{
					using (var str = new StreamWriter(allPath))
					{
						str.WriteLine(content);
						str.Flush();

						MessageBox.Show("File has been overwritten");
					}
				}
			}

			try
			{
				documents = new Documents();
				documents.Get2(true);

			}
			catch (Exception es)
			{
				Console.WriteLine(es);
			}

			listOfFiles.ItemsSource = null;
			listOfFiles.ItemsSource = new DirectoryInfo(path).GetFiles();

			TitleBox.IsReadOnly = true;
			contentBox.IsReadOnly = true;
			toggleSwitch.IsChecked = false;
			saveBtn.IsEnabled = false;
		}

		private void toggleSwitch_Checked(object sender, RoutedEventArgs e)
		{
			if (toggleSwitch.IsEnabled == true)
			{
				TitleBox.IsReadOnly = false;
				contentBox.IsReadOnly = false;
				saveBtn.IsEnabled = true;

				if (appStatus.isServerOnline == true && appStatus.isOnline == true && appStatus.isUserLogged)
				{
					if (!string.IsNullOrWhiteSpace(TitleBox.Text))
					{
						Console.WriteLine("dziala");
						newSocket.socketIoEmit(TitleBox.Text, 1, login, socket);
					}
				}
			}
		}

		private void toggleSwitch_Unchecked(object sender, RoutedEventArgs e)
		{
			if (toggleSwitch.IsEnabled == true)
			{
				TitleBox.IsReadOnly = true;
				contentBox.IsReadOnly = true;
				saveBtn.IsEnabled = false;

				if (appStatus.isServerOnline == true && appStatus.isOnline == true && appStatus.isUserLogged)
				{
					if (!string.IsNullOrWhiteSpace(TitleBox.Text))
					{
						newSocket.socketIoEmit(TitleBox.Text, 0, login, socket);
					}
				}
			}
		}

		private void chatBtn_Click(object sender, RoutedEventArgs e)
		{
			if (appStatus.isServerOnline == true && appStatus.isOnline == true && appStatus.isUserLogged)
			{
				ChatGrid.Visibility = Visibility.Visible;
				chatBorder.Visibility = Visibility.Visible;
				if (newSocket.receivedFrom != null)
				{
					//chatWindowBox.AppendText(newSocket.receivedFrom + ": " + newSocket.receivedMsg + "\n");
				}
			} else
			{
				MessageBox.Show("You don't have connection to use chat!");
			}
			
		}

		private void closeChatBtn_Click(object sender, RoutedEventArgs e)
		{

			ChatGrid.Visibility = Visibility.Hidden;
			chatBorder.Visibility = Visibility.Hidden;
			
		}

		private void chatDatagrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{
			

		}

		private void sendBtn_Click(object sender, RoutedEventArgs e)
		{
			if (appStatus.isServerOnline == true && appStatus.isOnline == true && appStatus.isUserLogged)
			{
				string sentMsg = messageBox.Text;
				newSocket.sendMsg(socket, login, sentMsg);

				//chatWindowBox.AppendText(login + ": " + sentMsg + "\n");
			}
			else
			{
				MessageBox.Show("You don't have connection to use chat!");
			}

			messageBox.Clear();
		}


	}
}

