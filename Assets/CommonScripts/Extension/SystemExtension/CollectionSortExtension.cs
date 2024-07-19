using System.Collections.Generic;
using System;
using UnityEngine;

namespace Custom
{
    public enum SortMethod
    {
        Quick,
        Heap,
        Quick_recursive,
        Shell,
        Bubble,
    }
    public static class CollectionSortExtension
    {
        /// <summary>
        /// 冒泡排序，算法的时间复杂度为O(n^2)，算法稳定
        /// </summary>
        /// <param name="arr"></param>
        public static void Sort_Bubble<T>(this IList<T> arr, bool asc = true) where T : IComparable
            => Sort_Bubble(arr, x => x, asc);
        /// <summary>
        /// 冒泡排序，算法的时间复杂度为O(n^2)，算法稳定
        /// </summary>
        /// <param name="arr"></param>
        public static void Sort_Bubble<T>(this IList<T> arr, Func<T, IComparable> SortKeyGet, bool asc = true)
        {
            var length = arr.Count;
            for (var i = 0; i < length; i++)//外层循环
            {
                for (var j = 0; j < length - i - 1; j++)//内层循环
                {
                    if (SortKeyGet(arr[j]).CompareTo(arr[j + 1]) > 0 && asc || SortKeyGet(arr[j]).CompareTo(arr[j + 1]) < 0 && !asc)
                        (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
                }
            }
        }
        /// <summary>
        /// 希尔排序，算法的时间复杂度为O(n^1.3)，最好O(n)，最坏O(n^2)，算法不稳定
        /// </summary>
        /// <param name="arr"></param>
        public static void Sort_Shell<T>(this IList<T> arr, bool asc = true) where T : IComparable
            => Sort_Shell(arr, x => x, asc);
        /// <summary>
        /// 希尔排序，算法的时间复杂度为O(n^1.3)，最好O(n)，最坏O(n^2)，算法不稳定
        /// </summary>
        /// <param name="arr"></param>
        public static void Sort_Shell<T>(this IList<T> arr, Func<T, IComparable> SortKeyGet, bool asc = true)
        {
            int length = arr.Count;
            int gap = length;//步长
            while (gap > 1)
            {
                gap = gap / 3 + 1;// 步长递减公式
                for (int i = 0; i < gap; i++)//分组, 对每一组, 进行插入排序
                {
                    T temp = default(T);//保存基准数
                    int index = 0;//坑的位置
                    for (int j = i + gap; j < length; j += gap)
                    {
                        index = j;
                        temp = arr[j];
                        for (int k = j - gap; k >= 0; k -= gap)//有序序列(从后往前遍历)
                        {
                            if (SortKeyGet(temp).CompareTo(arr[k]) < 0 && asc || SortKeyGet(temp).CompareTo(arr[k]) > 0 && !asc)//基准数根有序序列中的元素比较
                            {
                                arr[k + gap] = arr[k];
                                index = k;
                            }
                            else
                            {
                                break;
                            }
                        }
                        arr[index] = temp;//填坑
                    }
                }
            }
        }
        /// <summary>
        /// 【递归】快速排序，算法的时间复杂度为O(nlogn)，算法不稳定
        /// 挖坑排序+分治算法
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static void Sort_Quick_recursive<T>(this IList<T> arr, bool asc = true) where T : IComparable
            => Sort_Quick_recursive(arr, x => x, asc);
        /// <summary>
        /// 【递归】快速排序，算法的时间复杂度为O(nlogn)，算法不稳定
        /// 挖坑排序+分治算法
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static void Sort_Quick_recursive<T>(this IList<T> arr, Func<T, IComparable> SortKeyGet, bool asc = true)
        {
            Sort_Quick_recursive(arr, 0, arr.Count - 1, SortKeyGet, asc);
        }
        private static void Sort_Quick_recursive<T>(IList<T> arr, int low, int high, Func<T, IComparable> SortKeyGet, bool asc)
        {
            if (low < high)
            {
                int l = low, r = high;
                T k = arr[l];//比较基数
                while (l < r)
                {
                    while (l < r && (SortKeyGet(arr[r]).CompareTo(k) >= 0 && asc || SortKeyGet(arr[r]).CompareTo(k) <= 0 && !asc))//从右往左查找第一个小于基数k的数，并放到左边
                        r--;
                    if (l < r)
                        arr[l++] = arr[r];//将arr[r]填到arr[l]中，arr[r]就形成了一个新的坑，进行过一次替换后，没必要将替换后的两值再次比较，所以l++直接下一位与k对比
                    while (l < r && (SortKeyGet(arr[l]).CompareTo(k) < 0 && asc || SortKeyGet(arr[l]).CompareTo(k) > 0 && !asc))//从左往右查找第一个大于基数k的数，并放到右边
                        l++;
                    if (l < r)
                        arr[r--] = arr[l];//将arr[l]填到arr[r]中，arr[l]就形成了一个新的坑，进行过一次替换后，没必要将替换后的两值再次比较，所以r--直接上一位与k对比
                }
                //退出时，l等于r。将k填到这个坑中。
                arr[l] = k;
                Sort_Quick_recursive(arr, low, l - 1, SortKeyGet, asc);
                Sort_Quick_recursive(arr, l + 1, high, SortKeyGet, asc);
            }
        }
        /// <summary>
        /// 【迭代】快速排序，算法的时间复杂度为O(nlogn)，算法不稳定
        /// 挖坑排序+分治算法
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static void Sort_Quick_iterable<T>(this IList<T> arr, bool asc = true) where T : IComparable
            => Sort_Quick_iterable(arr, x => x, asc);
        /// <summary>
        /// 【迭代】快速排序，算法的时间复杂度为O(nlogn)，算法不稳定
        /// 挖坑排序+分治算法
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static void Sort_Quick_iterable<T>(this IList<T> arr, Func<T, IComparable> SortKeyGet, bool asc = true)
        {
            Stack<int> stack = new Stack<int>();
            stack.Push(0);
            stack.Push(arr.Count - 1);
            while (stack.Count > 0)
            {
                int low = stack.Pop();
                int high = stack.Pop();
                if (low > high)
                    (low, high) = (high, low);
                Sort_Quick_iterable(arr, low, high, stack, SortKeyGet, asc);
            }
        }
        private static void Sort_Quick_iterable<T>(IList<T> R, int Low, int High, Stack<int> stack, Func<T, IComparable> SortKeyGet, bool asc)
        {
            int low = Low;
            int high = High;
            T temp = R[low];
            while (high > low)
            {
                while (low < high && (SortKeyGet(temp).CompareTo(R[high]) <= 0 && asc || SortKeyGet(temp).CompareTo(R[high]) >= 0 && !asc))
                {
                    high--;
                }
                if (high > low)
                {
                    R[low] = R[high];
                    R[high] = temp;
                }
                while (low < high && (SortKeyGet(temp).CompareTo(R[low]) > 0 && asc || SortKeyGet(temp).CompareTo(R[low]) < 0 && !asc))
                {
                    low++;
                }
                if (high > low)
                {
                    R[high] = R[low];
                    R[low] = temp;
                }
                if (low == high)
                {
                    if (Low < low - 1)
                    {
                        stack.Push(Low);
                        stack.Push(low - 1);
                    }
                    if (High > low + 1)
                    {
                        stack.Push(low + 1);
                        stack.Push(High);
                    }
                }
            }
        }
        /// <summary>
        /// 堆排序，算法的时间复杂度为O(nlogn)，算法不稳定
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static void Sort_Heap<T>(this IList<T> arr, bool asc = true) where T : IComparable
            => Sort_Heap(arr, a => a, asc);
        /// <summary>
        /// 堆排序，算法的时间复杂度为O(nlogn)，算法不稳定
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static void Sort_Heap<T>(this IList<T> arr, Func<T, IComparable> SortKeyGet, bool asc = true)
        {
            var heap = new Heap<T>(arr, SortKeyGet, !asc);
            for (var i = 0; i < arr.Count; i++)
                @arr[i] = heap.Pop();
        }

        /// <summary>
        /// 选择k个最大值
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="elements">输入值</param>
        /// <returns>从小到大排列</returns>
        public static IEnumerable<T> Maxrange<T>(this IList<T> arr, int count) where T: IComparable
            => Maxrange(arr, count, a => a);
        public static IEnumerable<T> Maxrange<T>(this IList<T> arr, int count, Func<T, IComparable> SortKeyGet)
        {
            foreach (var e in Heap<T>.SelectMaximumElements(count, arr, SortKeyGet))
                yield return e.key;
        }
        /// <summary>
        /// 选择k个最小值
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="elements">输入值</param>
        /// <returns>从大到小排列</returns>
        public static IEnumerable<T> Minrange<T>(this IList<T> arr, int count) where T: IComparable
            => Minrange(arr, count, a => a);
        public static IEnumerable<T> Minrange<T>(this IList<T> arr, int count, Func<T, IComparable> SortKeyGet)
        {
            foreach (var e in Heap<T>.SelectMinimunElements(count, arr, SortKeyGet))
            {
                yield return e.key;
            }
        }
    }
}