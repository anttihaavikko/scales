using System;
using UnityEngine;

namespace AnttiStarterKit.Utils
{
    public class DevSpeedControl : MonoBehaviour
    {
        [SerializeField] private float fast = 10f;
        [SerializeField] private float slow = 0.1f;
        [SerializeField] private bool isToggle;
        [SerializeField] private KeyCode fastKey = KeyCode.Tab;
        [SerializeField] private KeyCode slowKey = KeyCode.LeftShift;


        private bool state;

        private void Update()
        {
            if (!Application.isEditor) return;

            if (isToggle)
            {
                if (state && Input.GetKey(fastKey) || Input.GetKey(slowKey))
                {
                    Time.timeScale = 1f;
                    state = false;
                    return;
                }
                
                if (Input.GetKey(fastKey))
                {
                    Time.timeScale = fast;
                    state = true;
                }
                
                if (Input.GetKey(slowKey))
                {
                    Time.timeScale = slow;
                    state = true;
                }

                return;
            }
            
            var speed = 1f;
            if (Input.GetKey(fastKey)) speed = fast;
            if (Input.GetKey(slowKey)) speed = slow;
            Time.timeScale = speed;
        }
    }
}