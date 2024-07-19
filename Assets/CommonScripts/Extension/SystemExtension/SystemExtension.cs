using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Custom
{
    public static class SystemExtension
    {
        /// <summary>
        /// 字符串拼接
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns> <summary>
        static public string Concat<T>(this IEnumerable<T> @this, string separator = "")
        {
            return string.Join(separator, @this);
        }
        /// <summary>
        /// 判断字符串是否为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        /// <summary>
        /// 格式化字符串
        /// e.g. "This is test string: {0} / {1}".FormatString(1, 2);
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static public string Format(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
        static public int HexToDecimal(in char ch)
        {
            switch (ch)
            {
                case '0': return 0x0;
                case '1': return 0x1;
                case '2': return 0x2;
                case '3': return 0x3;
                case '4': return 0x4;
                case '5': return 0x5;
                case '6': return 0x6;
                case '7': return 0x7;
                case '8': return 0x8;
                case '9': return 0x9;
                case 'a':
                case 'A': return 0xA;
                case 'b':
                case 'B': return 0xB;
                case 'c':
                case 'C': return 0xC;
                case 'd':
                case 'D': return 0xD;
                case 'e':
                case 'E': return 0xE;
                case 'f':
                case 'F': return 0xF;
            }
            return 0xF;
        }
        public static void TryInvoke(this Action @this)
        {
            if (@this is null)
                return;
            try
            {
                @this();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        public static void TryInvoke<T>(this Action<T> @this, T arg)
        {
            try
            {
                @this(arg);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        public static void TryInvoke<T0, T1>(this Action<T0, T1> @this, T0 arg0, T1 arg1)
        {
            try
            {
                @this(arg0, arg1);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
