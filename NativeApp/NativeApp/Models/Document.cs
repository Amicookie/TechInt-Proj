using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NativeApp.Models;

namespace NativeApp
{

    public class Document
    {
        public int file_id { get; set; }
        public string file_name { get; set; }
        public string file_content { get; set; }
        public DateTime file_creation_date { get; set; }
        public DateTime file_update_date { get; set; }
        public int file_creator_id { get; set; }
        public int file_last_editor_id { get; set; }
        public int? user_id { get; set; }
        public bool is_online_file { get; set; }

        public Document(string file_name, string file_content, DateTime file_creation_date, DateTime file_update_date, int file_creator_id, int? user_id,bool is_online_file = true)
        {
            this.file_name = file_name;
            this.file_content = file_content;
            this.file_creation_date = file_creation_date;
            this.file_update_date = file_update_date;
            this.file_creator_id = file_creator_id;
            this.file_last_editor_id = file_last_editor_id;
            this.user_id = user_id;
            this.is_online_file = is_online_file;
        }


        public void PostDocument(Document document)
        {
            Get2(document);
        }

        public async void Get2(Document document)
        {
            var r = await IsAuthenticated(document);
        }

        public async Task<bool> IsAuthenticated(Document document)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(adresIP.adres);
                var response = client.PostAsJsonAsync("files", document).Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    Console.Write("Success");
                    return true;
                }
                else
                    Console.Write("Error");
                return false;
            }
        }


        public async void CallUpdateDoc()
        {
            var r = await PutFile();
        }
        public async Task<bool> PutFile()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(adresIP.adres);
                var response = client.PutAsJsonAsync($@"files/{this.file_id}", this).Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    Console.Write("Success");
                    return true;
                }
                else
                    Console.Write("Error");
                return false;
            }
        }


        public bool checkIfNoDuplicated()
        {
            Documents documents = new Documents();
            documents.Get2(false);
            var list = Documents.currentDocuments;
            int falseCount = 0;
            if (list.Count > 0)
            {
                foreach (var doc in list)
                {
                    if (file_name == doc.file_name)
                    {
                        falseCount++;
                    }
                }

                if (falseCount > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }

    }


}
