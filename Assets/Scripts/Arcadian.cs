using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Arcadian : MonoBehaviour
{
	[SerializeField] SkinnedMeshRenderer renderer;

	public void ReplaceParts(ORARenderer.ArcadianParts parts)
	{
		ReplacePart(renderer.materials[0], parts.Parts.Find(x => x.Location == "Skin"));
		ReplacePart(renderer.materials[1], parts.Parts.Find(x => x.Location == "Eyes"));
		ReplacePart(renderer.materials[2], parts.Parts.Find(x => x.Location == "Mouth"));
		ReplacePart(renderer.materials[3], parts.Parts.Find(x => x.Location == "Top"));
		ReplacePart(renderer.materials[4], parts.Parts.Find(x => x.Location == "Bottom"));
		ReplacePart(renderer.materials[5], parts.Parts.Find(x => x.Location == "Right Hand"));
		ReplacePart(renderer.materials[6], parts.Parts.Find(x => x.Location == "Left Hand"));
		ReplacePart(renderer.materials[7], parts.Parts.Find(x => x.Location == "Head"));
	}

	private void ReplacePart(Material material, ORARenderer.PartData partData)
	{
		byte[] imageByteArray = null;
		FileStream fileStream = new FileStream(partData.Src, FileMode.Open, FileAccess.Read);
		using (BinaryReader reader = new BinaryReader(fileStream))
		{
			imageByteArray = new byte[reader.BaseStream.Length];
			for (int i = 0; i < reader.BaseStream.Length; i++)
                    imageByteArray[i] = reader.ReadByte();
		};

		Texture2D newTex = new Texture2D(1,1);
		newTex.LoadImage(imageByteArray);
		material.SetTexture(partData.Name, newTex);
	}
}
