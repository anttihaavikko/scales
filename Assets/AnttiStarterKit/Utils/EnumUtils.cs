using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;

namespace AnttiStarterKit.Utils
{
    public static class EnumUtils
    {
        public static IEnumerable<T> ToList<T>()
        {
            var enumType = typeof (T);

            if (enumType.BaseType != typeof(Enum))
            {
                throw new ArgumentException("T must be of type System.Enum");
            }

            var enumValArray = Enum.GetValues(enumType);

            var enumValList = new List<T>(enumValArray.Length);
            enumValList.AddRange(from int val in enumValArray select (T)Enum.Parse(enumType, val.ToString()));

            return enumValList;
        }
        
        public static T Random<T>()
        {
            return ToList<T>().ToList().Random();
        }
    }
}