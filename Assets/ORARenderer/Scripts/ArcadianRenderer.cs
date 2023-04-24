namespace ORARenderer
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public class ArcadianRenderer : MonoBehaviour
	{
		[SerializeField] private SkinnedMeshRenderer arcadianRenderer;

		/// <summary>
		/// Ordered the same as the materials of the SkinnedMeshRenderer 
		/// </summary>
		[SerializeField] private List<string> materialLocationNames = new List<string>()
		{
			"Skin",
			"Eyes",
			"Mouth",
			"Top",
			"Bottom",
			"Right Hand",
			"Left Hand",
			"Head"
		};

		public void RequestParts(ArcadianLoadRequest loadRequest)
		{
			if (loadRequest == null)
				return;

			if (oraReader == null)
				oraReader = ORAReader.GetInstance();

			ArcadianParts newParts = oraReader.GetPartData(loadRequest);
			ReplaceParts(newParts);
		}

		public void RequestPartsRandom()
		{
			if (oraReader == null)
				oraReader = ORAReader.GetInstance();

			ArcadianParts randomParts = oraReader.GetRandom();
			ReplaceParts(randomParts);
		}

		#region Private

		private ORAReader oraReader = null;

		private void ReplaceParts(ArcadianParts newParts)
		{
			for (int i = 0; i < arcadianRenderer.materials.Length; i++)
			{
				if (i >= materialLocationNames.Count)
				{
					Debug.LogError(
						"Not all material locations are specified in ArcadianRenderer.materialLocationNames!" +
						" Add location names for all mats used by the SkinnedMeshRenderer to this List, and make sure they are the same order."
					);
					return;
				}

				Predicate<LocationData> match = x => x != null && x.Name == materialLocationNames[i];

				if (!newParts.Locations.Exists(match))
					continue;

				var location = newParts.Locations.Find(match);

				if (location.Parts.Count == 0)
					continue;

				ReplacePart(arcadianRenderer.materials[i], location.Parts[0]);
			}
		}

		private void ReplacePart(Material material, PartData partData)
		{
			if (partData == null)
				return;

			Texture2D sourceTex = new Texture2D(2, 2);
			sourceTex.LoadImage(partData.Src);

			Texture2D newTex = ResizePart(sourceTex, partData, oraReader.ArcadianReference.canvasSizeW, oraReader.ArcadianReference.canvasSizeH);

			material.mainTexture = newTex;
		}

		private Texture2D ResizePart(Texture2D source, PartData partData, int targetWidth, int targetHeight)
		{
			Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);

			Color[] rPixels = result.GetPixels(0);
			Color[] sPixels = source.GetPixels(0);
			int sPx = 0;

			Rect dstRect = new Rect(
				partData.PosX,
				(result.height - partData.PosY) - source.height,
				source.width,
				source.height
			);

			for (int px = 0; px < rPixels.Length; px++)
			{
				int x = px % targetWidth;
				int y = px / targetHeight;

				if (dstRect.Contains(new Vector2(x, y)))
				{
					rPixels[px] = sPixels[sPx];
					sPx++;
				}
				else
				{
					rPixels[px] = Color.clear;
				}
			}

			result.SetPixels(rPixels, 0);
			result.Apply();
			return result;
		}

		#endregion
	}

}
