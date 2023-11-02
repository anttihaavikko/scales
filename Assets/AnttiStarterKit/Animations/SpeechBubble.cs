using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Managers;
using AnttiStarterKit.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace AnttiStarterKit.Animations
{
    public class SpeechBubble : MonoBehaviour
    {
        public Action onVocal, onWord;
        
        [SerializeField] private TMP_Text textArea;
        [SerializeField] private Color highlightColor = Color.red;
        [SerializeField] private float delayBetweenLetters = 0.02f;
        [SerializeField] private float delayBetweenWords = 0.05f;
        [SerializeField] private bool staticPlacing = true;
        [SerializeField] private SoundCollection toggleSound;

        private Appearer appearer;
        private string hex;
        private IEnumerator showHandle;
        private string message;
        private readonly string[] vocals = {"a", "e", "i", "o", "u", "y"};
        private bool done = true;
        private bool showing;
        private Queue<string> queue;

        private void Start()
        {
            queue = new Queue<string>();
            hex = ColorUtility.ToHtmlStringRGB(highlightColor);
            appearer = GetComponent<Appearer>();
        }

        public void Show(string text, bool force = false)
        {
            if (force)
            {
                queue.Clear();
                ShowNext(text);
                return;
            }
            
            if (!done && showing || queue.Any())
            {
                queue.Enqueue(text);
                return;
            }

            ShowNext(text);
        }

        private void ShowNext(string text)
        {
            done = false;
            CancelPrevious();
            message = text;
            showHandle = RevealText();
            StartCoroutine(showHandle);

            if (showing) return;

            PlaySound();
            appearer.Show();
            showing = true;
        }

        private void PlaySound()
        {
            if (toggleSound)
            {
                AudioManager.Instance.PlayEffectFromCollection(toggleSound, transform.position);
            }
        }

        private void Update()
        {
            if (!Input.anyKeyDown) return;
            SkipOrHide();
        }

        private void SkipOrHide()
        {
            if (done)
            {
                HideOrShowNext();
                return;
            }

            Skip();
        }

        private void HideOrShowNext()
        {
            if (queue.Any())
            {
                ShowNext(queue.Dequeue());
                return;
            }

            Hide();
            PlaySound();
        }

        private void Hide()
        {
            showing = false;
            appearer.Hide();
        }

        private void CancelPrevious()
        {
            if (showHandle != null)
            {
                StopCoroutine(showHandle);
            }
        }

        private string GetMessagePart(string message, int pos)
        {
            string msg = message.Substring (0, pos);

            var openCount = msg.Split('(').Length - 1;
            var closeCount = msg.Split(')').Length - 1;

            if (openCount > closeCount) {
                msg += ")";
            }

            var rest = message.Substring(pos).Replace("(", "").Replace(")", "");
            return staticPlacing ? $"{msg}<color=#00000000>{rest}</color>" : msg;
        }

        private string ApplyColors(string text)
        {
            return text.Replace("(", "<color=#" + hex + ">")
                .Replace(")", "</color>");
        }

        private IEnumerator RevealText()
        {
            var pos = 1;

            while (pos <= message.Length)
            {
                var text = GetMessagePart(message, pos);
                textArea.text = ApplyColors(text);
                var current = message.Substring(pos - 1, 1);
                CheckForVocal(current);
                CheckForWord(pos, current);
                var delay = current == " " ? delayBetweenWords : delayBetweenLetters;
                pos++;
                yield return new WaitForSeconds(delay);
            }

            RevealDone();
        }

        private void CheckForWord(int pos, string current)
        {
            if (pos == 0 || current == " ")
            {
                onWord?.Invoke();
            }
        }

        private void RevealDone()
        {
            done = true;
        }

        private void CheckForVocal(string current)
        {
            if (vocals.Contains(current))
            {
                onVocal?.Invoke();
            }
        }
        
        private void Skip()
        {
            CancelPrevious();
            textArea.text = ApplyColors(message);
            RevealDone();
        }
    }
}