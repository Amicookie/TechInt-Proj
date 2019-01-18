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
            isServerOnline = CheckForServerConnection();
        }

        public static bool CheckForInternetConnection()
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

        public static bool CheckForServerConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead(adresIP.adres))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
       

        }


}
