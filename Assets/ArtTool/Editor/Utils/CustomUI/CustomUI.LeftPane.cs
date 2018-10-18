using System;
using UnityEngine;
using UnityEditor;

namespace ArtTool
{
    public static partial class CustomUI
    {
        public const int LeftPaneWidth = LeftPaneButtonWidth + LeftPanePaddingLeft + LeftPanePaddingRight;
        //private const int LeftPaneButtonWidth = 66, LeftPanePaddingLeft = 4, LeftPanePaddingRight = 2;
        private const int LeftPaneButtonWidth = 66, LeftPanePaddingLeft = 4, LeftPanePaddingRight = 2;
        //public const int LeftSubPaneButtonWidth = 133;
        public const int LeftSubPaneButtonWidth = 143;

        /// 左側の枠のタイトル ////////////////////////////////////////////////////////////////// 
        [System.NonSerialized] private static GUIStyle m_LeftPaneTitleLabelStyle;
        public static GUIStyle LeftPaneTitleLabelStyle => m_LeftPaneTitleLabelStyle ?? (m_LeftPaneTitleLabelStyle = Create_LeftPaneTitleLabelStyle());
        public static GUIStyle Create_LeftPaneTitleLabelStyle()
        {
            var style = new GUIStyle(EditorStyles.whiteLabel);
            //style.margin.bottom = 5;
            //style.fontSize = 10;
            return style;
        }
        public static readonly GUILayoutOption[] LeftSubPaneTitleLabelOptions = new GUILayoutOption[]
        {
            GUILayout.ExpandWidth(true),
            GUILayout.Width(LeftSubPaneButtonWidth),
            GUILayout.Height(18f),
        };

        /// 左の枠のスライダー描画
        public static void DrawLeftPaneIntSlider(GUIContent content, ref int sliderValue, int left, int right)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(content, EditorStyles.whiteMiniLabel, GUILayout.Width(34f));

            /// draw slider
            var rect = GUILayoutUtility.GetRect(6f, 16f, CustomUI.ScaleSliderStyle, CustomUI.ScaleSliderOptions);
            sliderValue = Mathf.FloorToInt(GUI.HorizontalSlider(rect,
                sliderValue, left, right
            ));

            /// draw value
            //EditorGUILayout.LabelField(sliderValue.ToString(), EditorStyles.whiteMiniLabel, GUILayout.Width(16f));
            EditorGUILayout.LabelField(sliderValue.ToString(), CustomUI.LeftPaneSliderValueLabelStyle, GUILayout.Width(24f));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(3f);
        }

        /// 左の枠のスライダー ////////////////////////////////////////////////////////////////// 
        private static GUIStyle m_LeftPaneSliderValueLabelStyle;
        public static GUIStyle LeftPaneSliderValueLabelStyle => m_LeftPaneSliderValueLabelStyle ?? (m_LeftPaneSliderValueLabelStyle = Create_LeftPaneSliderValueLabelStyle());
        public static GUIStyle Create_LeftPaneSliderValueLabelStyle() /// スライダー数値ラベル
        {
            var style = new GUIStyle(EditorStyles.whiteMiniLabel);
            style.alignment = TextAnchor.MiddleRight;
            style.margin.left = 0;
            style.margin.right = 6;
            return style;
        }

        private static GUIStyle m_LeftPaneSliderStyle;
        public static GUIStyle LeftPaneSliderStyle => m_LeftPaneSliderStyle ?? (m_LeftPaneSliderStyle = Create_LeftPaneSliderStyle());
        public static GUIStyle Create_LeftPaneSliderStyle() /// スライダー
        {
            var style = new GUIStyle(GUI.skin.horizontalSlider);
            style.margin.left = 6;
            style.margin.right = 6;
            //style.margin.bottom = 4;
            return style;
        }
        public static readonly GUILayoutOption[] LeftPaneSliderOptions = new GUILayoutOption[]
        {
            GUILayout.MinWidth(1f),
            GUILayout.ExpandWidth(true),
            //GUILayout.Width(30f),
        };


        /// 左側の枠 ////////////////////////////////////////////////////////////////// 
        private static GUIStyle m_LeftPaneBoxStyle;
        public static GUIStyle LeftPaneBoxStyle => m_LeftPaneBoxStyle ?? (m_LeftPaneBoxStyle = Create_LeftPaneBoxStyle());
        public static GUIStyle Create_LeftPaneBoxStyle()
        {
            var style = new GUIStyle(GUI.skin.box);
            style.padding = new RectOffset(LeftPanePaddingLeft, LeftPanePaddingRight, 2, 2);
            style.margin = new RectOffset(0, 0, 0, 2);
            return style;
        }
        public static readonly GUILayoutOption[] LeftPaneBoxOptions = new GUILayoutOption[]
        {
            GUILayout.Width(LeftPaneButtonWidth + LeftPanePaddingLeft + LeftPanePaddingRight),
            GUILayout.ExpandHeight(true),
        };

        /// 左側のサブ枠 ////////////////////////////////////////////////////////////////// 
        private static GUIStyle m_SubLeftPaneBoxStyle;
        public static GUIStyle SubLeftPaneBoxStyle => m_SubLeftPaneBoxStyle ?? (m_SubLeftPaneBoxStyle = Create_SubLeftPaneBoxStyle());
        public static GUIStyle Create_SubLeftPaneBoxStyle()
        {
            var style = new GUIStyle(GUI.skin.box);
            style.margin = new RectOffset(0, 0, 0, 2);
            style.padding.top = 5;
            style.padding.left = 2;
            style.alignment = TextAnchor.MiddleLeft;
            return style;
        }
        public static readonly GUILayoutOption[] SubLeftPaneBoxOptions = new GUILayoutOption[]
        {
            // GUILayout.Width(120),
            GUILayout.Width(LeftSubPaneButtonWidth),
            GUILayout.ExpandHeight(true),
        };

        /// 左枠のボタン ////////////////////////////////////////////////////////////////// 
        public static readonly GUIContent RootButtonContent_ResetColor = new GUIContent("Reset\nColor");
        public static readonly GUIContent AlphaClipContentOn = new GUIContent("AlphaClip\nOn");
        public static readonly GUIContent AlphaClipContentOff = new GUIContent("AlphaClip\nOff");
        private static GUIStyle m_LeftPaneButtonStyle;
        private static GUIStyle m_SubLeftPaneButtonStyle;
        public static GUIStyle LeftPaneButtonStyle => m_LeftPaneButtonStyle ?? (m_LeftPaneButtonStyle = Create_LeftPaneButtonStyle());
        public static GUIStyle SubLeftPaneButtonStyle => m_SubLeftPaneButtonStyle ?? (m_SubLeftPaneButtonStyle = Create_SubLeftPaneButtonStyle());
        public static GUIStyle Create_LeftPaneButtonStyle()
        {
            var style = new GUIStyle(EditorStyles.miniButton);
            style.margin = new RectOffset(0, 0, 0, 0);
            style.padding = new RectOffset(3, 3, 4, 5);
            style.wordWrap = true;
            return style;
        }
        public static GUIStyle Create_SubLeftPaneButtonStyle() /// サブ枠の中のボタン
        {
            //var style = new GUIStyle(EditorStyles.miniButton);
            var style = new GUIStyle(GUI.skin.button);
            style.margin = new RectOffset(0, 0, 0, 1);
            style.padding = new RectOffset(3, 3, 4, 5);
            style.fontSize = 11;
            // style.wordWrap = true;
            return style;
        }
        public static readonly GUILayoutOption[] LeftPaneButtonOptions = new GUILayoutOption[]
        {
            GUILayout.Width(LeftPaneButtonWidth),
        };
        public static readonly GUILayoutOption[] SubLeftPaneButtonOptions = new GUILayoutOption[]
        {
            // GUILayout.Width(62),
        };
    }
}
