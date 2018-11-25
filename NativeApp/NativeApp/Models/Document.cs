using System;
using System.Collections.Generic;

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

        public Document(string file_name, string file_content, DateTime file_creation_date, DateTime file_update_date, int file_creator_id, int? user_id)
        {
            this.file_name = file_name;
            this.file_content = file_content;
            this.file_creation_date = file_creation_date;
            this.file_update_date = file_update_date;
            this.file_creator_id = file_creator_id;
            this.file_last_editor_id = file_last_editor_id;
            this.user_id = user_id;
        }

        public void UpdateDocument(string documentContent)
        {
            this.file_content = documentContent;
            this.file_update_date = DateTime.Now;
        }
    }


}
