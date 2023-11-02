using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnttiStarterKit.Managers
{
    public abstract class ObjectPool<T> : MonoBehaviour where T : Component
    {
        [SerializeField]
        private T prefab;

        private Queue<T> pool;

        public T Get(bool activate = true)
        {
            if (pool == null)
                pool = new Queue<T>();

            if (!pool.Any())
                AddObjects(1);

            var obj = pool.Dequeue();
            obj.gameObject.SetActive(activate);

            return obj;
        }

        private void AddObjects(int count)
        {
            for(var i = 0; i < count; i++)
            {
                var obj = Instantiate(prefab);
                pool.Enqueue(obj);
            }
        }

        public void ReturnToPool(T obj)
        {
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}
