using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace Cosmos
{
    public static class UIExtension
    {
        /// <summary>
        /// 将X16 string转换为Color
        /// </summary>
        public static Color ToColor(this string text)
        {
            Assert.IsNotNull(text, "A NULL text can't be convert to color");
            Assert.IsTrue(text.Length == 6, "The text can't be convert to color: " + text);

            int r = (SystemExtension.HexToDecimal(text[0]) << 4) | SystemExtension.HexToDecimal(text[1]);
            int g = (SystemExtension.HexToDecimal(text[2]) << 4) | SystemExtension.HexToDecimal(text[3]);
            int b = (SystemExtension.HexToDecimal(text[4]) << 4) | SystemExtension.HexToDecimal(text[5]);

            return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
        }

        public static void ScrollToTop(this RectTransform rectTransform)
        {
            if (rectTransform != null) rectTransform.anchoredPosition = Vector2.zero;
        }

        public static void ScrollToTop(this GridLayoutGroup gridLayoutGroup)
        {
            if (gridLayoutGroup != null) ScrollToTop(gridLayoutGroup.GetComponent<RectTransform>());
        }

        public static void ScrollToTop(this VerticalLayoutGroup verticalLayoutGroup)
        {
            if (verticalLayoutGroup != null) ScrollToTop(verticalLayoutGroup.GetComponent<RectTransform>());
        }
    }
}
