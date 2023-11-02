using System.Globalization;
using UnityEngine;

namespace AnttiStarterKit.Utils
{
    public class TextUtils
    {
        public static string TextWith(string text, int size)
        {
            return "<size=" + size + ">" + text + "</size>";
        }

        public static string TextWith(string text, string color)
        {
            return "<color=" + color + ">" + text + "</color>";
        }

        public static string TextWith(string text, string color, float size)
        {
            return "<size=" + size + ">" + "<color=" + color + ">" + text + "</color></size>";
        }

        public static string TextWith(string text, Color color)
        {
            var hex = ColorUtility.ToHtmlStringRGB(color);
            return "<color=#" + hex + ">" + text + "</color>";
        }

        public static string TextWith(string text, Color color, int size)
        {
            var hex = ColorUtility.ToHtmlStringRGB(color);
            return "<size=" + size + ">" + "<color=#" + hex + ">" + text + "</color></size>";
        }

        public static string NumberWithSign(int number)
        {
            return number > 0 ? $"+{number}" : number.ToString();
        }
        
        public static string NumberWithSpaces(long number)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            return number.ToString("#,0", nfi);
        }
    }
}
