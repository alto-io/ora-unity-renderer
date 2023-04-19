namespace ORARenderer
{
	using UnityEditor;
	using UnityEngine;

	public class ArcadianLoader : MonoBehaviour
	{
		[SerializeField] private SkinnedMeshRenderer arcadianRenderer;

		public void ReplaceParts(ArcadianParts parts)
		{
			ReplacePart(arcadianRenderer.materials[0], parts.Parts.Find(x => x.Location == PartLocation.Skin));
			ReplacePart(arcadianRenderer.materials[1], parts.Parts.Find(x => x.Location == PartLocation.Eyes));
			ReplacePart(arcadianRenderer.materials[2], parts.Parts.Find(x => x.Location == PartLocation.Mouth));
			ReplacePart(arcadianRenderer.materials[3], parts.Parts.Find(x => x.Location == PartLocation.Top));
			ReplacePart(arcadianRenderer.materials[4], parts.Parts.Find(x => x.Location == PartLocation.Bottom));
			ReplacePart(arcadianRenderer.materials[5], parts.Parts.Find(x => x.Location == PartLocation.RightHand));
			ReplacePart(arcadianRenderer.materials[6], parts.Parts.Find(x => x.Location == PartLocation.LeftHand));
			ReplacePart(arcadianRenderer.materials[7], parts.Parts.Find(x => x.Location == PartLocation.Head));

			AssetDatabase.Refresh();
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
					rPixels[px] = UnityEngine.Color.clear;
				}
			}

			result.SetPixels(rPixels, 0);
			result.Apply();
			return result;
		}

		#endregion
	}

}
