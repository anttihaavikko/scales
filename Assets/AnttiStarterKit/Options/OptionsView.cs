using AnttiStarterKit.Managers;
using AnttiStarterKit.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace AnttiStarterKit.Options
{
	public class OptionsView : MonoBehaviour
	{
		public Slider musicSlider, soundSlider;
		[SerializeField] private SoundCollection slideSound, sampleSound;

		private bool optionsOpen;
		private RectTransform panel;

		private void Start()
		{
			panel = GetComponent<RectTransform>();
			
			soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
			musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
		}

		private void DoInputs()
		{
			if (Input.GetKeyDown (KeyCode.Escape))
			{
				if (optionsOpen)
				{
					ToggleOptions (false);
				}
				
				return;
			}

			if (Input.GetKeyDown (KeyCode.O) || Input.GetKeyDown (KeyCode.P)) {
				ToggleOptions ();
			}
		}
	
		private void Update ()
		{
			DoInputs ();

			var optionsX = optionsOpen ? 0f : panel.sizeDelta.x;
			panel.anchoredPosition = Vector2.Lerp(panel.anchoredPosition, new Vector2(optionsX, 0f), Time.deltaTime * 10f);
		}

		public void ChangeMusicVolume()
		{
			AudioManager.Instance.ChangeMusicVolume(musicSlider.value);
			AudioManager.Instance.SaveVolumes();
		}

		public void ChangeSoundVolume()
		{
			if (!(Mathf.Abs(soundSlider.value - AudioManager.Instance.volume) > 0.05f)) return;
			AudioManager.Instance.volume = soundSlider.value;
			AudioManager.Instance.PlayEffectFromCollection(sampleSound, Vector3.zero, 0.5f);
			AudioManager.Instance.SaveVolumes();
		}

		public void ToggleOptions()
		{
			ToggleOptions(!optionsOpen);
		}

		public void ToggleOptions(bool state)
		{
			AudioManager.Instance.PlayEffectFromCollection(slideSound, Vector3.zero, 0.5f);
			optionsOpen = state;
		}
	}
}
