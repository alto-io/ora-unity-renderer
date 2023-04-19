namespace ORARenderer
{
	using UnityEngine;

	public class ArcadianRenderer : MonoBehaviour
	{
		[SerializeField] private SkinnedMeshRenderer arcadianRenderer;

		public void ReplaceParts(ArcadianParts parts)
		{
			// materials should be the same order as PartLocation enum declaration
			for (int i = 0; i <= 7; i++)
				ReplacePart(arcadianRenderer.materials[i], parts.Parts.Find(x => x.Location == (PartLocation)i));
		}

		#region Private

		private void ReplacePart(Material material, PartData partData)
		{
			Texture2D sourceTex = new Texture2D(2, 2);
			sourceTex.LoadImage(partData.Src);

			Texture2D newTex = ResizePart(sourceTex, partData, 399, 399);

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
