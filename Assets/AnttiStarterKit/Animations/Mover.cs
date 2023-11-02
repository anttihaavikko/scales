using UnityEngine;

namespace AnttiStarterKit.Animations
{
	public class Mover : MonoBehaviour {

		public float speed = 1f;
		public float offset;
		public bool noNegatives;
		public Vector3 direction = Vector3.up;

		[SerializeField] private bool randomizeOffset;

		private Vector3 _originalPosition;

		private void Start () {
			_originalPosition = transform.localPosition;

			if (randomizeOffset)
			{
				offset = Random.value * 100f;
			}
		}
		
		private void Update () {
			var sinVal = Mathf.Sin (Time.time * speed + offset * Mathf.PI);
			sinVal = noNegatives ? Mathf.Abs (sinVal) : sinVal;
			transform.localPosition = _originalPosition + direction * sinVal;
		}
	}
}
