using UnityEngine;

namespace AnttiStarterKit.Randomizers
{
    public class RandomRotation : MonoBehaviour
    {
        [SerializeField] private float min = 0f;
        [SerializeField] private float max = 360f;
        
        
        private void Start()
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(min, max)));
        }
    }
}
