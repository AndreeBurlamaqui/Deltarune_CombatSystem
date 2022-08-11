using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TinyCacto.Utils
{
    public static class TinyUI
    {
        public static void SetAlpha(this Image i, float newAlpha)
        {
            Color c = i.color;
            c.a = newAlpha;

            i.color = c;
        }

        /// <summary>
        /// Starts a Coroutine that updates the layout group
        /// </summary>
        public static Coroutine UpdateLayoutGroup(this LayoutGroup targetGroup, CanvasGroup targetCanvaGroup, bool updateFade = true)
        {
            return targetGroup.StartCoroutine(UpdateLayoutGroup());

            IEnumerator UpdateLayoutGroup()
            {
                if (updateFade)
                    targetCanvaGroup.alpha = 0;

                targetGroup.enabled = false;
                yield return new WaitForEndOfFrame();
                targetGroup.enabled = true;
                yield return new WaitForEndOfFrame();
                targetGroup.SetLayoutVertical();
                targetGroup.SetLayoutHorizontal();

                if (updateFade)
                    targetCanvaGroup.DOFade(1, 0.2f);

            }
        }

        /// <summary>
        /// Shorten value to an easy-to-read (user-friendly) string
        /// </summary>
        public static string ShortString(this float value)
        {
            if (value < 1000)
                return value.ToString("0.##");

            var ordinals = new[] { "", "K", "M", "G", "T", "P", "E" };

            float rate = value;

            var ordinal = 0;

            while (rate >= 1000)
            {
                rate /= 1000;
                ordinal++;
            }
            return $"{rate.ToString().Replace(',', '.')}{ordinals[ordinal]}";
        }



        public static string RemoveWhitespace(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return new string(input.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }

        public static Color HexToColor(this string hex)
        {
            return ColorUtility.TryParseHtmlString(hex, out Color color) ? color : Color.white;
        }
    }
}