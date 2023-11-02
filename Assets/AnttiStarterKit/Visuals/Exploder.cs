using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using UnityEngine;

namespace AnttiStarterKit.Visuals
{
    public class Exploder : MonoBehaviour
    {
        [SerializeField] private List<int> effects;
        [SerializeField] private float shakeAmount = 0.5f;

        private EffectCamera cam;

        private void Start()
        {
            cam = Camera.main.GetComponent<EffectCamera>();
        }

        public void Explode()
        {
            if (effects.Any())
            {
                EffectManager.AddEffects(effects, transform.position);    
            }
            
            var distance = 1f / Mathf.Max(1f, (transform.position.WhereZ(0) - cam.transform.position.WhereZ(0)).magnitude);
            cam.BaseEffect(shakeAmount * distance);

            Destroy(gameObject);
        }
    }
}