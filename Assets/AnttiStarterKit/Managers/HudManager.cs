using UnityEngine;
using UnityEngine.UI;

namespace AnttiStarterKit.Managers
{
	public class HudManager : MonoBehaviour {
		public Text infoDisplay;

		private float messageAlpha = 0;
		private float deltaTime = 0f;

		public Transform worldCanvas;
		public Text flyingText;

		private static HudManager instance = null;
		public static HudManager Instance {
			get { return instance; }
		}

		void Awake() {
			if (instance != null && instance != this) {
				Destroy (this.gameObject);
				return;
			} else {
				instance = this;
			}
		}

		void Update() {
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
			messageAlpha = Mathf.MoveTowards (messageAlpha, 0, Time.deltaTime / Time.timeScale);

			if (messageAlpha <= 1f) {
				infoDisplay.color = new Color (1, 1, 1, messageAlpha);
			}
		}

		public void DisplayMessage(string msg, float delay = 1f) {
			if(infoDisplay) {
				infoDisplay.text = msg;
				infoDisplay.color = Color.white;
				messageAlpha = 1f + delay;
			} else {
				Debug.Log("Message display area not set!");
			}
		}

		public void ShowStatus(float x, float y, string str, Color c) {
			Text t = Instantiate (flyingText, new Vector3 (x, y + 1f, 0), Quaternion.identity);
			t.color = c;
			t.text = str;
			t.transform.SetParent (worldCanvas, false);
		}
	}
}
