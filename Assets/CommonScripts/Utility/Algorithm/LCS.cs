using System.Collections.Generic;
using System;

namespace Custom.Algorithm
{
    /// <summary>
    /// 最长公共子序列和Diff方法
    /// </summary>
    public static class Custom_LCS
    {
        /// <summary>
        /// 字符串 Diff and Replace
        /// </summary>
        /// <param name="str_old">旧字符串</param>
        /// <param name="str_new">新字符串</param>
        /// <param name="str_replace">替换字符串（需要和旧字符串的长度一致）</param>
        public static List<string> Diff_Replace(string str_old, string str_new, string str_replace)
            =>  LCS_interface(str_old, str_new,
                    (s, i, l) => l.Insert(0, str_replace[i].ToString()),
                    null,
                    (s, i, l) => l.Insert(0, s[i].ToString()));
        /// <summary>
        /// 字符串 Diff
        /// </summary>
        /// <param name="str_old">旧字符串</param>
        /// <param name="str_new">新字符串</param>
        public static List<string> Diff(string str_old, string str_new)
            =>  LCS_interface(str_old, str_new,
                    (s, i, l) => l.Insert(0, s[i].ToString()),
                    (s, i, l) => l.Insert(0, '-' + s[i].ToString()),
                    (s, i, l) => l.Insert(0, '+' + s[i].ToString()));

        /// <summary>
        /// LCS (最长公共子序列)
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        public static List<string> LCS(string str1, string str2)
             => LCS_interface(str1, str2,
                    (s, i, l) => l.Insert(0, s[i].ToString()),
                    null,
                    null);
        /// <summary>
        /// LCS (最长公共子序列)
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <param name="oriFunc"></param>
        /// <param name="delfunc"></param>
        /// <param name="addfunc"></param>
        /// <returns>
        /// 子序列
        /// </returns>
        public static List<string> LCS_interface(string str1, string str2,
            Action<string, int, List<string>> oriFunc, Action<string, int, List<string>> delfunc, Action<string, int, List<string>> addfunc)
        {
            int n = str1.Length;
            int m = str2.Length;
            var _matrix = new int[n + 1, m + 1];
            for (int i = 0; i <= n; i++)
                _matrix[i, 0] = i;
            for (int j = 0; j <= m; j++)
                _matrix[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    if (str1[i - 1] == str2[j - 1])
                        _matrix[i, j] = _matrix[i - 1, j - 1];
                    else
                        _matrix[i, j] = Math.Min(_matrix[i - 1, j], _matrix[i, j - 1]) + 1;
                }
            }
            var _result = new List<string>();
            int x = n, y = m;
            while (x > 0 || y > 0)
            {
                if (x > 0 && y > 0 && str1[x - 1] == str2[y - 1])
                {
                    x--;
                    y--;
                    oriFunc?.Invoke(str1, x, _result);
                }
                else if (x > 0 && _matrix[x, y] == _matrix[x - 1, y] + 1)
                {
                    x--;
                    delfunc?.Invoke(str1, x, _result);
                }
                else
                {
                    y--;
                    addfunc?.Invoke(str2, y, _result);
                }
            }
            return _result;
        }
    }
}