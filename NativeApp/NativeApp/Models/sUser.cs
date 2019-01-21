using NativeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NativeApp
{
	public class sUser
	{
		public int user_id { get; set; }
		public string username { get; set; }
		public string file_name { get; set; }
		public int file_id { get; set; }
	}
}
