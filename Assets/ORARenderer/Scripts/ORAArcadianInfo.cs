using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot(ElementName = "layer")]
public class ORAArcadianLayer
{
	[XmlAttribute(AttributeName = "composite-op")]
	public string CompositeOp;

	[XmlAttribute(AttributeName = "name")]
	public string Name;

	[XmlAttribute(AttributeName = "opacity")]
	public double Opacity;

	[XmlAttribute(AttributeName = "src")]
	public string Src;

	[XmlAttribute(AttributeName = "visibility")]
	public string Visibility;

	[XmlAttribute(AttributeName = "x")]
	public int X;

	[XmlAttribute(AttributeName = "y")]
	public int Y;
}

[XmlRoot(ElementName = "stack")]
public class ORAArcadianStack
{

	[XmlElement(ElementName = "layer")]
	public List<ORAArcadianLayer> Layers;

	[XmlAttribute(AttributeName = "composite-op")]
	public string CompositeOp;

	[XmlAttribute(AttributeName = "name")]
	public string Name;

	[XmlAttribute(AttributeName = "opacity")]
	public double Opacity;

	[XmlAttribute(AttributeName = "visibility")]
	public string Visibility;

	[XmlElement(ElementName = "stack")]
	public List<ORAArcadianStack> Stack;
}

[Serializable]
[XmlRoot(ElementName = "image")]
public class ORAArcadianImage
{

	[XmlElement(ElementName = "stack")]
	public ORAArcadianStack Stack;

	[XmlAttribute(AttributeName = "h")]
	public int H;

	[XmlAttribute(AttributeName = "w")]
	public int W;
}