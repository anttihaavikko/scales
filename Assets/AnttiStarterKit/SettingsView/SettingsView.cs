using AnttiStarterKit.Managers;
using AnttiStarterKit.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace AnttiStarterKit.SettingsView
{
	public class SettingsView : MonoBehaviour
	{
		[SerializeField] private Slider musicSlider;
		[SerializeField] private Slider soundSlider;
		[SerializeField] private SoundCollection changeSound;
		[SerializeField] private float soundChangeThreshold = 0.05f;

		private RectTransform options;

		private void Start()
		{
			soundSlider.value = AudioManager.Instance.SoundVolume;
			musicSlider.value = AudioManager.Instance.MusicVolume;
		
			musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
			soundSlider.onValueChanged.AddListener(ChangeSoundVolume);
		}

		private static void CheckInputs()
		{
			if (Input.GetKeyDown (KeyCode.Escape))
			{
				SceneChanger.Instance.ChangeScene("Start");
			}
		}

		private static void ChangeMusicVolume(float vol)
		{
			AudioManager.Instance.ChangeMusicVolume(vol);
		}

		private void ChangeSoundVolume(float vol)
		{
			AudioManager.Instance.ChangeSoundVolume(vol);
			if (!(Mathf.Abs(vol - AudioManager.Instance.SoundVolume) > soundChangeThreshold)) return;
			AudioManager.Instance.PlayEffectFromCollection(changeSound, Vector3.zero);
		}
	}
}
