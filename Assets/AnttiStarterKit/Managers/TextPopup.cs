using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace AnttiStarterKit.Managers
{
    public class TextPopup : MonoBehaviour
    {
        [SerializeField] private List<TMP_Text> texts;
        [SerializeField] private SortingGroup sortingGroup;

        private Animator anim;
        private int defaultState;
        private float duration = 5f;

        private void Awake()
        {
            anim = GetComponent<Animator>();

            if (!anim) return;
            // var info = anim.GetNextAnimatorStateInfo(-1);
            // defaultState = info.shortNameHash;
            // duration = info.length;
        }

        public void Play(string content, int depth)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-3f, 3f)));
            anim.speed = Random.Range(0.9f, 1.1f);
            sortingGroup.sortingOrder = depth;
            texts.ForEach(t => t.text = content);
            Invoke(nameof(Done), duration);

            if (!anim) return;
            anim.Play(defaultState, -1, 0);
        }

        private void Done()
        {
            EffectManager.Instance.ReturnToPool(this);
        }
    }
}