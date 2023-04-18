using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Arcadian : MonoBehaviour
{
	[SerializeField] SkinnedMeshRenderer arcadianRenderer;

	public void ReplaceParts(ORARenderer.ArcadianParts parts)
	{
		ReplacePart(arcadianRenderer.materials[0], parts.Parts.Find(x => x.Location == "Skin"));
		ReplacePart(arcadianRenderer.materials[1], parts.Parts.Find(x => x.Location == "Eyes"));
		ReplacePart(arcadianRenderer.materials[2], parts.Parts.Find(x => x.Location == "Mouth"));
		ReplacePart(arcadianRenderer.materials[3], parts.Parts.Find(x => x.Location == "Top"));
		ReplacePart(arcadianRenderer.materials[4], parts.Parts.Find(x => x.Location == "Bottom"));
		ReplacePart(arcadianRenderer.materials[5], parts.Parts.Find(x => x.Location == "Right Hand"));
		ReplacePart(arcadianRenderer.materials[6], parts.Parts.Find(x => x.Location == "Left Hand"));
		ReplacePart(arcadianRenderer.materials[7], parts.Parts.Find(x => x.Location == "Head"));

		AssetDatabase.Refresh();
	}

	private void ReplacePart(Material material, ORARenderer.PartData partData)
	{
		Texture2D ogTex = null;
		byte[] fileData;
		if (File.Exists(partData.Src))
		{
			fileData = File.ReadAllBytes(partData.Src);
			ogTex = new Texture2D(2, 2);
			ogTex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
		}

		Texture2D newTex = ResizePart(ogTex, partData, 399, 399);

		byte[] testByte = newTex.EncodeToPNG();
		File.WriteAllBytes($"Assets/ORARenderer/{partData.Name}Test.png", testByte);

		material.mainTexture = newTex;
	}

	private Texture2D ResizePart(Texture2D source, ORARenderer.PartData partData, int targetWidth, int targetHeight)
	{
		Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
		UnityEngine.Color[] rpixels = result.GetPixels(0);

		float targetX = partData.Position.x;
		float targetY = partData.Position.y;

		int columnCounter = 0;
		int rowCounter = 0;
		int imgXCounter = 0;
		int imgYCounter = 0;


		for (int px = 0; px < rpixels.Length; px++)
		{
			if (columnCounter >= targetWidth)
			{
				columnCounter = 0;
				rowCounter++;
			}

			if (imgXCounter >= source.width)
			{
				imgXCounter = 0;
				imgYCounter++;
			}

			bool isXBlank = columnCounter < targetX || columnCounter >= targetX + source.width;
			bool isYBlank = rowCounter < targetY || rowCounter >= targetY + source.height;

			if (isXBlank || isYBlank)
			{
				rpixels[px] = UnityEngine.Color.clear;
			}
			else
			{
				rpixels[px] = source.GetPixel(imgXCounter, imgYCounter);
				imgXCounter++;
			}

			columnCounter++;
		}

		Debug.Log(partData.Name);
		Debug.Log($"sourceDimensions = {source.width}x{source.height} | targetLocation = {targetX},{targetY}");

		result.SetPixels(rpixels, 0);
		result.Apply();
		return result;
	}
}
