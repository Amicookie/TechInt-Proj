using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NativeApp.Models
{
	public class Documents
	{
		public enum stateOfDocument
		{
			notChanged = 0,
			changedLocal = 1,
			changedGlobal = 2,
			exsistOnlyLocal = 3,
			exsistOnlyGlobal = 4
		}
		public static string mainUrl = "http://127.0.0.1:5000";
		public static string documentUrl = mainUrl + @"/files";
		private static string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Files";
		public static List<Document> currentDocuments;
		Dictionary<Document, stateOfDocument> documentsState = new Dictionary<Document, stateOfDocument>();

		public bool isUpdated { get; set; }

		public Documents()
		{
			isUpdated = true;
		}

		public async void Get2(bool ifCreate)
		{
			var r = await DownloadPage2(documentUrl, ifCreate);
		}

		async Task<string> DownloadPage2(string url, bool ifCreate)
		{
			List<Document> model = new List<Document>();
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				using (var r = await client.GetAsync(new Uri(url)))
				{
					var file = await r.Content.ReadAsStringAsync();
					model = await r.Content.ReadAsAsync<List<Document>>();
				}
				currentDocuments = model;
				if (ifCreate)
				{
					CreateFiles(model);
				}

				return model.ToString();
			}
		}
		private static void CreateFiles(List<Document> listka)
		{
			string directoryPath = path;
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
			foreach (var i in listka)
			{
				var path = System.IO.Path.Combine(directoryPath, i.file_name);
				path = path.Replace(" ", string.Empty);
				path = path + ".txt";

				using (StreamWriter str = File.CreateText(path))
				{
					str.WriteLine(i.file_content);
					str.Flush();
				}

				File.SetCreationTime(path, i.file_creation_date);
				File.SetLastWriteTime(path, i.file_update_date);
			}
		}
		//toDo
		public void CompareDocuments()
		{
			documentsState.Clear();
			Get2(false);
			var globalDocuments = currentDocuments;
			var localDocuments = Directory.GetFiles(path).ToList();
			List<String> listOfLocalPath = new List<string>();
			var commonFiles = new List<Document>();
			List<String> commonFilesNameToList = commonFiles.Select(a => a.file_name).ToList();
			foreach (var docG in globalDocuments)
			{
				foreach (var docL in localDocuments)
				{
					if (docG.file_name == Path.GetFileNameWithoutExtension(docL))
					{
						commonFiles.Add(docG);
						break;
					}
				}
			}
			var globalOnly = globalDocuments.Except(commonFiles);
			foreach (var gDocument in globalOnly)
			{
				documentsState.Add(gDocument, stateOfDocument.exsistOnlyGlobal);
			}
			var localOnly = localDocuments.Except(commonFilesNameToList);
		}

		public stateOfDocument CompareLastAccess(DateTime dateTime, string path)
		{
			DateTime localFileDateTime = File.GetLastWriteTime(path);
			if (localFileDateTime > dateTime)
			{
				return stateOfDocument.changedLocal;
			}
			else if (localFileDateTime < dateTime)
			{
				return stateOfDocument.changedGlobal;
			}
			else
			{
				return stateOfDocument.notChanged;
			}
		}
	}


}
