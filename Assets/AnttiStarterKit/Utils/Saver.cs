using UnityEngine;

namespace AnttiStarterKit.Utils
{
    public static class Saver 
    {
        private static string Key => Application.productName + " Save";
        
        public static void Save(object data, string keySuffix = "")
        {
            var json = JsonUtility.ToJson(data, true);
            PlayerPrefs.SetString(Key + keySuffix, json);
        }

        public static T Load<T>(string keySuffix = "") where T : class
        {
            if (!PlayerPrefs.HasKey(Key + keySuffix)) return null;
            
            var json = PlayerPrefs.GetString(Key + keySuffix);
            return JsonUtility.FromJson<T>(json);
        }

        public static bool Exists(string keySuffix = "")
        {
            return PlayerPrefs.HasKey(Key + keySuffix);
        }

        public static void Clear(string keySuffix = "")
        {
            if (!PlayerPrefs.HasKey(Key + keySuffix)) return;
            
            PlayerPrefs.DeleteKey(Key + keySuffix);
        }

        public static void Debug(string keySuffix = "")
        {
            UnityEngine.Debug.Log(PlayerPrefs.GetString(Key + keySuffix, "No data"));
        }

        public static T LoadOrCreate<T>(string keySuffix = "") where T : class, new()
        {
            return Exists(keySuffix) ? Load<T>(keySuffix) : new T();
        }
    }
}