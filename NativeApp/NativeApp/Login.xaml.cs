using NativeApp.Models;
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
                    Documents documents = new Documents();
                    documents.Get2(true);
                    listOfFiles.ItemsSource = new DirectoryInfo(path).GetFiles();
                    loginBttn.Visibility = Visibility.Hidden;
                    logoutBtn.Visibility = Visibility.Visible;
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
        }


private void SocketIO_Click(object sender, RoutedEventArgs e)
        {
            Sockets newSocket = new Sockets();
            newSocket.socketIoManager();
            socketStatusLabel.Content = Sockets.labelText;
        }
    }


}

