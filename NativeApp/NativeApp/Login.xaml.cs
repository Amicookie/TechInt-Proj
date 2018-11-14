using System;
using System.Collections.Generic;
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

		public Login()
		{
			InitializeComponent();

		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{

			mainGrid.Visibility = Visibility.Hidden;
			LoginGrid.Visibility = Visibility.Visible;
            Get2();
        }

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			LoginGrid.Visibility = Visibility.Hidden;
			mainGrid.Visibility = Visibility.Visible;
		}

		private void loginBtn_Click(object sender, RoutedEventArgs e)
		{
			MenuGrid.Visibility = Visibility.Hidden;
			loginMenuGrid.Visibility = Visibility.Visible;
			LoginGrid.Visibility = Visibility.Hidden;
			filesListGrid.Visibility = Visibility.Visible;
		}

		private void filesBtn_Click(object sender, RoutedEventArgs e)
		{
			filesListGrid.Visibility = Visibility.Visible;
			newFileGrid.Visibility = Visibility.Hidden;


		}

		private void newFileBtn_Click(object sender, RoutedEventArgs e)
		{
			filesListGrid.Visibility = Visibility.Hidden;
			newFileGrid.Visibility = Visibility.Visible;
		}

		private void logoutBtn_Click(object sender, RoutedEventArgs e)
		{
			MenuGrid.Visibility = Visibility.Visible;
			loginMenuGrid.Visibility = Visibility.Hidden;
			mainGrid.Visibility = Visibility.Visible;
			filesListGrid.Visibility = Visibility.Hidden;
			newFileGrid.Visibility = Visibility.Hidden;
		}

        async void Get2()
        {
            var r = await DownloadPage2("http://127.0.0.1:5000/files");
        }

        async Task<string> DownloadPage2(string url)
        {
            List<Document> model = new List<Document>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using (var r = await client.GetAsync(new Uri(url)))
                {
                    var file = await r.Content.ReadAsStringAsync();
                    model = await r.Content.ReadAsAsync<List<Document>>();
                }
                CreateFiles(model);
                return model.ToString();
            }
        }

        private static void CreateFiles(List<Document> listka)
        {
            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Files";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            foreach (var i in listka)
            {
                var path = System.IO.Path.Combine(directoryPath, i.file_name);
                path = path.Replace(" ", string.Empty);
                path = path.Substring(0, path.Length - 1);
                path = path + ".txt";
                File.Create(path);
                //File.WriteAllText(path, i.file_content);
                
            }
        }
    }
}
