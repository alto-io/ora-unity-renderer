using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ORARenderer")]
public class ORARenderer : ScriptableObject
{
	private const string MAIN_DIRECTORY = "Assets/ORARenderer/";
	private const string OUTPUT_SUB_DIR = "Extracted/";

	[SerializeField] public ORAArcadianImage arcadianImage;

	public void ReadFiles()
	{
		List<FileInfo> allOraFiles = GetAllOraFiles();
		ClearOutputDirectory();
		List<string> newSubDirs = ExtractOraFiles(allOraFiles);
		AssetDatabase.Refresh();

		foreach(string subDir in newSubDirs)
		{
			ReadXml(subDir);
		}
	}

	private List<FileInfo> GetAllOraFiles()
	{
		string[] filePaths = Directory.GetFiles(MAIN_DIRECTORY);

		List<FileInfo> allOraFiles = new List<FileInfo>();

		foreach (string filePath in filePaths)
		{
			FileInfo fileInfo = new FileInfo(filePath);
			if (fileInfo.Extension == ".ora")
				allOraFiles.Add(fileInfo);
		}

		return allOraFiles;
	}

	private void ClearOutputDirectory()
	{
		string path = MAIN_DIRECTORY + OUTPUT_SUB_DIR;
		if (!Directory.Exists(path))
			return;

		DirectoryInfo di = new DirectoryInfo(path);

		foreach (FileInfo file in di.EnumerateFiles())
			file.Delete();

		foreach (DirectoryInfo dir in di.EnumerateDirectories())
			dir.Delete(true);
	}

	private List<string> ExtractOraFiles(List<FileInfo> files)
	{
		List<string> newSubDirs = new List<string>();

		foreach (FileInfo fileInfo in files)
		{
			string fileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
			string newPath = MAIN_DIRECTORY + OUTPUT_SUB_DIR + fileName;

			ZipFile.ExtractToDirectory(fileInfo.FullName, newPath);
			newSubDirs.Add(newPath);
		}

		return newSubDirs;
	}

	private void ReadXml(string directory)
	{
		XmlDocument doc = new XmlDocument();
		doc.Load(directory + "/stack.xml");
		var json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None);
		arcadianImage = JsonConvert.DeserializeObject<ORAArcadianImage>(json);
		Debug.Log(json);

		return;

		XDocument xml = XDocument.Load(directory + "/stack.xml");

		var filtered =
			xml.Descendants()
			.Where(p => p.HasElements && p.Name != "image" && p.HasAttributes);

		foreach (var el in filtered)
		{
			Debug.Log(el);
			Debug.Log(el.Descendants().Count());
		}
	}
}
