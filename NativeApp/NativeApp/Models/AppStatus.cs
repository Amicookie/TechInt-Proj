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

        public AppStatus()
        {
            isOnline = CheckForInternetConnection();
            var t = CheckForServerStatusAsync();
            isServerOnline = t.Result;
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

        private static async Task<bool> CheckForServerStatusAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (var r = await client.GetAsync(Documents.mainUrl))
                    {
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
