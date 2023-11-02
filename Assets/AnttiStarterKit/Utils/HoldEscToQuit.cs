using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace AnttiStarterKit.Utils
{
    public class HoldEscToQuit : MonoBehaviour
    {
        public Vector3 hiddenSize = Vector3.zero;
        public float speed = 0.3f;
        public UnityEvent onQuit;
        public Transform bar;

        private Vector3 targetSize;
        private float escHeldFor;
        
        private void Start()
        {
            var t = transform;
            targetSize = t.localScale;
            t.localScale = hiddenSize;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Tweener.Instance.ScaleTo(transform, targetSize, speed, 0f, TweenEasings.BounceEaseOut);
                DoSound();
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (bar)
                {
                    bar.localScale = Vector3.zero;   
                }
                
                escHeldFor = 0f;
            }

            if (Input.GetKey(KeyCode.Escape))
            {
                escHeldFor += Time.deltaTime;
                
                if (bar)
                {
                    bar.localScale = new Vector3(escHeldFor / 1.5f, 1f, 1f);
                }
                
                CancelInvoke(nameof(HideText));
                Invoke(nameof(HideText), 2f);
            }

            if(escHeldFor > 1.5f)
            {
                escHeldFor = 0;
                
                if (onQuit != null)
                {
                    onQuit.Invoke();
                    return;
                }
                
                Debug.Log("Quit");
                Application.Quit();
            }
        }

        private void HideText()
        {
            Tweener.Instance.ScaleTo(transform, hiddenSize, speed, 0f, TweenEasings.QuarticEaseIn);
            DoSound();
        }

        private void DoSound()
        {
            // AudioManager.Instance.PlayEffectAt(25, transform.position, 0.5f);
            // AudioManager.Instance.PlayEffectAt(1, transform.position, 0.75f);
        }
    }
}
