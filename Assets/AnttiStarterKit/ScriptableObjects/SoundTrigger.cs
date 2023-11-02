using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using UnityEngine;

namespace AnttiStarterKit.ScriptableObjects
{
    public class SoundTrigger : MonoBehaviour
    {
        [SerializeField] private SoundCollection sound;
        [SerializeField] private SoundComposition composition;
        [SerializeField] private float volume = 1f;

        public void Trigger()
        {
            var pos = transform.position.WhereZ(0);
            
            if (sound)
            {
                AudioManager.Instance.PlayEffectFromCollection(sound, pos, volume);
            }

            if (composition)
            {
                composition.Play(pos, volume);
            }
        }
    }
}