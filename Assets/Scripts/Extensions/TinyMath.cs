using System;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TinyCacto.Utils
{
    public static class TinyMath
    {
        public static bool IsWithin(this int value, int minimum, int maximum) => minimum <= value && value <= maximum;
     
        public static bool IsWithin(this float value, float minimum, float maximum) => minimum <= value && value <= maximum;
        
        public static bool IsWithin(this float value, FloatRange range) => range.Minimum <= value && value <= range.Maximum;
        
        public static int ToInt(this float f) => Mathf.RoundToInt(f);
        public static float MillisecondsToSeconds(this float f) => f / 1000;
        public static int MillisecondsToSeconds(this int f) => f / 1000;
        public static float SecondsToMilliseconds(this float f) => f * 1000;
        public static int SecondsToMilliseconds(this int f) => f * 1000;
        public static float Lerp(this float f, FloatRange r) => r.LerpFromRange(f);
        public static float InverseLerp(this float f, FloatRange r) => r.InverseLerpFromRange(f);
    }

    #region FLOAT RANGE

    [Serializable]
    public struct FloatRange
    {
        public float Minimum;
        public float Maximum;

        /// <summary>
        /// Get a random value within the <see cref="Minimum"/> and <see cref="Maximum"/>
        /// </summary>
        public float RandomInRange => Random.Range(Minimum, Maximum);

        /// <summary>
        /// Get a random value following <see cref="Minimum"/> with an override maximum value: <paramref name="maximumClamp"/>
        /// </summary>
        public float RandomMinByCustomMax(float maximumClamp) => Random.Range(Minimum, maximumClamp);

        /// <summary>
        /// Get a random value following <see cref="Maximum"/> with an override maximum value: <paramref name="minimumClamp"/>
        /// </summary>
        public float RandomMaxByCustomMin(float minimumClamp) => Random.Range(minimumClamp, Maximum);

        public float LerpFromRange(float t) => Mathf.Lerp(Minimum, Maximum, t);
        public float InverseLerpFromRange(float t) => Mathf.InverseLerp(Minimum, Maximum, t);

        public FloatRange(float min, float max)
        {
            Minimum = min;
            Maximum = max;
        }


#if UNITY_EDITOR

        [CustomPropertyDrawer(typeof(FloatRange))]
        public class FloatRangeDrawer : PropertyDrawer
        {
            private const float SubLabelSpacing = 4;
            private const float BottomSpacing = 2;
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUIUtility.singleLineHeight * (EditorGUIUtility.wideMode ? 1 : 2);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                // Find the SerializedProperties by name
                var x = property.FindPropertyRelative(nameof(Minimum));
                var y = property.FindPropertyRelative(nameof(Maximum));

                // Using BeginProperty / EndProperty on the parent property means that
                // prefab override logic works on the entire property.
                EditorGUI.BeginProperty(position, label, property);
                {


                    // Vector2Field which handles the correct drawing
                    //EditorGUI.Vector2Field(position, label, new Vector2(x.floatValue, y.floatValue));

                    var content = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                    var subLabels = new[] { new GUIContent("Min"), new GUIContent("Max") };
                    var props = new[] {x, y};

                    // backup gui settings
                    var indent = EditorGUI.indentLevel;
                    var labelWidth = EditorGUIUtility.labelWidth;

                    // draw properties
                    var propsCount = props.Length;
                    var width = (content.width - (propsCount - 1) * SubLabelSpacing) / propsCount;
                    var contentPos = new Rect(content.x, content.y, width, content.height);
                    EditorGUI.indentLevel = 0;
                    for (var i = 0; i < propsCount; i++)
                    {
                        EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(subLabels[i]).x;
                        EditorGUI.PropertyField(contentPos, props[i], subLabels[i]);
                        contentPos.x += width + SubLabelSpacing;
                    }

                    // restore gui settings
                    EditorGUIUtility.labelWidth = labelWidth;
                    EditorGUI.indentLevel = indent;
                }
                EditorGUI.EndProperty();
            }
        }

#endif

    }



    #endregion
}