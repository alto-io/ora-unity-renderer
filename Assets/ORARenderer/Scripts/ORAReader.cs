namespace ORARenderer
{
	using System.IO;
	using System.IO.Compression;
	using System.Xml.Linq;
	using UnityEngine;

	public class ORAReader : ORASingleton<ORAReader>
	{
		[SerializeField] private string oraPath = "Assets/ORARenderer/arcadians.ora";

		[SerializeField] private ArcadianParts arcadianReference = null;

		public ArcadianParts GetPartData(ArcadianLoadRequest request)
		{
			if (request == null || request.Parts.Count == 0)
				return null;

			if (arcadianReference == null || arcadianReference.Locations == null || arcadianReference.Locations.Count == 0)
			{
				ReadStackXml();
				if (arcadianReference == null || arcadianReference.Locations == null || arcadianReference.Locations.Count == 0)
					return null;
			}

			ArcadianParts newArcadian = new ArcadianParts();

			foreach (PartsLoadRequest part in request.Parts)
			{
				if (newArcadian.Locations.Exists(x => x.Name == part.Location))
					Debug.LogError($"Duplicates of {part.Location} exist! May cause errors");

				PartData partData = FindPart(part);
				var locationData = new LocationData { Name = part.Location };
				locationData.Parts.Add(partData);
				newArcadian.Locations.Add(locationData);
			}

			return newArcadian;
		}

		public ArcadianParts GetRandom()
		{
			if (arcadianReference == null || arcadianReference.Locations == null || arcadianReference.Locations.Count == 0)
			{
				ReadStackXml();
				if (arcadianReference == null || arcadianReference.Locations == null || arcadianReference.Locations.Count == 0)
					return null;
			}

			ArcadianParts newArcadian = new ArcadianParts();

			foreach (LocationData location in arcadianReference.Locations)
			{
				var newLocation = new LocationData { Name = location.Name, };
				newLocation.Parts.Add(new PartData(location.Parts[Random.Range(0, location.Parts.Count)]));
				newArcadian.Locations.Add(newLocation);
			}

			return newArcadian;
		}

		#region Private

		private void Awake() => arcadianReference = null;

		private void ReadStackXml()
		{
			XDocument stackXml = null;

			using (ZipArchive zip = ZipFile.Open(oraPath, ZipArchiveMode.Read))
			{
				foreach (ZipArchiveEntry entry in zip.Entries)
				{
					if (entry.Name == "stack.xml")
					{
						stackXml = XDocument.Load(entry.Open());
						break;
					}
				}
			}

			if (stackXml == null)
			{
				Debug.LogError("Invalid .ora file! stack.xml does not exist");
				arcadianReference = null;
				return;
			}

			arcadianReference = new ArcadianParts();

			foreach (XElement image in stackXml.Elements())
				foreach (XElement topStack in image.Elements())
					foreach (XElement root in topStack.Elements())
						foreach (XElement layer in root.Elements())
							ReadLayerXml(layer);
		}

		private void ReadLayerXml(XElement layer)
		{
			var locationData = new LocationData
			{
				Name = (string)layer.Attribute("name"),
				Opacity = (float)layer.Attribute("opacity"),
			};

			foreach (XElement partElement in layer.Elements())
			{
				var partData = new PartData()
				{
					Name = (string)partElement.Attribute("name"),
					Location = locationData.Name,
					Src = GetPartSrc(partElement),

					Opacity = (float)partElement.Attribute("opacity"),
					PosX = (int)partElement.Attribute("x"),
					PosY = (int)partElement.Attribute("y")
				};
				locationData.Parts.Add(partData);
			}

			arcadianReference.Locations.Add(locationData);
		}

		private byte[] GetPartSrc(XElement part)
		{
			string pngFileName = (
				(string)part.Attribute("src")).Substring(
				((string)part.Attribute("src")).LastIndexOf('/') + 1
			);

			using (ZipArchive zip = ZipFile.Open(oraPath, ZipArchiveMode.Read))
				foreach (ZipArchiveEntry entry in zip.Entries)
					if (entry.Name == pngFileName)
						return ConvertStreamToByteArray(entry.Open());

			return null;
		}

		private byte[] ConvertStreamToByteArray(Stream stream)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				stream.CopyTo(ms);
				return ms.ToArray();
			}
		}

		private PartData FindPart(PartsLoadRequest part)
		{
			var defaultData = new PartData
			{
				Name = part.Name,
				Location = part.Location
			};

			if (!arcadianReference.Locations.Exists(x => x.Name == part.Location))
			{
				Debug.LogError($"Part Location: {part.Location} does not exist!");
				return defaultData;
			}

			var locationRef = arcadianReference.Locations.Find(x => x.Name == part.Location);

			if (!locationRef.Parts.Exists(x => x.Name == part.Name))
			{
				Debug.LogError($"Part Name: {part.Name} at Part Location: {part.Location} does not exist!");
				return defaultData;
			}

			var partRef = locationRef.Parts.Find(x => x.Name == part.Name);

			return new PartData(partRef);
		}

		#endregion
	}
}
