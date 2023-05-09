namespace ORARenderer
{
	using System;
	using System.Collections;
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

		public List<string> MaterialLocationNames { get { return materialLocationNames; } }

		public void RequestParts(ArcadianLoadRequest loadRequest) => StartCoroutine(RequestPartsCR(loadRequest));

		public void RequestPartsRandom() => StartCoroutine(RequestRandomCR());

		public bool IsLoading { get; private set; } = false;

		#region Private

		private ORAReader oraReader = null;

		private IEnumerator RequestPartsCR(ArcadianLoadRequest loadRequest)
		{
			if (loadRequest == null)
				yield break;

			if (oraReader == null)
				oraReader = ORAReader.GetInstance();

			if (oraReader == null)
			{
				Debug.LogWarning("ORA renderer could not instantiate!");
				yield break;
			}

			IsLoading = true;

			yield return new WaitUntil(() => oraReader.IsLoaded);

			ArcadianParts newParts = oraReader.GetPartData(loadRequest);
			ReplaceParts(newParts);

			IsLoading = false;
		}

		private IEnumerator RequestRandomCR()
		{
			if (oraReader == null)
				oraReader = ORAReader.GetInstance();

			if (oraReader == null)
			{
				Debug.LogWarning("ORA renderer could not instantiate!");
				yield break;
			}

			yield return new WaitUntil(() => oraReader.IsLoaded);

			ArcadianParts randomParts = oraReader.GetRandom();
			ReplaceParts(randomParts);
		}

		private void ReplaceParts(ArcadianParts newParts)
		{
			bool isErrorLogged = false;
			for (int i = 0; i < arcadianRenderer.materials.Length; i++)
			{
				if (i >= materialLocationNames.Count)
				{
					if (!isErrorLogged)
					{
						Debug.LogError(
							"Not all material locations are specified in ArcadianRenderer.materialLocationNames!" +
							" Add location names for all mats used by the SkinnedMeshRenderer to this List, and make sure they are the same order."
						);

						isErrorLogged = true;
					}

					LoadBlankPart(arcadianRenderer.materials[i]);
					continue;
				}

				else if (!oraReader.ArcadianReference.Locations.Exists(x => x.Name == materialLocationNames[i]))
				{
					LoadBlankPart(arcadianRenderer.materials[i]);
					continue;
				}

				Predicate<LocationData> match = x => x != null && x.Name == materialLocationNames[i];
				var defaultPart = oraReader.ArcadianReference.Locations.Find(match).Parts[0];

				if (newParts == null || newParts.Locations == null || !newParts.Locations.Exists(match))
				{
					ReplacePart(arcadianRenderer.materials[i], defaultPart);
					continue;
				}

				var location = newParts.Locations.Find(match);

				if (location.Parts.Count == 0)
					ReplacePart(arcadianRenderer.materials[i], defaultPart);
				else
					ReplacePart(arcadianRenderer.materials[i], location.Parts[0]);
			}
		}

		private void ReplacePart(Material material, PartData partData)
		{
			if (partData == null || partData.Src == null || partData.Src.Length == 0)
			{
				LoadBlankPart(material);
				return;
			}

			Texture2D sourceTex = new Texture2D(2, 2);
			sourceTex.LoadImage(partData.Src);

			material.mainTexture = ResizePart(sourceTex, partData, oraReader.ArcadianReference.canvasSizeW, oraReader.ArcadianReference.canvasSizeH);
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

		private void LoadBlankPart(Material material)
		{
			Texture2D blankTexture = new Texture2D(1, 1);

			Color[] pixels = blankTexture.GetPixels(0);
			for (int px = 0; px < pixels.Length; px++)
				pixels[px] = Color.clear;

			blankTexture.SetPixels(pixels, 0);
			blankTexture.Apply();

			material.mainTexture = blankTexture;
		}

		#endregion
	}

}
