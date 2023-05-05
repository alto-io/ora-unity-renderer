namespace ORARenderer
{
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;
	using System.Xml.Linq;
	using UnityEngine;
	using UnityEngine.Networking;

	public class ORAReader : MonoBehaviour
	{
		[SerializeField] private List<string> oraFileNames = new List<string>();

		//[SerializeField]
		private ArcadianParts arcadianReference = null;

		public ArcadianParts ArcadianReference { get { return arcadianReference; } }

		public bool IsLoaded { get; private set; } = false;

		public ArcadianParts GetPartData(ArcadianLoadRequest request)
		{
			if (!IsLoaded || request == null || request.Parts.Count == 0)
				return null;

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
			if (!IsLoaded)
				return null;

			ArcadianParts newArcadian = new ArcadianParts();

			foreach (LocationData location in arcadianReference.Locations)
			{
				var newLocation = new LocationData { Name = location.Name, };
				newLocation.Parts.Add(new PartData(location.Parts[UnityEngine.Random.Range(0, location.Parts.Count)]));
				newArcadian.Locations.Add(newLocation);
			}

			return newArcadian;
		}

		#region Static

		public static ORAReader GetInstance()
		{
			if (instance == null)
				instance = FindObjectOfType<ORAReader>();
			else if (instance != FindObjectOfType<ORAReader>())
				Destroy(FindObjectOfType<ORAReader>());

			return instance;
		}

		private static ORAReader instance = null;

		#endregion

		#region Private

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
			arcadianReference = null;
			StartCoroutine(LoadOraFilesCR());
		}

		private IEnumerator LoadOraFilesCR()
		{
			IsLoaded = false;
			arcadianReference = new ArcadianParts();
			foreach (string oraFile in oraFileNames)
				yield return GetOraStreamCR(Application.streamingAssetsPath + "\\" + oraFile);
			IsLoaded = true;
		}

		private IEnumerator GetOraStreamCR(string url)
		{
			using (UnityWebRequest request = UnityWebRequest.Get(url))
			{
				yield return request.SendWebRequest();

				if (request.result == UnityWebRequest.Result.Success)
				{
					yield return new WaitUntil(() => request.downloadHandler.isDone);
					MemoryStream memStream = new MemoryStream(request.downloadHandler.data, true);
					ReadFromStream(memStream);
				}
				else
				{
					Debug.LogError("Could not get file");
				}
			}
		}

		private void ReadFromStream(MemoryStream stream)
		{
			XDocument stackXml = null;

			using (ZipArchive zip = new ZipArchive(stream))
			{
				foreach (ZipArchiveEntry entry in zip.Entries)
				{
					if (entry.Name == "stack.xml")
					{
						stackXml = XDocument.Load(entry.Open());
						break;
					}
				}

				if (stackXml == null)
				{
					Debug.LogError("Invalid .ora file! stack.xml does not exist");
					return;
				}

				foreach (XElement image in stackXml.Elements())
				{
					arcadianReference.canvasSizeW = (int)image.Attribute("w");
					arcadianReference.canvasSizeH = (int)image.Attribute("h");

					foreach (XElement topStack in image.Elements())
						foreach (XElement root in topStack.Elements())
							foreach (XElement layer in root.Elements())
								ReadLayerXml(layer, zip);
				}
			}
		}

		private void ReadLayerXml(XElement layer, ZipArchive zip)
		{
			LocationData locationData;
			string locationName = (string)layer.Attribute("name");
			bool isNewLocation = false;

			if (arcadianReference.Locations.Exists(x => x.Name == locationName))
			{
				locationData = arcadianReference.Locations.Find(x => x.Name == locationName);
			}
			else
			{
				locationData = new LocationData
				{
					Name = locationName,
					Opacity = (float)layer.Attribute("opacity"),
				};
				isNewLocation = true;
			}

			foreach (XElement partElement in layer.Elements())
			{
				var partData = new PartData()
				{
					Name = (string)partElement.Attribute("name"),
					Location = locationData.Name,
					Src = GetPartSrc(partElement, zip),

					Opacity = (float)partElement.Attribute("opacity"),
					PosX = (int)partElement.Attribute("x"),
					PosY = (int)partElement.Attribute("y")
				};
				locationData.Parts.Add(partData);
			}

			if (isNewLocation)
				arcadianReference.Locations.Add(locationData);
		}

		private byte[] GetPartSrc(XElement part, ZipArchive zip)
		{
			string pngFileName = (
				(string)part.Attribute("src")).Substring(
				((string)part.Attribute("src")).LastIndexOf('/') + 1
			);

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
			
			if (string.IsNullOrWhiteSpace(part.Name))
				return defaultData;

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
