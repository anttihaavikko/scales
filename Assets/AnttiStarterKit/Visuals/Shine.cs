using UnityEngine;

namespace AnttiStarterKit.Visuals
{
	public class Shine : MonoBehaviour {
		public float distance = 0.1f;
		public float maxEffectiveDistance = 5f;
		public Transform mirrorParent;
		public bool checkRotation;
		public Vector3 focus = Vector3.up * 10f;
		public bool reversed;

		private Vector3 originalPos;
		
		private void Start () {
			originalPos = transform.localPosition;
		}

		private void LateUpdate () {
			var t = transform;
			var direction = focus - t.position;
			direction.z = originalPos.z;
			direction.x = mirrorParent ? mirrorParent.localScale.x * direction.x : direction.x;

			if (checkRotation) {
				var parent = t.parent;
				var angle = parent.rotation.eulerAngles.z;
				var aMod = Mathf.Sign (parent.lossyScale.x);
				direction = Quaternion.Euler(new Vector3(0, 0, -angle * aMod)) * direction;
			}

			var step = direction.magnitude / maxEffectiveDistance;
			step = direction.magnitude < 0.3f ? 0 : step;
			var d = Mathf.Lerp(0, distance, step);
			var target = originalPos + (reversed ? -direction.normalized : direction.normalized) * d;
			transform.localPosition = Vector3.MoveTowards(originalPos, target, distance);
		}
	}
}
