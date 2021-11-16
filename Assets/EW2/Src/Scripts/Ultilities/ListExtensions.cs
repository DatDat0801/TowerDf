using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MoreMountains.Tools
{
    /// <summary>
    /// List extensions
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Swaps two items in a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public static void MMSwap<T>(this IList<T> list, int i, int j)
        {
            var temporary = list[i];
            list[i] = list[j];
            list[j] = temporary;
        }

        /// <summary>
        /// Shuffles a list randomly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void MMShuffle<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list.MMSwap(i, Random.Range(i, list.Count));
            }
        }

        public static int Replace<T>(this IList<T> source, T oldValue, T newValue)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var index = source.IndexOf(oldValue);
            if (index != -1)
                source[index] = newValue;
            return index;
        }

        public static void ReplaceAll<T>(this IList<T> source, T oldValue, T newValue)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            int index = -1;
            do
            {
                index = source.IndexOf(oldValue);
                if (index != -1)
                    source[index] = newValue;
            } while (index != -1);
        }


        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, T oldValue, T newValue)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Select(x => EqualityComparer<T>.Default.Equals(x, oldValue) ? newValue : x);
        }

        /// <summary>
        /// Find an index of a first element that satisfies <paramref name="match"/>
        /// </summary>
        /// <typeparam name="T">Type of elements in the source collection</typeparam>
        /// <param name="source">This</param>
        /// <param name="match">Match predicate</param>
        /// <returns>Zero based index of an element. -1 if there is not such matches</returns>
        public static int IndexOf<T>(this IList<T> source, Predicate<T> match)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            for (int i = 0; i < source.Count; ++i)
                if (match(source[i]))
                    return i;

            return -1;
        }

        /// <summary>
        /// Replace the first occurance of an oldValue which satisfies the <paramref name="replaceByCondition"/> by a newValue
        /// </summary>
        /// <typeparam name="T">Type of elements of a target list</typeparam>
        /// <param name="source">Source collection</param>
        /// <param name="replaceByCondition">A condition which decides is a value should be replaced or not</param>
        /// <param name="newValue">A new value instead of replaced</param>
        /// <returns>This</returns>
        public static IList<T> Replace<T>(this IList<T> source, Predicate<T> replaceByCondition, T newValue)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (replaceByCondition == null)
            {
                throw new ArgumentNullException(nameof(replaceByCondition));
            }


            int index = source.IndexOf(replaceByCondition);
            if (index != -1)
                source[index] = newValue;

            return source;
        }

        /// <summary>
        /// Replace all occurance of values which satisfy the <paramref name="replaceByCondition"/> by a newValue
        /// </summary>
        /// <typeparam name="T">Type of elements of a target list</typeparam>
        /// <param name="source">Source collection</param>
        /// <param name="replaceByCondition">A condition which decides is a value should be replaced or not</param>
        /// <param name="newValue">A new value instead of replaced</param>
        /// <returns>This</returns>
        public static IList<T> ReplaceAll<T>(this IList<T> source, Predicate<T> replaceByCondition, T newValue)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (replaceByCondition == null)
            {
                throw new ArgumentNullException(nameof(replaceByCondition));
            }

            for (int i = 0; i < source.Count; ++i)
                if (replaceByCondition(source[i]))
                    source[i] = newValue;

            return source;
        }
    }
}