namespace ORARenderer
{
	using System.IO;
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

		private void ReplacePart(Material material, PartData partData)
		{
			Texture2D ogTex = null;
			byte[] fileData;
			if (File.Exists(partData.Src))
			{
				fileData = File.ReadAllBytes(partData.Src);
				ogTex = new Texture2D(2, 2);
				ogTex.LoadImage(fileData);
			}

			Texture2D newTex = ResizePart(ogTex, partData, 399, 399);

			material.mainTexture = newTex;
		}

		private Texture2D ResizePart(Texture2D source, PartData partData, int targetWidth, int targetHeight)
		{
			Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);

			UnityEngine.Color[] rPixels = result.GetPixels(0);
			UnityEngine.Color[] sPixels = source.GetPixels(0);
			int sPx = 0;

			Rect dstRect = new Rect(
				partData.PosX,
				(result.height - partData.PosY) - source.height,
				source.width,
				source.height
			);

			//int debugXMin = int.MaxValue;
			//int debugYMin = int.MaxValue;
			//int debugXMax = -1;
			//int debugYMax = -1;

			for (int px = 0; px < rPixels.Length; px++)
			{
				int x = px % targetWidth;
				int y = px / targetHeight;

				if (dstRect.Contains(new Vector2(x, y)))
				{
					rPixels[px] = sPixels[sPx];
					sPx++;

					//if (x < debugXMin) debugXMin = x;
					//else if (x > debugXMax) debugXMax = x;
					//if (y < debugYMin) debugYMin = y;
					//else if (y > debugYMax) debugYMax = y;
				}
				else
				{
					rPixels[px] = UnityEngine.Color.clear;
				}
			}

			//Debug.Log($"{partData.Name} + rect = {dstRect.x}, {dstRect.y}, {dstRect.xMax}, {dstRect.yMax}");
			//Debug.Log($"{source.width} x {source.height}");
			//Debug.Log($"{partData.Name} min = {debugXMin},{debugYMin} | maX = {debugXMax},{debugYMax}");

			result.SetPixels(rPixels, 0);
			result.Apply();
			return result;
		}
	}

}
