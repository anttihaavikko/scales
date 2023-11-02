using System.Text.RegularExpressions;
using AnttiStarterKit.Utils;
using UnityEngine;

namespace AnttiStarterKit.Extensions
{
    public static class StringExtensions
    {
        public static string Color(this string text, Color color)
        {
            return TextUtils.TextWith(text, color);
        }
        
        public static string Size(this string text, int size)
        {
            return TextUtils.TextWith(text, size);
        }
        
        public static string Style(this string text, Color color, int size)
        {
            return TextUtils.TextWith(text, color, size);
        }
        
        public static int NthIndexOf(this string target, string value, int n)
        {
            var m = Regex.Match(target, "((" + Regex.Escape(value) + ").*?){" + n + "}");

            if (m.Success)
                return m.Groups[2].Captures[n - 1].Index;
            
            return -1;
        }
    }
}