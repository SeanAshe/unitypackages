using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace Custom
{
    public static class CollectionsExtension
    {
        /// <summary>
        /// 判断索引是否合法
        /// </summary>
        static public bool IsIndexValid<T>(this IEnumerable<T> collection, int index)
        {
            if (collection == null || collection.IsNullorEmpty())
                return false;
            else
                return index >= 0 && index < collection.Count();
        }
        static public bool IsNullorEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || collection.Count() <= 0;
        }
        /// <summary>
        /// 洗牌算法 会改变原有集合顺序
        /// </summary>
        /// <returns></returns>
        static public void Shuffle<T>(this IList<T> @this, in System.Random random = null)
        {
            if (@this.IsNullorEmpty()) return;
            var length = @this.Count;
            var maxi = length - 2;
            for (var i = 0; i <= maxi; i++)
            {
                var tmp = random == null ? UnityEngine.Random.Range(0, length) : random.Next(0, length);
                (@this[i], @this[tmp]) = (@this[tmp], @this[i]);
            }
        }
        /// <summary>
        /// 洗牌算法 不改变原有集合顺序
        /// </summary>
        /// <returns></returns>
        static public IEnumerable<T> ShuffleReturn<T>(this IReadOnlyCollection<T> @this, in System.Random random = null)
        {
            if (@this.IsNullorEmpty()) return null;
            T[] shuffled = new T[@this.Count];
            Array.Copy(@this.ToArray(), shuffled, @this.Count);
            shuffled.Shuffle(random);
            return shuffled;
        }

        public static IEnumerable<(int index, T value)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((value, index) => (index, value));
        }
        public static void ForEach<T>(this IReadOnlyCollection<T> sequence, in Action<T> action)
        {
            foreach (T item in sequence)
                action?.Invoke(item);
        }
        public static void ForEach<T>(this IReadOnlyCollection<T> sequence, in Action<int, T> action)
        {
            foreach (var (index, item) in sequence.WithIndex())
                action?.Invoke(index, item);
        }
        public static IEnumerable<T> ZipLongest<T1, T2, T>(this IReadOnlyCollection<T1> first, IReadOnlyCollection<T2> second, Func<T1, T2, T> operation)
        {
            using (var iter1 = first.GetEnumerator())
            using (var iter2 = second.GetEnumerator())
            {
                while (iter1.MoveNext())
                    yield return operation(iter1.Current, iter2.MoveNext() ? iter2.Current : default);

                while (iter2.MoveNext())
                    yield return operation(default, iter2.Current);
            }
        }
        /// <summary>
        /// 去重
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, TKey>(this IReadOnlyCollection<T> sequence, Func<T, TKey> validator)
        {
            return sequence.Distinct(new EqualityComparer<T, TKey>(validator));
        }

        private class EqualityComparer<T, TKey> : IEqualityComparer<T>
        {
            Func<T, TKey> _validator;

            public EqualityComparer(Func<T, TKey> validator) => _validator = validator;
            public bool Equals(T x, T y) => _validator(x).Equals(_validator(y));
            public int GetHashCode(T obj) => _validator(obj).GetHashCode();
        }
        public static bool TryGetValue(this Hashtable @this, object key, out object value)
        {
            if (@this.ContainsKey(key))
            {
                value = @this[key];
                return true;
            }
            value = null;
            return false;
        }
        public static bool TryGetValue<T>(this Hashtable @this, object key, out T value)
        {
            if (@this.TryGetValue(key, out var unconvertedValue) && unconvertedValue is T)
            {
                value = (T)unconvertedValue;
                return true;
            }
            value = default;
            return false;
        }
        /// <summary>
        /// 排序（替换）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="weightGetter"></param>
        public static void CSort<T>(this IList<T> @this, bool asc = true, SortMethod mod = SortMethod.Quick) where T : IComparable
            => CSort(@this, x => x, asc, mod);
        /// <summary>
        /// 排序（替换）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="weightGetter"></param>
        public static void CSort<T>(this IList<T> @this, Func<T, IComparable> weightGetter, bool asc = true, SortMethod mod = SortMethod.Quick)
        {
            switch (mod)
            {
                case SortMethod.Quick:
                    @this.Sort_Quick_iterable(weightGetter, asc);
                    break;
                case SortMethod.Heap:
                    @this.Sort_Heap(weightGetter, asc);
                    break;
                case SortMethod.Quick_recursive:
                    @this.Sort_Quick_recursive(weightGetter, asc);
                    break;
                case SortMethod.Shell:
                    @this.Sort_Shell(weightGetter, asc);
                    break;
                case SortMethod.Bubble:
                    @this.Sort_Bubble(weightGetter, asc);
                    break;
            }
        }
        /// <summary>
        /// 排序（out 输出）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="weightGetter"></param>
        public static void CSort<T>(this IList<T> @this, out IList<T> res, bool asc = true, SortMethod mod = SortMethod.Quick) where T : IComparable
            => CSort(@this, x => x, out res, asc, mod);
        /// <summary>
        /// 排序（out 输出）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="weightGetter"></param>
        public static void CSort<T>(this IList<T> @this, Func<T, IComparable> weightGetter, out IList<T> res, bool asc = true, SortMethod mod = SortMethod.Quick)
        {
            var source = @this.ToArray();
            var copy = new T[@this.Count];
            Array.Copy(@source, copy, @this.Count);
            switch (mod)
            {
                case SortMethod.Quick:
                    copy.Sort_Quick_iterable(weightGetter, asc);
                    break;
                case SortMethod.Heap:
                    copy.Sort_Heap(weightGetter, asc);
                    break;
                case SortMethod.Quick_recursive:
                    copy.Sort_Quick_recursive(weightGetter, asc);
                    break;
                case SortMethod.Shell:
                    copy.Sort_Shell(weightGetter, asc);
                    break;
                case SortMethod.Bubble:
                    copy.Sort_Bubble(weightGetter, asc);
                    break;
            }
            res = copy.ToList();
        }
        /// <summary>
        /// 排序（return 输出）堆排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="weightGetter"></param>
        public static IEnumerable<T> CSortReturn<T>(this IEnumerable<T> @arr, bool asc = true) where T : IComparable
            => CSortReturn(arr, x => x, asc);
        public static IEnumerable<T> CSortReturn<T>(this IEnumerable<T> @arr, Func<T, IComparable> SortKeyGet, bool asc = true)
        {
            var count = arr.Count();
            var heap = new Heap<T>(arr, SortKeyGet, !asc);
            for (var i = 0; i < count; i++)
                yield return heap.Pop();
        }
    }
}
