using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Cosmos
{
    public static class CRandom
    {
        public static bool TryRandomPick<T>(this IReadOnlyList<T> @this, out T value, in System.Random random = null)
        {
            if (@this is null || @this.Count <= 0)
            {
                value = default;
                return false;
            }
            var randomIndex = random == null ? UnityEngine.Random.Range(0, @this.Count) : random.Next(0, @this.Count);
            value = @this[randomIndex];
            return true;
        }
        /// <summary>
        /// 随机挑选 随机种子
        /// </summary>
        public static T RandomPick<T>(this IReadOnlyList<T> @this, System.Random random = null)
        {
            if (!@this.TryRandomPick(out var value, random))
            {
                Debug.LogError("list is null or empty.");
                return value;
            }
            return value;
        }
        public static bool TryRandomPop<T>(this IList<T> @this, out T value, bool keepOrder = false, in System.Random random = null)
        {
            if (@this is null || @this.Count <= 0)
            {
                value = default;
                return false;
            }
            var index = random == null ? UnityEngine.Random.Range(0, @this.Count) : random.Next(0, @this.Count);
            if (!keepOrder)
            {
                var lastIndex = @this.Count - 1;
                (@this[lastIndex], @this[index]) = (@this[index], @this[lastIndex]);
                value = @this[lastIndex];
                @this.RemoveAt(lastIndex);
            }
            else
            {
                value = @this[index];
                @this.RemoveAt(index);
            }
            return true;
        }
        /// <summary>
        /// 随机移除元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="value"></param>
        /// <param name="keepOrder">是否保持有序</param>
        /// <returns></returns>
        public static T RandomPop<T>(this IList<T> @this, bool keepOrder = false, in System.Random random = null)
        {
            if (!@this.TryRandomPop(out var value, keepOrder, random))
            {
                Debug.LogError("list is null or empty.");
                return value;
            }
            return value;
        }
        public static T RandomPop<T>(this IList<T> @this, Func<T, float> weightGetter, bool keepOrder = false, in System.Random random = null)
        {
            var value = float.MinValue;
            var resultIndex = 0;
            var count = @this.Count;
            for(var index = 0; index < count; index ++)
            {
                var item = @this[index];
                float weight;
                try
                {
                    weight = weightGetter(item);
                    if (weight <= 0) continue;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    continue;
                }
                var score = Mathf.Pow(random == null ? UnityEngine.Random.value : (float)random.NextDouble(), 1 / weight);
                if (score > value)
                {
                    resultIndex = index;
                    value = score;
                }
            }
            T result;
            if (!keepOrder)
            {
                var lastIndex = @this.Count - 1;
                (@this[lastIndex], @this[resultIndex]) = (@this[resultIndex], @this[lastIndex]);
                result = @this[lastIndex];
                @this.RemoveAt(lastIndex);
            }
            else
            {
                result = @this[resultIndex];
                @this.RemoveAt(resultIndex);
            }
            return result;
        }
        /// <summary>
        /// 高效的随机挑选算法,按权重不重复随机挑选<paramref name="pickCount"/>个
        /// </summary>
        /// <remarks>
        /// <para>权重&lt;=0将不被抽取</para>
        /// </remarks>
        /// <remarks>时间复杂度O(n)</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="this">物品序列</param>
        /// <param name="weightGetter">权重计算器</param>
        /// <param name="pickCount">挑选数量</param>
        /// <returns>被挑选物品 (可能不足<paramref name="pickCount"/>个)</returns>
        public static IEnumerable<T> RandomPick<T>(this IEnumerable<T> @this, Func<T, float> weightGetter, int pickCount, System.Random random = null)
        {
            if (@this is null)
                yield break;
            var heap = new Heap<T>();
            float score;
            float weight;
            foreach (var item in @this)
            {
                try
                {
                    weight = weightGetter(item);
                    if (weight.CompareTo(0) <= 0) continue;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    continue;
                }
                // 随机 ^ (1 / weight), 最大的n个即为所求
                if (random != null)
                    score = Mathf.Pow((float)random.NextDouble(), 1 / weight);
                else
                    score = Mathf.Pow(UnityEngine.Random.value, 1 / weight);
                if (heap.Count >= pickCount)
                {
                    if (heap.TryPeek(out _, out var value) && value.CompareTo(score) < 0)
                    {
                        heap.Pop();
                        heap.Add(item, score);
                    }
                }
                else
                    heap.Add(item, score);
            }
            foreach (var item in heap)
                yield return item.Key;
        }
        /// <summary>
        /// 高效的随机挑选算法,不重复随机挑选<paramref name="pickCount"/>个
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <remarks>时间复杂度O(n)</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="this">物品序列</param>
        /// <param name="pickCount">挑选数量</param>
        /// <returns>被挑选物品 (可能不足<paramref name="pickCount"/>个)</returns>
        public static IEnumerable<T> RandomPick<T>(this IEnumerable<T> @this, int pickCount, System.Random random)
        {
            if (random is null)
            {
                foreach (var element in RandomPick(@this, pickCount))
                    yield return element;
                yield break;
            }
            // 不调用带权重的RandomPick，因为可以进一步提高效率
            if (@this is null)
                yield break;
            var heap = new Heap<T>();
            float score;
            foreach (var item in @this)
            {
                score = (float)random.NextDouble();
                if (heap.Count >= pickCount)
                {
                    if (heap.TryPeek(out _, out var value) && value.CompareTo(score) < 0)
                    {
                        heap.Pop();
                        heap.Add(item, score);
                    }
                }
                else
                    heap.Add(item, score);
            }
            foreach (var item in heap)
                yield return item.Key;
        }
                /// <summary>
        /// 高效的随机挑选算法,不重复随机挑选<paramref name="pickCount"/>个
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <remarks>时间复杂度O(n)</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="this">物品序列</param>
        /// <param name="pickCount">挑选数量</param>
        /// <returns>被挑选物品 (可能不足<paramref name="pickCount"/>个)</returns>
        public static IEnumerable<T> RandomPick<T>(this IEnumerable<T> @this, int pickCount)
        {
            // 不调用带权重的RandomPick，因为可以进一步提高效率
            if (@this is null)
                yield break;
            var heap = new Heap<T>();
            float score;
            foreach (var item in @this)
            {
                score = UnityEngine.Random.value;
                if (heap.Count >= pickCount)
                {
                    if (heap.TryPeek(out _, out var value) && value.CompareTo(score) < 0)
                    {
                        heap.Pop();
                        heap.Add(item, score);
                    }
                }
                else
                    heap.Add(item, score);
            }
            foreach (var item in heap)
                yield return item.Key;
        }
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> @this)
        {
            var hashSet = new HashSet<T>();
            foreach (var element in @this)
                hashSet.Add(element);
            return hashSet;
        }
        public static int IndexOf<T>(this IEnumerable<T> @this, T value)
        {
            foreach (var (index, element) in @this.WithIndex())
                if (value.Equals(element))
                    return index;
            return -1;
        }
    }
}
