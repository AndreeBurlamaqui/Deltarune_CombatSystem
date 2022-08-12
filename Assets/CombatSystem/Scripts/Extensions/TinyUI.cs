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

        ///<summary>
        /// Returns a Rect in WorldSpace dimensions using <see cref="RectTransform.GetWorldCorners"/>
        ///</summary>
        public static Rect GetWorldRect(this RectTransform rectTransform)
        {
            // This returns the world space positions of the corners in the order
            // [0] bottom left,
            // [1] top left
            // [2] top right
            // [3] bottom right
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            Vector2 min = corners[0];
            Vector2 max = corners[2];
            Vector2 size = max - min;

            return new Rect(min, size);
        }

        ///<summary>
        /// Checks if a <see cref="RectTransform"/> fully encloses another one
        ///</summary>
        public static bool IsInsideOf(this RectTransform rectTransform, RectTransform other)
        {
            var rect = rectTransform.GetWorldRect();
            var otherRect = other.GetWorldRect();

            // Now that we have the world space rects simply check
            // if the other rect lies completely between min and max of this rect
            return rect.xMin <= otherRect.xMin
                && rect.yMin <= otherRect.yMin
                && rect.xMax >= otherRect.xMax
                && rect.yMax >= otherRect.yMax;
        }

    }
}