using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NativeApp.Models
{

    public class Documents
    {
        public static string mainUrl = "http://127.0.0.1:5000";
        public static string documentUrl = mainUrl + @"/Files";
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Files";
        private static List<Document> documentsAA;

        public bool isUpdated { get; set; }

        public Documents()
        {
            isUpdated = true;
        }

        public async void Get2()
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
            string directoryPath = path;
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

                using (StreamWriter str = File.CreateText(path))
                {
                    str.WriteLine(i.file_content);
                    str.Flush();
                }

            }
        }
    }


}
