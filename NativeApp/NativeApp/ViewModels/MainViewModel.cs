using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NativeApp.Models;

namespace NativeApp.ViewModels
{
    public class MainViewModel
    {
        private DocumentModel _document;
        public FileViewModel File { get; set; }

        public MainViewModel()
        {
            _document = new DocumentModel();
            File = new FileViewModel(_document);
            Get();
            Get2();
        }
        async void Get()
        {
            var r = await DownloadPage("http://127.0.0.1:5000/");
        }

        async void Get2()
        {
            var r = await DownloadPage2("http://127.0.0.1:5000/files");
        }

        async Task<string> DownloadPage(string url)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using (var r = await client.GetAsync(new Uri(url)))
                {
                    var file = await r.Content.ReadAsStringAsync();
                    _document.HelloWorld = file;
                    return null;
                }
            }
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

                return model.ToString();
            }
        }
    }
}
