using UnityEngine;

namespace AnttiStarterKit.Animations
{
	public class Rotator : MonoBehaviour {

		public float speed = 1f;
		public float pulsingSpeed, pulsingMin;
		public bool randomizeDirection;

		private float angle;
		private float dir = 1f;

		private void Start()
		{
			dir = randomizeDirection ? 1f : -1f;
			angle = Random.value * 360f;
		}
		
		private void Update ()
		{
			var mod = pulsingSpeed > 0 ? Mathf.Abs(Mathf.Sin(Time.time * pulsingSpeed)) + pulsingMin : 1f;
			angle -= speed * Time.deltaTime * 60f * mod * dir;
			transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, angle));
		}

		public void ChangeSpeed(float s) {
			speed = s;
		}
	}
}
