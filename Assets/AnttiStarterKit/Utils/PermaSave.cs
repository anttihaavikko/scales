using System.Runtime.InteropServices;
using UnityEngine;

namespace AnttiStarterKit.Utils
{
    public class PermaSave : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void DoSetString(string key, string value);
        
        [DllImport("__Internal")]
        private static extern string DoGetString(string key, string defaultValue);
        
        [DllImport("__Internal")]
        private static extern void DoSetInt(string key, int value);
        
        [DllImport("__Internal")]
        private static extern int DoGetInt(string key, int defaultValue);
        
        [DllImport("__Internal")]
        private static extern bool DoHasKey(string key);
        
        public static void SetString(string key, string value)
        {
            DoSetString(key, value);
        }
        
        public static string GetString(string key, string defaultValue = null)
        {
            return DoGetString(key, defaultValue);
        }
        
        public static void SetInt(string key, int value)
        {
            DoSetInt(key, value);
        }
        
        public static int GetInt(string key, int defaultValue)
        {
            return DoGetInt(key, defaultValue);
        }
        
        public static bool HasKey(string key)
        {
            return DoHasKey(key);
        }
        
#else
        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
        
        public static string GetString(string key, string defaultValue = null)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
        
        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
        
        public static int GetInt(string key, int defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
#endif
        
    }
}