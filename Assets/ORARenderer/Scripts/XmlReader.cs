using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public class XmlReader : MonoBehaviour
{
	public class Part
	{
		public string Name;
		public string Path;
	}

	private string xmlRoot = "Assets/Resources/Female Assassin/";
	private string xmlFileName = "stack.xml";
	private List<Part> parts = new List<Part>();

	void Start()
	{
		XDocument xml = XDocument.Load(xmlRoot + xmlFileName);

		var query = (
			from el in xml.Descendants("stack")
			where (string)el.Attribute("name") == "Left Hand"
			select el
		);

		parts = new List<Part>();
		foreach (var el in query.Descendants().Where(p => !p.HasElements))
		{
			parts.Add(
				new Part
				{
					Name = el.Attribute("name").Value,
					Path = xmlRoot + el.Attribute("src").Value,
				}
			);
		}

		foreach (var part in parts)
		{
			Debug.Log(part.Name + "/" + part.Path);
		}
	}
}
