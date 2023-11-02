using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using TMPro;
using UnityEngine;

namespace AnttiStarterKit.Game
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private List<TMP_Text> valueFields, additionFields, multiFields;
        [SerializeField] private Appearer additionAppearer;

        [SerializeField] private float minSpeed = 0.3f;
        [SerializeField] private float maxSpeed = 3f;
        [SerializeField] private float additionShowTime = 2.5f;

        [SerializeField] private bool separateThousands = true;

        private List<Pulsater> multiPulsates = new();

        private int value;
        private float shownValue;
        private int addition;
        private int multiplier = 1;

        public int Total => value;
        public int Multi => multiplier;

        private void Start()
        {
            if (multiFields.Any())
            {
                multiPulsates = multiFields.Select(f => f.GetComponent<Pulsater>()).ToList();   
            }
        }

        public void Set(int amount, int multi = 1)
        {
            value = amount;
            multiplier = multi;
            shownValue = amount;
            valueFields.ForEach(f => f.text = Format(value));
            ShowMulti();
        }

        private string Format(int number)
        {
            return separateThousands ? number.AsScore() : number.ToString();
        }

        private void ShowMulti()
        {
            if (!multiFields.Any()) return;
            
            multiFields.ForEach(f => f.text = $"x{multiplier}");

            if (multiPulsates.Any())
            {
                multiPulsates.ForEach(p => p.Pulsate());
            }
        }

        private void Update()
        {
            if (Mathf.Abs(shownValue - value) < 0.1f) return;
            var speed = Mathf.Max(Mathf.Abs(value - shownValue) * Time.deltaTime * maxSpeed, minSpeed);
            shownValue = Mathf.MoveTowards(shownValue, value, speed);
            valueFields.ForEach(f => f.text = Format(Mathf.RoundToInt(shownValue)));
        }

        private string GetAdditionAsText()
        {
            var number = separateThousands ? addition.AsScore() : addition.ToString();
            return addition > 0 ? $"+{number}" : number;
        }

        public void Add(int amount, bool useMulti = true)
        {
            var amt = useMulti ? amount * multiplier : amount;

            if (amt < 0 && value + amt < 0)
            {
                amt = -value;
            }
        
            value += amt;
            addition += amt;

            if (additionAppearer)
            {
                additionFields.ForEach(f => f.text = GetAdditionAsText());
                additionAppearer.Show();   
            }

            CancelInvoke(nameof(ClearAddition));
            Invoke(nameof(ClearAddition), additionShowTime);
        }

        public void AddMulti(int amount = 1)
        {
            multiplier += amount;
            ShowMulti();
        }

        public void ResetMulti()
        {
            multiplier = 1;
            ShowMulti();
        }

        public void DecreaseMulti()
        {
            if (multiplier > 1)
            {
                multiplier--;
                ShowMulti();
            }
        }

        private void ClearAddition()
        {
            if (additionAppearer)
            {
                additionAppearer.Hide();
                addition = 0;   
            }
        }
    }
}