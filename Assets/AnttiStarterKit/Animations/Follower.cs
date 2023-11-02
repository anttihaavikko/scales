using UnityEngine;

namespace AnttiStarterKit.Animations
{
    public class Follower : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private void Update()
        {
            transform.position = target.position;
        }
    }
}