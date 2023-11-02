using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnttiStarterKit.Utils
{
    public class Tutorial<T> where T : struct, IConvertible
    {
        public Action<T> onShow;
        
        private readonly string key;
        private TutorialData data;

        public Tutorial(string key)
        {
            if (!typeof(T).IsEnum) 
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            
            this.key = key;
            Load();
        }

        public bool IsSeen(T message)
        {
            return data.seen.Contains(message);
        }

        public void Show(T message)
        {
            if (data.seen.Contains(message)) return;
            
            onShow?.Invoke(message);
            Mark(message);
        }

        public void Mark(T message)
        {
            data.seen.Add(message);
            Saver.Save(data, key);
        }

        public void Clear()
        {
            Saver.Clear(key);
            data = new TutorialData();
        }

        private void Load()
        {
            if (Saver.Exists(key))
            {
                data = Saver.Load<TutorialData>(key);
                return;
            }
            
            data = new TutorialData();
        }

        [Serializable]
        internal class TutorialData
        {
            public List<T> seen;

            public TutorialData()
            {
                seen = new List<T>();
            }
        }
    }
}