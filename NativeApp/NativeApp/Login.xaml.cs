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
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
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

		internal static Login main; // new thread

		private String login, password;
		int idPickedDoc, loggedUser;
		public static List<sUser> lockedFilesList;

		String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Files";
		AppStatus appStatus = new AppStatus();

		Sockets newSocket = new Sockets();
		sUser suser = new sUser();

		ButtonAutomationPeer peer;
		IInvokeProvider invokeProv;

		public Login()
		{
			InitializeComponent();
			main = this; // new thread

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
				//user.GetUsersList();

				if (user.user_exists == true && user.logged_in)
				{
					appStatus.isUserLogged = true;
				    AppStatus.userID = user.user_id;
					MenuGrid.Visibility = Visibility.Hidden;
					loginMenuGrid.Visibility = Visibility.Visible;
					LoginGrid.Visibility = Visibility.Hidden;
					welcomeGrid.Visibility = Visibility.Visible;
					welcomeLabel.Content = $"Logged in as \n{user.user_login}";
				    chatBtn.Visibility = Visibility.Visible;

                    Documents documents = new Documents();
					documents.Get2(true);
					listOfFiles.ItemsSource = new DirectoryInfo(path).GetFiles();

					loginBttn.Visibility = Visibility.Hidden;
					logoutBtn.Visibility = Visibility.Visible;

					loggedUser = user.user_id;
					lockedFilesList = user.list_of_locked_files;
					Console.WriteLine("Locked file: {0}", lockedFilesList);

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
			chatBtn.Visibility = Visibility.Hidden;

		    if (appStatus.isUserLogged == true)
		    {
		        loginBttn.Visibility = Visibility.Hidden;
		        logoutBtn.Visibility = Visibility.Visible;
		        chatBtn.Visibility = Visibility.Visible;
		    }
		    else
		    {

		        loginBttn.Visibility = Visibility.Visible;
		        logoutBtn.Visibility = Visibility.Hidden;
		        chatBtn.Visibility = Visibility.Hidden;

		    }

        }

		private void filesBtn_Click(object sender, RoutedEventArgs e)
		{
			if (appStatus.isUserLogged == true)
		    {
		        loginBttn.Visibility = Visibility.Hidden;
		        logoutBtn.Visibility = Visibility.Visible;
		        chatBtn.Visibility = Visibility.Visible;
		    }
		    else
		    {
		        loginBttn.Visibility = Visibility.Visible;
		        logoutBtn.Visibility = Visibility.Hidden;
		        chatBtn.Visibility = Visibility.Hidden;

		    }
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
			toggleSwitch.IsEnabled = false;
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
		    try
		    {
                String title = listOfFiles.SelectedItems[0].ToString();
				string text = System.IO.File.ReadAllText(System.IO.Path.Combine(path, title));

		    
		        foreach (Document doc in Documents.currentDocuments)
		        {
		            if (doc.file_name.Equals(title.Substring(0, title.Length - 4)))
		            {
		                idPickedDoc = doc.file_id;
		                //Console.WriteLine("Id wybranego dokumnetu: {0}", idPickedDoc);
		            }
		        }
		        TitleBox.Text = title.Substring(0, title.Length - 4);
		        contentBox.Text = text;
            }
		    catch (Exception ee)
		    {
                Console.WriteLine(ee);
		    }

		    filesListGrid.Visibility = Visibility.Hidden;
			newFileGrid.Visibility = Visibility.Visible;


			if (appStatus.isServerOnline == true && appStatus.isOnline == true && appStatus.isUserLogged)
			{
				try
				{
					foreach (sUser s in lockedFilesList)
					{
						if(s.file_name.Equals(TitleBox.Text) && s.file_id.Equals(idPickedDoc) && !s.file_name.Equals(newSocket.unlockedFile))
						{
							toggleSwitch.IsChecked = false;
							toggleSwitch.IsEnabled = false;
							Console.WriteLine("Warunek 1");
							
						} else if (!s.file_name.Equals(TitleBox.Text))
						{
							toggleSwitch.IsChecked = false;
							toggleSwitch.IsEnabled = true;
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}

				if (TitleBox.Text.Equals(newSocket.lockedFile) && !TitleBox.Text.Equals(newSocket.unlockedFile))
				{
					toggleSwitch.IsChecked = false;
					toggleSwitch.IsEnabled = false;
					Console.WriteLine("Warunek 2");
				}
				else if(lockedFilesList == null)
				{
					toggleSwitch.IsEnabled = true;
				}

				if (TitleBox.Text.Equals(newSocket.unlockedFile) && !TitleBox.Text.Equals(newSocket.lockedFile))
				{
					toggleSwitch.IsEnabled = true;
				}
			} else
			{
				toggleSwitch.IsChecked = true;
				toggleSwitch.IsEnabled = false;
				Console.WriteLine("Warunek 3");
			}

			if(toggleSwitch.IsChecked == true && newSocket.lockedFile != TitleBox.Text)
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
				Console.WriteLine("Warunek 4");

			}
		}


		private void backBtn_Click(object sender, RoutedEventArgs e)
		{
		    if (appStatus.isUserLogged == true)
		    {
		        loginBttn.Visibility = Visibility.Hidden;
		        logoutBtn.Visibility = Visibility.Visible;
		        chatBtn.Visibility = Visibility.Visible;
		    }
		    else
		    {

		        loginBttn.Visibility = Visibility.Visible;
		        logoutBtn.Visibility = Visibility.Hidden;
		        chatBtn.Visibility = Visibility.Hidden;

		    }
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

		    if (appStatus.isUserLogged == true)
		    {
		        loginBttn.Visibility = Visibility.Hidden;
		        logoutBtn.Visibility = Visibility.Visible;
		        chatBtn.Visibility = Visibility.Visible;
		    }
		    else
		    {

		        loginBttn.Visibility = Visibility.Visible;
		        logoutBtn.Visibility = Visibility.Hidden;
		        chatBtn.Visibility = Visibility.Hidden;

		    }

            if (appStatus.isServerOnline == true && appStatus.isOnline == true && appStatus.isUserLogged == true)
		    {
                Documents documents = new Documents();
                documents.Get2(true);
            }

		    String title = TitleBox.Text;
			String content = contentBox.Text;
			string filenameNoExtension = title;
			string filename = String.Format("{0}.txt", title);
			string allPath = System.IO.Path.Combine(path, filename);

			if (!File.Exists(allPath))
			{
				if (appStatus.isServerOnline == true && appStatus.isOnline == true && appStatus.isUserLogged == true)
				{
					Document document = new Document(filenameNoExtension, content, DateTime.Now, DateTime.Now, 1, user.user_id);
					if (document.checkIfNoDuplicated())
					{
						document.PostDocument(document);
						newSocket.socketIoEmit(TitleBox.Text, 2, login, socket, user.user_id, 0);
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
				    if (appStatus.isUserLogged == true)
				    {
				        loginBttn.Visibility = Visibility.Hidden;
				        logoutBtn.Visibility = Visibility.Visible;
				        chatBtn.Visibility = Visibility.Visible;
				    }
				    else
				    {

				        loginBttn.Visibility = Visibility.Visible;
				        logoutBtn.Visibility = Visibility.Hidden;
				        chatBtn.Visibility = Visibility.Hidden;

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
					exsistingDoc.user_id = user.user_id;
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
						newSocket.socketIoEmit(TitleBox.Text, 3, login, socket, user.user_id, idPickedDoc);
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
				    if (appStatus.isUserLogged == true)
				    {
				        loginBttn.Visibility = Visibility.Hidden;
				        logoutBtn.Visibility = Visibility.Visible;
				        chatBtn.Visibility = Visibility.Visible;
				    }
				    else
				    {

				        loginBttn.Visibility = Visibility.Visible;
				        logoutBtn.Visibility = Visibility.Hidden;
				        chatBtn.Visibility = Visibility.Hidden;

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
                if (appStatus.isServerOnline == true && appStatus.isOnline == true && appStatus.isUserLogged == true)
                {
                    Documents documents = new Documents();
                    documents.Get2(true);
                }

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
						//Console.WriteLine("ID usera:{0}, id pliku: {1}", loggedUser, idPickedDoc);
						newSocket.socketIoEmit(TitleBox.Text, 1, login, socket, loggedUser, idPickedDoc);
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
						newSocket.socketIoEmit(TitleBox.Text, 0, login, socket, loggedUser, idPickedDoc);
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
			}
			
		}

		private void chatDatagrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{

		}

		private void closeChatBtn_Click(object sender, RoutedEventArgs e)
		{

			ChatGrid.Visibility = Visibility.Hidden;
			chatBorder.Visibility = Visibility.Hidden;
			
		}

		private void sendBtn_Click(object sender, RoutedEventArgs e)
		{
			if (appStatus.isServerOnline == true && appStatus.isOnline == true && appStatus.isUserLogged)
			{
				string sentMsg = messageBox.Text;
				newSocket.sendMsg(socket, login, sentMsg);

				chatWindowBox.AppendText(login + ": " + sentMsg + "\n");
			}
			else
			{
				MessageBox.Show("You don't have connection to use chat!");
			}

			messageBox.Clear();
		}


		// nowy wątek dla czatu
		internal string Status
		{
			get { return chatWindowBox.Text.ToString(); }
			set { Dispatcher.Invoke(new Action(() => { chatWindowBox.AppendText(value); })); }
		}

		internal bool Toggle
		{
			get { return toggleSwitch.IsEnabled = true; }
			set { Dispatcher.Invoke(new Action(() => {
				if (TitleBox.Text.Equals(newSocket.lockedFile))
				{
					toggleSwitch.IsEnabled = value;
				}
			})); }
		}

		internal bool CleanFiles
		{
			get { return false; }
			set {
				Dispatcher.Invoke(new Action(() => {
					listOfFiles.ItemsSource = null;
					downloadTable();
					//Console.WriteLine("wyczyszczono");
				}));
			}
		}

		internal bool UploadFiles
		{
			get { return false; }
			set
			{
				Dispatcher.Invoke(new Action(() => {
					cleanTable();
					//Console.WriteLine("uzupelniono");
				}));
			}
		}

		public void downloadTable()
		{
			Documents doc = new Documents();
			doc.Get2(true);
			System.Threading.Thread.Sleep(2000);
		}
		public void cleanTable()
		{
			listOfFiles.ItemsSource = null;
			listOfFiles.ItemsSource = new DirectoryInfo(path).GetFiles();
			
		}

	}
}

