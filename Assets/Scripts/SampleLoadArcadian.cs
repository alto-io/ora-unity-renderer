using UnityEngine;

public class SampleLoadArcadian : MonoBehaviour
{
	[SerializeField] private ORARenderer.ArcadianRenderer arcadianLoader;

	[SerializeField] private string skin = "Metal";
	[SerializeField] private string eyes = " Icy Stare";
	[SerializeField] private string mouth = "Uwu Mouth";
	[SerializeField] private string top = "Sci-Fi Armor";
	[SerializeField] private string bottom = "Sci-Fi  Pants";
	[SerializeField] private string rightHand = "Glowing Hands R";
	[SerializeField] private string leftHand = "Gowing Hands L";
	[SerializeField] private string head = "AR Goggles";

	public void Start()
	{
		var loadRequest = new ORARenderer.ArcadianPartLoadRequest
		{
			Skin = skin,
			Eyes = eyes,
			Mouth = mouth,
			Top = top,
			Bottom = bottom,
			RightHand = rightHand,
			LeftHand = leftHand,
			Head = head
		};

		var oraReader = ORARenderer.ORAReader.GetInstance();
		var arcadianParts = oraReader.GetPartData(loadRequest);
		arcadianLoader.ReplaceParts(arcadianParts);
	}
}
