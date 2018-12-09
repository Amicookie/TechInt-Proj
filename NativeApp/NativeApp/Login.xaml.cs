﻿using NativeApp.Models;
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

namespace NativeApp
{
	/// <summary>
	/// Logika interakcji dla klasy Login.xaml
	/// </summary>
	public partial class Login : Window
	{
		String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Files";
		AppStatus appStatus = new AppStatus();
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

			mainGrid.Visibility = Visibility.Hidden;
			if (appStatus.isServerOnline == true && appStatus.isOnline == true)
			{
				loginBtn.Visibility = Visibility.Visible;
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
			User user;
			String login = loginBox.Text;
			String password = passBox.Password.ToString();
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
					welcomeLabel.Content = $"Zalogowany jako {user.user_login}";
				}
				else
				{
					appStatus.isUserLogged = false;
					Console.WriteLine("Wrong Credentials");
				}
			}

		}

		private void workOffBtn_Click(object sender, RoutedEventArgs e)
		{
			appStatus.isUserLogged = false;
			filesListGrid.Visibility = Visibility.Visible;
			LoginGrid.Visibility = Visibility.Hidden;
			newFileGrid.Visibility = Visibility.Hidden;
			welcomeGrid.Visibility = Visibility.Hidden;

			listOfFiles.ItemsSource = null;
			listOfFiles.ItemsSource = new DirectoryInfo(path).GetFiles();

			loginMenuGrid.Visibility = Visibility.Visible;

		}

		private void filesBtn_Click(object sender, RoutedEventArgs e)
		{
			filesListGrid.Visibility = Visibility.Visible;
			newFileGrid.Visibility = Visibility.Hidden;
			welcomeGrid.Visibility = Visibility.Hidden;

			listOfFiles.ItemsSource = null;
			listOfFiles.ItemsSource = new DirectoryInfo(path).GetFiles();
			Documents documents = new Documents();
			documents.Get2(true);
			if (Documents.currentDocuments != null)
			{
				documents.CompareDocuments();
			}


		}

		private void newFileBtn_Click(object sender, RoutedEventArgs e)
		{
			filesListGrid.Visibility = Visibility.Hidden;
			newFileGrid.Visibility = Visibility.Visible;
			welcomeGrid.Visibility = Visibility.Hidden;

			TitleBox.Clear();
			contentBox.Clear();
		}


		private void logoutBtn_Click(object sender, RoutedEventArgs e)
		{
			MenuGrid.Visibility = Visibility.Visible;
			loginMenuGrid.Visibility = Visibility.Hidden;
			mainGrid.Visibility = Visibility.Visible;
			filesListGrid.Visibility = Visibility.Hidden;
			newFileGrid.Visibility = Visibility.Hidden;
			welcomeGrid.Visibility = Visibility.Hidden;
		}

		private void Row_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

			String title = listOfFiles.SelectedItems[0].ToString();
			string text = System.IO.File.ReadAllText(System.IO.Path.Combine(path, title));
			filesListGrid.Visibility = Visibility.Hidden;
			newFileGrid.Visibility = Visibility.Visible;
			TitleBox.Text = title.Substring(0, title.Length - 4);
			contentBox.Text = text;

		}

		private void saveBtn_Click(object sender, RoutedEventArgs e)
		{
			String title = TitleBox.Text;
			String content = contentBox.Text;

			string filename = String.Format("{0}.txt", title);
			string allPath = System.IO.Path.Combine(path, filename);

			if (!File.Exists(allPath))
			{
				using (StreamWriter str = File.CreateText(allPath))
				{
					str.WriteLine(content);
					str.Flush();

					MessageBox.Show("File has been saved");
				}
				if (appStatus.isServerOnline == true && appStatus.isOnline == true)
				{
					Document document = new Document(filename, content, DateTime.Now, DateTime.Now, 1, 0);
					if (document.checkIfNoDuplicated())
					{
						document.PostDocument(document);

					}
					else
					{
						document.file_name = document.file_name + "1";
					}
				}
			}
			else if (File.Exists(allPath))
			{
				using (var str = new StreamWriter(allPath))
				{
					str.WriteLine(content);
					str.Flush();

					MessageBox.Show("File has been overwritten");
				}
			}

			TitleBox.Clear();
			contentBox.Clear();
		}

		private void SocketIO_Click(object sender, RoutedEventArgs e)
		{
			Sockets newSocket = new Sockets();
			newSocket.socketIoManager();
			socketStatusLabel.Content = Sockets.labelText;


		}
	}


}

