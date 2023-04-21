namespace ORARenderer
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public class PartsLoadRequest
	{
		public string Name;
		public string Location;
	}

	[Serializable]
	public class ArcadianLoadRequest
	{
		public List<PartsLoadRequest> Parts = new List<PartsLoadRequest>();
	}

	[Serializable]
	public class PartData
	{
		public PartData() { }

		public PartData(PartData copy)
		{
			Name = copy.Name;
			Location = copy.Location;
			Src = copy.Src;

			Opacity = copy.Opacity;
			PosX = copy.PosX;
			PosY = copy.PosY;
		}
		public string Name;
		public string Location;
		public byte[] Src;

		public float Opacity;
		public int PosX;
		public int PosY;
	}

	[Serializable]
	public class LocationData
	{
		public string Name;
		public List<PartData> Parts = new List<PartData>();

		public float Opacity;
	}

	[Serializable]
	public class ArcadianParts
	{
		public List<LocationData> Locations = new List<LocationData>();
	}
}
