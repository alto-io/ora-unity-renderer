namespace ORARenderer
{
	using UnityEngine;

	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T instance;

		public static T GetInstance()
		{
			if (instance == null)
			{
				instance = FindObjectOfType<T>();
			}
			else if (instance != FindObjectOfType<T>())
			{
				Destroy(FindObjectOfType<T>());
			}

			return instance;
		}

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}