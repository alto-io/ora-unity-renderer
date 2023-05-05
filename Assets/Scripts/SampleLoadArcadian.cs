using UnityEngine;

public class SampleLoadArcadian : MonoBehaviour
{
	[SerializeField] private ORARenderer.ArcadianRenderer arcadianRenderer;

	[SerializeField] bool isRandom = false;
	[SerializeField] ORARenderer.ArcadianLoadRequest loadRequest = new ORARenderer.ArcadianLoadRequest();

	public void Start()
	{
		if (isRandom)
			arcadianRenderer.RequestPartsRandom();
		else
			arcadianRenderer.RequestParts(loadRequest);
	}
}
