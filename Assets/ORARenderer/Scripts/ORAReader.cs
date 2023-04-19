namespace ORARenderer
{
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using System.Xml.Linq;
	using UnityEditor;
	using UnityEngine;

	public class ORAReader : Singleton<ORAReader>
	{
		#region Public

		public static ORAReader Instance { get; private set; } = null;

		[SerializeField] private string oraPath = "Assets/ORARenderer/arcadians.ora";

		public ArcadianParts GetPartData(ArcadianPartLoadRequest request)
		{
			ArcadianParts newParts = ConvertLoadRequest(request);

			using (ZipArchive zip = ZipFile.Open(oraPath, ZipArchiveMode.Read))
			{
				foreach (ZipArchiveEntry entry in zip.Entries)
				{
					if (entry.Name == "stack.xml")
					{
						xmlStack = XDocument.Load(entry.Open());
						break;
					}
				}
			}

			if (xmlStack == null)
			{
				Debug.LogError("Invalid ORA file! Does not contain stack.xml");
				return null;
			}

			for (int i = 0; i < newParts.Parts.Count; i++)
			{
				PartData part = newParts.Parts[i];
				newParts.Parts[i] = GetPartData(part.Name, part.Location);
			}

			return newParts;
		}

		#endregion

		#region Private

		private const string MAIN_DIRECTORY = "Assets/ORARenderer/";
		private const string OUTPUT_SUB_DIR = "Extracted/";
		private XDocument xmlStack;

		private ArcadianParts ConvertLoadRequest(ArcadianPartLoadRequest request)
		{
			ArcadianParts newParts = new ArcadianParts();

			newParts.Parts.Add(new PartData { Name = request.Skin, Location = PartLocation.Skin });
			newParts.Parts.Add(new PartData { Name = request.Eyes, Location = PartLocation.Eyes });
			newParts.Parts.Add(new PartData { Name = request.Mouth, Location = PartLocation.Mouth });
			newParts.Parts.Add(new PartData { Name = request.Top, Location = PartLocation.Top });
			newParts.Parts.Add(new PartData { Name = request.Bottom, Location = PartLocation.Bottom });
			newParts.Parts.Add(new PartData { Name = request.RightHand, Location = PartLocation.RightHand });
			newParts.Parts.Add(new PartData { Name = request.LeftHand, Location = PartLocation.LeftHand });
			newParts.Parts.Add(new PartData { Name = request.Head, Location = PartLocation.Head });

			return newParts;
		}

		private PartData GetPartData(string partName, PartLocation partLocation)
		{
			PartData newPart = new PartData();

			IEnumerable<XElement> query = (
				from element in xmlStack.Descendants("stack")
				where (element.Attribute("name") != null && ((string)element.Attribute("name")).Replace(" ", string.Empty) == partLocation.ToString())
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

			if (partElement == null)
				return newPart;

			string pngFileName = (
				(string)partElement.Attribute("src")).Substring(
				((string)partElement.Attribute("src")).LastIndexOf('/') + 1
			);

			string pngPath = MAIN_DIRECTORY + OUTPUT_SUB_DIR + pngFileName;

			using (ZipArchive zip = ZipFile.Open(oraPath, ZipArchiveMode.Read))
			{
				foreach (ZipArchiveEntry entry in zip.Entries)
				{
					if (entry.Name == pngFileName)
					{
						newPart.Src = ConvertStreamToByteArray(entry.Open());
						break;
					}
				}
			}

			newPart.Opacity = (float)partElement.Attribute("opacity");
			newPart.PosX = (int)partElement.Attribute("x");
			newPart.PosY = (int)partElement.Attribute("y");

			return newPart;
		}

		private byte[] ConvertStreamToByteArray(Stream stream)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				stream.CopyTo(ms);
				return ms.ToArray();
			}
		}

		#endregion
	}
}
