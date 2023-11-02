using UnityEngine;

namespace AnttiStarterKit.Managers
{
	public abstract class Manager<T> : MonoBehaviour where T : MonoBehaviour {
		private static T instance;
		public static T Instance => instance;

		private void Awake()
		{
			var component = gameObject.GetComponent<T>();
			
			if (instance != default && instance != component) {
				Destroy (gameObject);
				return;
			}

			instance = component;
		}
	}
}
