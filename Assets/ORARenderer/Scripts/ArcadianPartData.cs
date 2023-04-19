using System;
using System.Collections.Generic;

namespace ORARenderer
{
	/// <summary>
	/// Ordered by material order on arcadian.fbx
	/// </summary>
	public enum PartLocation { 
		Skin = 0,
		Eyes = 1,
		Mouth = 2,
		Top = 3,
		Bottom = 4,
		RightHand = 5,
		LeftHand = 6,
		Head = 7
	};

	[Serializable]
	public class ArcadianPartLoadRequest
	{
		public string Skin = "";
		public string Eyes = "";
		public string Mouth = "";
		public string Top = "";
		public string Bottom = "";
		public string RightHand = "";
		public string LeftHand = "";
		public string Head = "";
	}

	[Serializable]
	public class PartData
	{
		public PartLocation Location;
		public string Name;

		public byte[] Src;
		public float Opacity;
		public int PosX;
		public int PosY;
	}

	[Serializable]
	public class ArcadianParts
	{
		public List<PartData> Parts = new List<PartData>();
	}
}
