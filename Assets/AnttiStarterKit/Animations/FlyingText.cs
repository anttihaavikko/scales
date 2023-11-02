using UnityEngine;
using UnityEngine.UI;

namespace AnttiStarterKit.Animations
{
	public class FlyingText : MonoBehaviour {

		private Text text;
		private float alpha = 1f;
		private float speed = 1f;
		private float duration = 2f;
		private Vector2 startScale;

		void Start() {
			text = GetComponent<Text> ();
			startScale = text.transform.localScale;
		}

		void Update () {
			if (alpha > 0) {
				// change the y position
				Vector3 pos = transform.position;
				pos.y += speed * Time.deltaTime;
				transform.position = pos;

				// change alpha value
				alpha -= Time.deltaTime / duration;

				Color color = text.color;
				color.a = alpha;
				text.color = color;

				text.transform.localScale = startScale * (0.5f + 0.5f * alpha);

			} else {
				// destroy the game object if it's invisible
				Destroy(gameObject);
			}
		}
	}
}
