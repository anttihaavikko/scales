using UnityEngine;

namespace AnttiStarterKit.Managers
{
	public class SoundEffect : MonoBehaviour {

		public AudioSource audioSource;

		void Awake() {
			DontDestroyOnLoad (gameObject);
		}

		public void Play(AudioClip clip, float volume, bool pitchShift = true) {

			name = clip.name;

			if (pitchShift) {
				float targetPitch = 1f;
				audioSource.pitch = (1f + Random.Range (-0.2f, 0.2f)) * targetPitch;
			}

			audioSource.PlayOneShot (clip, AudioManager.Instance.volume * volume);

			Invoke ("DoDestroy", clip.length * 1.2f);
		}

		public void ChangeSpatialBlend(float blend, float min, float max) {
			audioSource.spatialBlend = blend;
			audioSource.minDistance = min;
			audioSource.maxDistance = max;
		}

		public void DoDestroy() {
			AudioManager.Instance.ReturnToPool(this);
		}
	}
}
