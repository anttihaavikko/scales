using System;
using UnityEngine;

namespace AnttiStarterKit.Animations
{
    public class SpeechBubbleMouth : MonoBehaviour
    {
        [SerializeField] private SpeechBubble speechBubble;
        [SerializeField] private Vector3 closedScale;
        [SerializeField] private float openSpeed = 0.025f, closeSpeed = 0.02f, closeDelay = 0.05f;
        [SerializeField] private GameObject hideOnOpen;

        private Vector3 openScale;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
            
            var t = transform;
            openScale = t.localScale;
            t.localScale = closedScale;

            if (speechBubble)
            {
                speechBubble.onVocal += OpenMouth;   
            }
        }

        private void OpenMouth()
        {
            spriteRenderer.enabled = true;
            Tweener.ScaleToQuad(transform, openScale, openSpeed);
            CancelInvoke(nameof(CloseMouth));
            CancelInvoke(nameof(AfterClose));
            Invoke(nameof(CloseMouth), closeDelay);

            if (hideOnOpen)
            {
                hideOnOpen.SetActive(false);
            }
        }

        private void CloseMouth()
        {
            Tweener.ScaleToQuad(transform, openScale, closeSpeed);
            CancelInvoke(nameof(AfterClose));
            Invoke(nameof(AfterClose), closeSpeed);
        }

        private void AfterClose()
        {
            spriteRenderer.enabled = false;
            hideOnOpen.SetActive(true);
        }
    }
}