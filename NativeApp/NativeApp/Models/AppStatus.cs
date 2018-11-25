using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NativeApp.Models
{
    public class AppStatus
    {
        public bool isOnline {get;set;}
        public bool isServerOnline { get; set; }
        public bool isUserLogged { get; set; }

        public AppStatus()
        {
            isOnline = CheckForInternetConnection();
            CheckForServerStatusAsync();
        }

        private static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public async void CheckForServerStatusAsync()
        {
                var r = await PingServer(Documents.mainUrl);

        }
        private async Task<bool> PingServer(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (var r = await client.GetAsync(url))
                    {
                        isServerOnline = r.IsSuccessStatusCode;
                        return r.IsSuccessStatusCode;
                    }
                }
            }
            catch
            {
                return false;
            }
        }


    }


}
