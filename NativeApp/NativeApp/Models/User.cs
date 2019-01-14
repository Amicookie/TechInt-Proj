using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NativeApp.Models
{
    public class User
    {
        public int? user_id { get; set; }
        public string user_login { get; set; }
        public string user_password { get; set; }
        public bool user_exists { get; set; }
        public bool logged_in { get; set; }

        public User(string login, string password)
        {
            user_login = login;
            user_password = password;
        }

        public async void Get2()
        {
            var r = await IsAuthenticated();
        }

        public async Task<bool> IsAuthenticated()
        {
            using (var client = new HttpClient())
            {
                User p = new User(user_login,user_password);
                client.BaseAddress = new Uri("http://192.168.43.218:5000/");
                var response = client.PostAsJsonAsync("users", p).Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsAsync<User>();
                    this.user_exists = jsonContent.user_exists;
                    this.logged_in = jsonContent.logged_in;
                    Console.Write("Success");
                    return true;
                }
                else
                    Console.Write("Error");
                return false;
            }
        }


    }
}
