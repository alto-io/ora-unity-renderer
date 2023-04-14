namespace ORARenderer
{
	using Newtonsoft.Json;
	using System;

	[Serializable]
	public class Root
	{
		[JsonProperty("?xml")]
		public Xml xml { get; set; }

		public Image image { get; set; }
	}

	[Serializable]
	public class Xml
	{
		[JsonProperty("@version")]
		public string version { get; set; }

		[JsonProperty("@encoding")]
		public string encoding { get; set; }
	}

	[Serializable]
	public class Image
	{
		[JsonProperty("@h")]
		public string h { get; set; }

		[JsonProperty("@w")]
		public string w { get; set; }

		public Stack stack { get; set; }
	}

	[Serializable]
	public class Stack
	{
		public Stack[] stack { get; set; }

		[JsonProperty("@composite-op")]
		public string compositeop { get; set; }

		[JsonProperty("@name")]
		public string name { get; set; }

		[JsonProperty("@opacity")]
		public string opacity { get; set; }

		[JsonProperty("@visibility")]
		public string visibility { get; set; }
		public object layer { get; set; }
	}



}
