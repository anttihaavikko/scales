using UnityEngine;
using UnityEngine.Rendering;

namespace AnttiStarterKit.Visuals
{
    public class AutoSpriteOrder : MonoBehaviour
    {
        [SerializeField] private int offset;
        [SerializeField] private Transform target;

        private SortingGroup _group;

        private void Start()
        {
            _group = GetComponent<SortingGroup>();
        }

        private void Update()
        {
            if (_group)
            {
                _group.sortingOrder = -Mathf.RoundToInt(target.position.y * 10) + offset;
            }
        }

        public void SetOffset(int o)
        {
            offset = o;
        }
    }
}
