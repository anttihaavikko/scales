using System.Collections.Generic;
using UnityEngine;

namespace AnttiStarterKit.Utils
{
    [CreateAssetMenu(fileName = "Cursor Changer", menuName = "Cursor Changer", order = 0)]
    public class CursorChanger : ScriptableObject
    {
        [SerializeField] private List<CursorData> cursors;

        public void Change(int to)
        {
            var cursor = cursors[to];
            Cursor.SetCursor(cursor.cursor, cursor.hotspot, CursorMode.Auto);
        }
    }

    [System.Serializable]
    public class CursorData
    {
        public Texture2D cursor;
        public Vector2 hotspot;
    }
}