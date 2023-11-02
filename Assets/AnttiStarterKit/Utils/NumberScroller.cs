using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using TMPro;
using UnityEngine;

namespace AnttiStarterKit.Utils
{
    public class NumberScroller : MonoBehaviour
    {
        [SerializeField] private TMP_Text valueField, additionField;
        [SerializeField] private Appearer additionAppearer;

        [SerializeField] private float minSpeed = 0.3f;
        [SerializeField] private float maxSpeed = 3f;
        [SerializeField] private float additionShowTime = 2.5f;

        private int _value;
        private float _shownValue;
        private int _addition;

        private void Update()
        {
            if (Mathf.Abs(_shownValue - _value) < 0.1f) return;
            var speed = Mathf.Max(Mathf.Abs(_value - _shownValue) * Time.deltaTime * maxSpeed, minSpeed);
            _shownValue = Mathf.MoveTowards(_shownValue, _value, speed);
            valueField.text = Mathf.RoundToInt(_shownValue).ToString();
        }

        public void Add(int amount)
        {
            _value += amount;
            _addition += amount;

            additionField.text = _addition.WithSign();
            additionAppearer.Show();
            
            CancelInvoke(nameof(ClearAddition));
            Invoke(nameof(ClearAddition), additionShowTime);
        }

        private void ClearAddition()
        {
            additionAppearer.Hide();
            _addition = 0;
        }

        public void SetValue(int amount)
        {
            _value = amount;
            _shownValue = amount;
            valueField.text = _value.ToString();
        }
    }
}