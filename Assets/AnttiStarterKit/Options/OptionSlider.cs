using System;
using UnityEngine;
using UnityEngine.UI;

namespace AnttiStarterKit.Options
{
    public class OptionSlider : MonoBehaviour
    {
        [SerializeField] private string optionKey;
        [SerializeField] private float defaultValue = 0.5f;
        [SerializeField] private float min, max = 1f;
        
        private Slider slider;

        private void Start()
        {
            slider = GetComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = PlayerPrefs.GetFloat(optionKey, defaultValue);
            
            slider.onValueChanged.AddListener(Changed);
        }

        private void Changed(float val)
        {
            PlayerPrefs.SetFloat(optionKey, val);
        }
    }
}