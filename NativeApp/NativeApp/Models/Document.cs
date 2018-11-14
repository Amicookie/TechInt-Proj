using System;

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
    }
}
