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
    public class User
    {
        public int user_id { get; set; }
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
                client.BaseAddress = new Uri(adresIP.adres);
                var response = client.PostAsJsonAsync("users", p).Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsAsync<User>();
                    this.user_exists = jsonContent.user_exists;
                    this.logged_in = jsonContent.logged_in;
					this.user_id = jsonContent.user_id;
                    Console.Write("Success");
                    return true;
                }
                else
                    Console.Write("Error");
                return false;
            }
        }

        public async void GetUsersList()
        {
            var r = await UserList();
        }

        async Task<List<User>> UserList()
        {
            List<User> model = new List<User>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using (var r = await client.GetAsync(new Uri(adresIP.adres+"/users")))
                {
                    var file = await r.Content.ReadAsStringAsync();
                    model = await r.Content.ReadAsAsync<List<User>>();
                }
                return model;
            }
        }



    }
}
