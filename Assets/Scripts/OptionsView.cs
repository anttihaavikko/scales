using System.Collections;
using System.Collections.Generic;
using AnttiStarterKit.Managers;
using AnttiStarterKit.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsView : MonoBehaviour
{
	public Slider musicSlider, soundSlider;
	public RectTransform options;
	[SerializeField] private SoundCollection slideSound, sampleSound;

	private bool optionsOpen = false;
	private bool canQuit = false;

	private void Start()
	{
		soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
		musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
	}

	private void DoInputs()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			if (optionsOpen) {
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

		var optionsX = optionsOpen ? 0f : options.sizeDelta.x;
		options.anchoredPosition = Vector2.Lerp(options.anchoredPosition, new Vector2(optionsX, 0f), Time.deltaTime * 10f);
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
