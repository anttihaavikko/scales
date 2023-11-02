using System.Globalization;
using AnttiStarterKit.Utils;

namespace AnttiStarterKit.Extensions
{
    public static class NumberExtensions
    {
        public static string AsScore(this int value)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            return value.ToString("#,0", nfi);
        }

        public static string WithSign(this int value)
        {
            return TextUtils.NumberWithSign(value);
        }
        
        public static float Remap(this float value, float valueRangeMin, float valueRangeMax, float newRangeMin, float newRangeMax)
        {
            return (value - valueRangeMin) / (valueRangeMax - valueRangeMin) * (newRangeMax - newRangeMin) + newRangeMin;
        }

        public static int LoopAround(this int value, int min, int max)
        {
            if (value < min) return max - 1;
            return value % max;
        }

        public static int AddWithLooping(this int value, int amount, int min, int max)
        {
            return (value + amount).LoopAround(min, max);
        }
    }
}