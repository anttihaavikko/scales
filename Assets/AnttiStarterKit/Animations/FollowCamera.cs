using UnityEngine;

namespace AnttiStarterKit.Animations
{
	public class FollowCamera : MonoBehaviour {

		public Transform target;
		public float dampTime = 0.15f;
		public Vector3 offset = Vector3.zero;

		private Vector3 velocity = Vector3.zero;

		// Update is called once per frame
		void Update () 
		{
			if (target) {
				Vector3 point = Camera.main.WorldToViewportPoint(target.position);
				Vector3 delta = target.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
				Vector3 destination = transform.position + delta + new Vector3(offset.x * target.localScale.x, offset.y, offset.z);
				transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
			}

		}
	}
}