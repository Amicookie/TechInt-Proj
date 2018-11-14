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
        }
        async void Get()
        {
            var r = await DownloadPage("http://127.0.0.1:5000/");
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
    }
}
