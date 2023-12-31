using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using AnttiStarterKit.ScriptableObjects;
using AnttiStarterKit.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AnttiStarterKit.Animations
{
    public class ButtonStyle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private bool doScale;
        [SerializeField] private float scaleAmount;
        [SerializeField] private bool doRotation;
        [SerializeField] private float rotationAmount;
    
        [SerializeField] private List<Image> frontImages, bgImages;
        [SerializeField] private List<TMP_Text> texts;

        [SerializeField] private bool doColors;
    
        [SerializeField] private List<Color> backColors = new() { Color.black };
        [SerializeField] private List<Color> frontColors = new() { Color.white };

        [SerializeField] private SoundCollection clickSound, hoverSound;
        [SerializeField] private bool inScreenSpace;

        [SerializeField] private CursorChanger cursorChanger;
        [SerializeField] private int normalCursor = -1;
        [SerializeField] private int hoverCursor = -1;
        

        private Vector3 originalScale;
        private Color originalBackColor, originalFrontColor;

        private Camera cam;

        private void Start()
        {
            originalScale = transform.localScale;
            cam = Camera.main;
            SaveOriginalColors();
        }

        private Vector3 GetSoundPos()
        {
            return inScreenSpace ? cam.ScreenToWorldPoint(transform.position) : transform.position;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ApplyScaling(scaleAmount, TweenEasings.BounceEaseOut);
            ApplyRotation(Random.Range(-rotationAmount, rotationAmount), TweenEasings.BounceEaseOut);
            ApplyColors(backColors.Random(), frontColors.Random());
            DoSound(hoverSound);
            if(hoverCursor >= 0) cursorChanger.Change(hoverCursor);
        }
    
        private void ApplyScaling(float amount, Func<float, float> easing)
        {
            if (doScale)
            {
                Tweener.Instance.ScaleTo(transform, originalScale * (1f + amount), 0.2f, 0f, easing);
            }
        }

        private void ApplyRotation(float amount, Func<float, float> easing)
        {
            if (doRotation)
            {
                Tweener.Instance.RotateTo(transform, Quaternion.Euler(0, 0, amount), 0.2f, 0f, easing);
            }
        }

        private void ApplyColors(Color back, Color front)
        {
            if (!doColors) return;
            bgImages.ForEach(i => i.color = back);
            frontImages.ForEach(i => i.color = front);
            if (!texts.Any()) return;
            texts.ForEach(t => t.color = front);
        }

        private void SaveOriginalColors()
        {
            if (!doColors) return;
            
            bgImages.ForEach(i =>
            {
                originalBackColor = i.color;
            });

            frontImages.ForEach(i =>
            {
                originalFrontColor = i.color;
            });
            
            if (!texts.Any()) return;
            originalFrontColor = texts.First().color;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DoSound(hoverSound);
            Reset();
        }

        public void Reset()
        {
            ApplyScaling(0, TweenEasings.BounceEaseOut);
            ApplyRotation(0, TweenEasings.BounceEaseOut);
            ApplyColors(originalBackColor, originalFrontColor);
            if(normalCursor >= 0) cursorChanger.Change(normalCursor);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            DoSound(clickSound);
            Reset();
        }

        private void DoSound(SoundCollection sound)
        {
            if (!sound) return;
            var pos = GetSoundPos();
            AudioManager.Instance.PlayEffectFromCollection(sound, pos);
        }
    }
}
