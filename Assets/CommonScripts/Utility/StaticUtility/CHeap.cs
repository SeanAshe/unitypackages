using System;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Custom
{
    public interface IReadOnlyHeap<T> : IReadOnlyCollection<KeyValuePair<T, IComparable>>
    {
        T Peek(out IComparable value, int index = 0);
        T Peek(int index = 0);
        bool TryPeek(out T key, int index = 0);
        bool TryPeek(out T key, out IComparable value, int index = 0);
    }
    public interface IHeap<T> : IReadOnlyHeap<T>
    {
        void Add(T element, IComparable sortingValue);
        T Pop(int index = 0);
        T Pop(out IComparable value, int index = 0);
        void TrimExcess();
        void Clear();
    }
    public class Heap<T> : IHeap<T>
    {
        public int Count => m_length;
        public bool Max => m_max;

        private T[] m_elements;
        private IComparable[] m_sortingValues;
        private int m_length = 0;
        private static bool m_max = false;
        public Heap(IEnumerable<T> elements, Func<T, IComparable> sortValueGetter, bool Max = false): this(elements.Count(), Max)
        {
            foreach(var element in elements)
                Add(element, sortValueGetter(element));
        }
        public Heap() : this(32) {}
        public Heap(int defaultCapability, bool Max = false)
        {
            if (defaultCapability <= 0)
                throw new ArgumentException($"{nameof(defaultCapability)} should be larger than 0");
            m_elements = new T[defaultCapability];
            m_sortingValues = new IComparable[defaultCapability];
            m_max = Max;
        }
        /// <summary>
        /// 堆增加一个元素
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="sortingValue">排序值</param>
        public void Add(T element, IComparable sortingValue)
        {
            if (m_length >= m_elements.Length)
            {
                var copiedKeys = new T[m_elements.Length * 2];
                Array.Copy(m_elements, copiedKeys, m_elements.Length);
                m_elements = copiedKeys;
                var copiedValues = new IComparable[m_sortingValues.Length * 2];
                Array.Copy(m_sortingValues, copiedValues, m_sortingValues.Length);
                m_sortingValues = copiedValues;
            }
            HeapAdd(element, sortingValue);
        }
        /// <summary>
        /// 堆增加一个元素
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="sortingValue">排序值</param>
        public void Add(T element, Func<T, IComparable> sortingValueGet)
        {
            if (m_length >= m_elements.Length)
            {
                var copiedKeys = new T[m_elements.Length * 2];
                Array.Copy(m_elements, copiedKeys, m_elements.Length);
                m_elements = copiedKeys;
                var copiedValues = new IComparable[m_sortingValues.Length * 2];
                Array.Copy(m_sortingValues, copiedValues, m_sortingValues.Length);
                m_sortingValues = copiedValues;
            }
            HeapAdd(element, sortingValueGet(element));
        }
        public void Update(T element, float sortingValue, int index)
        {
            if (index < 0 || index >= m_length)
                throw new ArgumentOutOfRangeException(nameof(index));
            HeapUpdate(element, sortingValue, index);
        }
        public void Clear()
        {
            Array.Clear(m_elements, 0, m_elements.Length);
            Array.Clear(m_sortingValues, 0, m_sortingValues.Length);
            m_length = 0;
        }
        public void AddOrUpdate(T element, float sortingValue)
        {
            for(var i = 0; i < m_length; i++)
            {
                if (m_elements[i].Equals(element))
                {
                    Update(element, sortingValue, i);
                    return;
                }
            }
            Add(element, sortingValue);
        }
        public T Pop(out IComparable value, int index = 0)
        {
            value = m_sortingValues[index];
            return HeapPop(index);
        }

        public T Pop(int index = 0)
        {
            return HeapPop(index);
        }

        public T Peek(out IComparable value, int index = 0)
        {
            value = m_sortingValues[index];
            return m_elements[index];
        }

        public T Peek(int index = 0)
        {
            return m_elements[index];
        }
        /// <summary>
        /// 向堆增加元素
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="sortingValue">排序值</param>
        /// <param name="heep">堆</param>
        private void HeapAdd(T element, IComparable sortingValue)
        {
            m_elements[m_length] = element;
            // m_sortingValues[m_length] = m_max ? sortingValue : -sortingValue;
            m_sortingValues[m_length] = sortingValue;
            AdjustUp(m_length++);
        }
        /// <summary>
        /// 移除堆的一个元素
        /// </summary>
        /// <param name="keys">堆化列表</param>
        /// <param name="length">长度</param>
        /// <param name="index">被移除元素下标</param>
        private T HeapPop(int index = 0)
        {
            var result = m_elements[index];
            m_elements[index] = m_elements[--m_length];
            m_sortingValues[index] = m_sortingValues[m_length];
            AdjustDown(index);
            AdjustUp(index);
            return result;
        }
        private void HeapUpdate(T element, float sortingValue, int index)
        {
            m_elements[index] = element;
            m_sortingValues[index] = sortingValue;
            AdjustDown(index);
            AdjustUp(index);
        }
        private void AdjustUp(int index) => AdjustUp(m_elements, m_sortingValues, index);
        private static void AdjustUp(T[] elements, IComparable[] sortingValues, int index)
        {
            int parentIndex;
            while (index > 0)
            {
                parentIndex = (index - 1) >> 1;
                if (sortingValues[index].CompareTo(sortingValues[parentIndex]) <= 0 && !m_max
                    ||sortingValues[index].CompareTo(sortingValues[parentIndex]) > 0 && m_max)
                {
                    (elements[parentIndex], elements[index]) = (elements[index], elements[parentIndex]);
                    (sortingValues[parentIndex], sortingValues[index]) = (sortingValues[index], sortingValues[parentIndex]);
                    index = parentIndex;
                }
                else
                    break;
            }
        }
        private void AdjustDown(int index) => AdjustDown(m_elements, m_sortingValues, m_length, index);
        private static void AdjustDown(T[] elements, IComparable[] sortingValues, int length, int index)
        {
            int leftIndex, rightIndex;
            while (true)
            {
                leftIndex = (index << 1) + 1;
                if (leftIndex >= length)
                    return;
                rightIndex = leftIndex + 1;
                if (rightIndex >= length)
                {
                    if (sortingValues[leftIndex].CompareTo(sortingValues[index]) <= 0 && !m_max
                        ||sortingValues[leftIndex].CompareTo(sortingValues[index]) > 0 && m_max)
                    {
                        (elements[leftIndex], elements[index]) = (elements[index], elements[leftIndex]);
                        (sortingValues[leftIndex], sortingValues[index]) = (sortingValues[index], sortingValues[leftIndex]);
                        return;
                    }
                    else
                        return;
                }
                else
                {
                    if (sortingValues[leftIndex].CompareTo(sortingValues[index]) <= 0 && !m_max
                        ||sortingValues[leftIndex].CompareTo(sortingValues[index]) > 0 && m_max)
                    {
                        if (sortingValues[rightIndex].CompareTo(sortingValues[leftIndex]) <= 0 && !m_max
                            ||sortingValues[rightIndex].CompareTo(sortingValues[leftIndex]) > 0 && m_max)
                        {
                            (elements[rightIndex], elements[index]) = (elements[index], elements[rightIndex]);
                            (sortingValues[rightIndex], sortingValues[index]) = (sortingValues[index], sortingValues[rightIndex]);
                            index = rightIndex;
                        }
                        else
                        {
                            (elements[leftIndex], elements[index]) = (elements[index], elements[leftIndex]);
                            (sortingValues[leftIndex], sortingValues[index]) = (sortingValues[index], sortingValues[leftIndex]);
                            index = leftIndex;
                        }
                    }
                    else
                    {
                        if (sortingValues[rightIndex].CompareTo(sortingValues[index]) <= 0 && !m_max
                            ||sortingValues[rightIndex].CompareTo(sortingValues[index]) > 0 && m_max)
                        {
                            (elements[rightIndex], elements[index]) = (elements[index], elements[rightIndex]);
                            (sortingValues[rightIndex], sortingValues[index]) = (sortingValues[index], sortingValues[rightIndex]);
                            index = rightIndex;
                        }
                        else
                            return;
                    }
                }
            }
        }
        IEnumerator<KeyValuePair<T, IComparable>> IEnumerable<KeyValuePair<T, IComparable>>.GetEnumerator()
        {
            for (var i = 0; i < m_length; i++)
                yield return new KeyValuePair<T, IComparable>(m_elements[i], m_sortingValues[i]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (var i = 0; i < m_length; i++)
                yield return new KeyValuePair<T, IComparable>(m_elements[i], m_sortingValues[i]);
        }
        public void TrimExcess()
        {
            var copiedKeys = new T[m_length];
            Array.Copy(m_elements, copiedKeys, m_length);
            m_elements = copiedKeys;
            var copiedValues = new IComparable[m_length];
            Array.Copy(m_sortingValues, copiedValues, m_length);
            m_sortingValues = copiedValues;
        }

        public bool TryPeek(out T key, int index = 0)
        {
            if (m_length > index)
            {
                key = Peek(index);
                return true;
            }
            key = default;
            return false;
        }

        public bool TryPeek(out T key, out IComparable value, int index = 0)
        {
            if (m_length > index)
            {
                key = Peek(out value, index);
                return true;
            }
            key = default;
            value = default;
            return false;
        }

        public static void CheckHeap_min(IComparable[] values, int length)
        {
            for (var index = 0; index < length; index++)
            {
                var value = values[index];
                var left = (index << 1) + 1;
                if (left >= length)
                    return;
                Assert.IsTrue(value.CompareTo(values[left]) <= 0);
                var right = left + 1;
                if (right >= length)
                    return;
                Assert.IsTrue(value.CompareTo(values[right]) <= 0);
            }
            return;
        }
        public static void CheckHeapNode_min(IComparable[] values, int length, int index)
        {
            var value = values[index];
            var left = (index << 1) + 1;
            if (left >= length)
                return;
            Assert.IsTrue(value.CompareTo(values[left]) <= 0);
            var right = left + 1;
            if (right >= length)
                return;
            Assert.IsTrue(value.CompareTo(values[right]) <= 0);
        }
        public static void HeapAdd_min(T key, IComparable value, ref T[] keys, ref IComparable[] values, ref int length)
        {
            CheckHeap_min(values, length);
            if (keys.Length <= length)
            {
                var copy = keys;
                keys = new T[length * 2 + 1];
                Array.Copy(copy, keys, copy.Length);
            }
            if (values.Length <= length)
            {
                var copy = values;
                values = new IComparable[length * 2 + 1];
                Array.Copy(copy, values, copy.Length);
            }
            keys[length] = key;
            values[length] = value;
            AdjustUp(keys, values, length++);
        }
        public static void HeapUpdate_min(T key, IComparable value, int index, T[] keys, IComparable[] values, int length)
        {
            CheckHeap_min(values, length);
            keys[index] = key;
            values[index] = value;
            AdjustDown(keys, values, length, index);
            AdjustUp(keys, values, index);
        }
        public static T HeapPop_min(T[] keys, IComparable[] values, ref int length, int index = 0)
        {
            CheckHeap_min(values, length);
            var result = keys[index];
            keys[index] = keys[--length];
            values[index] = values[length];
            AdjustDown(keys, values, length, index);
            AdjustUp(keys, values, index);
            return result;
        }
        /// <summary>
        /// 选择k个最小值
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="elements">输入值</param>
        /// <returns>从大到小排列</returns>
        public static IEnumerable<(T key, IComparable value)> SelectMinimunElements(int count, IEnumerable<(T, IComparable)> elements)
            => SelectElements(count, elements, false);
        /// <summary>
        /// 选择k个最小值
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="elements">输入值</param>
        /// <returns>从大到小排列</returns>
        public static IEnumerable<(T key, IComparable value)> SelectMinimunElements(int count, params (T, IComparable)[] elements)
            => SelectElements(count, elements, false);
        /// <summary>
        /// 选择k个最小值
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="elements">输入值</param>
        /// <returns>从大到小排列</returns>
        public static IEnumerable<(T key, IComparable value)> SelectMinimunElements(int count, IEnumerable<T> elements, Func<T, IComparable> sortValueGetter)
            => SelectElements(count, elements, sortValueGetter, false);
        /// <summary>
        /// 选择k个最大值
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="elements">输入值</param>
        /// <returns>从小到大排列</returns>
        public static IEnumerable<(T key, IComparable value)> SelectMaximumElements(int count, params (T, IComparable)[] elements)
            => SelectElements(count, elements, true);
        /// <summary>
        /// 选择k个最大值
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="elements">输入值</param>
        /// <returns>从小到大排列</returns>
        public static IEnumerable<(T key, IComparable value)> SelectMaximumElements(int count, IEnumerable<(T, IComparable)> elements)
            => SelectElements(count, elements, true);
                    /// <summary>
        /// 选择k个最大值
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="elements">输入值</param>
        /// <returns>从小到大排列</returns>
        public static IEnumerable<(T key, IComparable value)> SelectMaximumElements(int count, IEnumerable<T> elements, Func<T, IComparable> sortValueGetter)
            => SelectElements(count, elements, sortValueGetter, true);

        private static IEnumerable<(T key, IComparable value)> SelectElements(int count, IEnumerable<T> elements, Func<T, IComparable> sortValueGetter, bool Max)
        {
            var heap = new Heap<T>(count, !Max);
            foreach (var key in elements)
                Add(heap, count, key, sortValueGetter(key), Max);
            while (heap.Count > 0)
            {
                var element = heap.Pop(out var value);
                yield return (element, value);
            }
        }
        private static IEnumerable<(T key, IComparable value)> SelectElements(int count, IEnumerable<(T key, IComparable value)> elements, bool Max)
        {
            var heap = new Heap<T>(count, !Max);
            foreach (var (key, value) in elements)
                Add(heap, count, key, value, Max);
            while (heap.Count > 0)
            {
                var element = heap.Pop(out var value);
                yield return (element, value);
            }
        }
        private static void Add(Heap<T> heap, int count, T element, IComparable value, bool max)
        {
            if (heap.Count >= count)
            {
                heap.Peek(out var topValue);
                if (value.CompareTo(topValue) > 0 && max || value.CompareTo(topValue) < 0 && !max)
                    heap.Pop();
                else
                    return;
            }
            heap.Add(element, value);
        }
    }
}
