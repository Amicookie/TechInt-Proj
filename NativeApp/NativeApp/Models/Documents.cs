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
		public static string mainUrl = adresIP.adres;
		public static string documentUrl = mainUrl + @"files";
		public static string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Files";
		public static List<Document> currentDocuments;
		static Dictionary<Document, stateOfDocument> documentsState = new Dictionary<Document, stateOfDocument>();

		public async void Get2(bool ifCreate)
		{
            try
            {
                var r = await DownloadPage2(documentUrl, ifCreate);
		    }
		    catch (Exception e)
		    {

		    }
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
					CreateFiles();
				}

				return model.ToString();
			}
		}
		private static void CreateFiles()
		{
			string directoryPath = path;
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

		    CompareDocuments();
			foreach (var i in documentsState.Where(a => a.Value == stateOfDocument.exsistOnlyGlobal))
			{
				var path = System.IO.Path.Combine(directoryPath, i.Key.file_name);
				path = path.Replace(" ", string.Empty);
				path = path + ".txt";

				using (StreamWriter str = File.CreateText(path))
				{
					str.WriteLine(i.Key.file_content);
					str.Flush();
				}

			    var creationDate = i.Key.file_creation_date.AddHours(-1);
			    var updateDate = i.Key.file_update_date.AddHours(-1);

                File.SetCreationTime(path, creationDate);
				File.SetLastWriteTime(path, updateDate);
			}
		    foreach (var i in documentsState.Where(a => a.Value == stateOfDocument.exsistOnlyLocal))
		    {
		        i.Key.PostDocument(i.Key);
		    }
		    foreach (var i in documentsState.Where(a => a.Value == stateOfDocument.changedGlobal))
		    {
		        var pathh = Path.Combine(path, i.Key.file_name + ".txt");
		        File.Delete(pathh);
		        using (StreamWriter str = File.CreateText(pathh))
		        {
		            str.WriteLine(i.Key.file_content);
		            str.Flush();
		        }
		        var creationDate = i.Key.file_creation_date.AddHours(-1);
		        var updateDate = i.Key.file_update_date.AddHours(-1);
		        File.SetCreationTime(pathh, creationDate);
		        File.SetLastWriteTime(pathh, updateDate);
            }
		    
            foreach (var i in documentsState.Where(a => a.Value == stateOfDocument.changedLocal))
		    {
                i.Key.user_id = AppStatus.userID;
		        i.Key.file_content = File.ReadAllText(Path.Combine(path, i.Key.file_name + ".txt"));
		        i.Key.CallUpdateDoc();
		    }
        }
		//toDo
		public static void CompareDocuments()
		{
			documentsState.Clear();
			var globalDocuments = currentDocuments;
			var localDocuments = Directory.GetFiles(path).ToList();
		    var localDocumentsName = new List<String>();
		    foreach (var file in localDocuments)
		    {
		        localDocumentsName.Add(Path.GetFileNameWithoutExtension(file));
		    }
			var commonFiles = new List<Document>();
			foreach (var docG in globalDocuments)
			{
				foreach (var docL in localDocumentsName)
				{
					if (docG.file_name == docL)
					{
						commonFiles.Add(docG);
						break;
					}
				}
			}

		    foreach (var fil in commonFiles)
		    {
		        var x = CompareLastAccess(fil.file_update_date, fil.file_name);
                documentsState.Add(fil,x);
		    }
			var globalOnly = globalDocuments.Except(commonFiles);
			foreach (var gDocument in globalOnly)
			{
				documentsState.Add(gDocument, stateOfDocument.exsistOnlyGlobal);
			}
		    List<String> commonFilesNameToList = commonFiles.Select(a => a.file_name).ToList();
		    var listOfLocalPath = localDocumentsName.Except(commonFilesNameToList);
		    foreach (var pathh in listOfLocalPath)
		    {
		        var pathFile = Path.Combine(path,pathh + ".txt");

                var content = File.ReadAllText(pathFile);
                Document document = new Document(pathh, content, DateTime.Now, DateTime.Now, AppStatus.userID, AppStatus.userID); // ====================================> POPRAWIĆ ID <========================
		        documentsState.Add(document, stateOfDocument.exsistOnlyLocal);
            }
        }

		public static stateOfDocument CompareLastAccess(DateTime dateTime, string name)
		{
		    var pathu = Path.Combine(path, name + ".txt");
			DateTime localFileDateTime = File.GetLastWriteTime(pathu);
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
