using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TinyCacto.Utils
{
    public static class TinyUtils 
    {
        public static IEnumerable<string> Uniquifier(this IEnumerable<string> values)
        {
            if (values == null) throw new ArgumentNullException("values");

            var unique = new HashSet<string>();

            foreach (var item in values)
            {
                var newItem = item;

                int count = 0;

                while (unique.Contains(newItem))
                {
                    count++;
                }

                newItem += $"({count})";

                unique.Add(newItem);

                yield return newItem;
            }
        }

        public static T RandomContent<T>(this T[] array)
        {
            if (array == null)
                return default(T);

            return array[Random.Range(0, array.Length)];
        }

        public static T RandomContent<T>(this List<T> list)
        {
            if (list == null)
                return default(T);

            return list[Random.Range(0, list.Count)];
        }
        public static T2 RandomValue<T1, T2>(this Dictionary<T1,T2> dictionary)
        {
            if (dictionary == null)
                return default(T2);

            return dictionary.Values.ElementAt(Random.Range(0, dictionary.Count));
        }

        public static int RandomKey<T1, T2>(this Dictionary<T1, T2> dictionary)
        {
            if (dictionary == null)
                return 0;

            return Random.Range(0, dictionary.Count);
        }
    }
}