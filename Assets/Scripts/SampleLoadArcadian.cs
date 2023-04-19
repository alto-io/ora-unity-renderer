using UnityEngine;

public class SampleLoadArcadian : MonoBehaviour
{
	[SerializeField] private ORARenderer.ArcadianLoader arcadianLoader;

	[SerializeField] private string skin = "Orc";
	[SerializeField] private string eyes = "Eyepatch";
	[SerializeField] private string mouth = "Cigar";
	[SerializeField] private string top = "Mage Robe";
	[SerializeField] private string bottom = "Challenger Leggings";
	[SerializeField] private string rightHand = "Mage Book R";
	[SerializeField] private string leftHand = "Shield L";
	[SerializeField] private string head = "Black Bandana";

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
