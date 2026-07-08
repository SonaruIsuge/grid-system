using System;

namespace SonaruUtilities
{
    public static class Extensions
    {
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) 
                throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

            T[] arr = (T[])Enum.GetValues(src.GetType());
            var j = Array.IndexOf(arr, src) + 1;
            return (arr.Length==j) ? arr[0] : arr[j];
        }
    }
}