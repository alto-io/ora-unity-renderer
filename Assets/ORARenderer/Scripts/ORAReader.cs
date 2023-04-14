namespace ORARenderer
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using System.Xml.Linq;
	using UnityEditor;
	using UnityEngine;

	[Serializable]
	public class PartData
	{
		public string Location;
		public string Name;

		public string Src;
		public float Opacity;
		public Vector2 Position;
	}

	[Serializable]
	public class ArcadianParts
	{
		public List<PartData> Parts;
	}

	public class ORAReader : MonoBehaviour
	{
		private const string MAIN_DIRECTORY = "Assets/ORARenderer/";
		private const string OUTPUT_SUB_DIR = "Extracted/";

		[SerializeField] private string oraPath = "Assets/ORARenderer/arcadians.ora";
		[SerializeField] private ArcadianParts arcadianParts = new ArcadianParts();

		private XDocument xmlStack;

		public void Start()
		{
			ClearOutputDirectory();
			GetAllParts();
			AssetDatabase.Refresh();
		}

		private void ClearOutputDirectory()
		{
			string path = MAIN_DIRECTORY + OUTPUT_SUB_DIR;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
				return;
			}

			DirectoryInfo di = new DirectoryInfo(path);

			foreach (FileInfo file in di.EnumerateFiles())
				file.Delete();

			foreach (DirectoryInfo dir in di.EnumerateDirectories())
				dir.Delete(true);
		}

		public void GetAllParts()
		{
			string xmlPath = MAIN_DIRECTORY + OUTPUT_SUB_DIR + "/" + "stack.xml";
			using (ZipArchive zip = ZipFile.Open(oraPath, ZipArchiveMode.Read))
				foreach (ZipArchiveEntry entry in zip.Entries)
					if (entry.Name == "stack.xml")
						entry.ExtractToFile(xmlPath);

			xmlStack = XDocument.Load(xmlPath);

			for (int i = 0; i < arcadianParts.Parts.Count; i++)
			{
				PartData part = arcadianParts.Parts[i];
				arcadianParts.Parts[i] = GetPartData(part.Name, part.Location);
			}
		}

		public PartData GetPartData(string partName, string partLocation)
		{
			PartData newPart = new PartData();

			IEnumerable<XElement> query = (
				from element in xmlStack.Descendants("stack")
				where (string)element.Attribute("name") == partLocation
				select element
			);

			XElement partElement = null;
			foreach (XElement element in query.Descendants().Where(p => !p.HasElements))
			{
				if ((string)element.Attribute("name") == partName)
				{
					partElement = element;
					break;
				}
			}

			newPart.Location = partLocation;
			newPart.Name = partName;

			if (partElement != null)
			{
				string pngFileName = (
					(string)partElement.Attribute("src")).Substring(
					((string)partElement.Attribute("src")).LastIndexOf('/') + 1
				);

				string pngPath = MAIN_DIRECTORY + OUTPUT_SUB_DIR + pngFileName;
				Debug.Log((string)partElement.Attribute("src"));
				Debug.Log(pngPath + "\n" + pngFileName);

				using (ZipArchive zip = ZipFile.Open(oraPath, ZipArchiveMode.Read))
					foreach (ZipArchiveEntry entry in zip.Entries)
						if (entry.Name == pngFileName)
							entry.ExtractToFile(pngPath);

				newPart.Src = pngPath;
				newPart.Opacity = (float)partElement.Attribute("opacity");
				newPart.Position = new Vector2
				(
					(float)partElement.Attribute("x"),
					(float)partElement.Attribute("y")
				);
			}

			return newPart;
		}
	}
}
