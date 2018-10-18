using System;
using UnityEngine;
using UnityEditor;

namespace ArtTool
{
    public static partial class CustomUI
    {
        /// 設定UIのラベル(上部) ////////////////////////////////////////////////////////////////// 
        public static readonly GUIContent InputTextureLabelContent = new GUIContent("Texture");
        public static readonly GUIContent InputShaderLabelContent = new GUIContent("Shader");
        public static readonly GUIContent InputMaterialLabelContent = new GUIContent("Material(Genereated)");
        public static readonly GUIContent InputRenderTextureLabelContent = new GUIContent("RT(Generated)");
        public static readonly GUILayoutOption[] InputLabelOptions = new GUILayoutOption[]
        {
            GUILayout.Width(134f),
        };

        /// 設定UIを囲む枠(上部) ////////////////////////////////////////////////////////////////// 
        private static GUIStyle m_InputFieldBoxStyle;
        public static GUIStyle InputFieldBoxStyle => m_InputFieldBoxStyle ?? (m_InputFieldBoxStyle = Create_InputFieldBoxStyle());
        public static GUIStyle Create_InputFieldBoxStyle()
        {
            var style = new GUIStyle();
            style.margin = new RectOffset();
            return style;
        }
        public static readonly GUILayoutOption[] InputFieldBoxOptions = new GUILayoutOption[]
        {
        };


        /// スケール設定スライダー ////////////////////////////////////////////////////////////////// 
        private static GUIStyle m_ScaleSliderStyle;
        public static GUIStyle ScaleSliderStyle => m_ScaleSliderStyle ?? (m_ScaleSliderStyle = Create_ScaleSliderStyle());
        public static GUIStyle Create_ScaleSliderStyle()
        {
            var style = new GUIStyle(GUI.skin.horizontalSlider);
            style.margin.left = 6;
            style.margin.right = 6;
            style.margin.bottom = 4;
            return style;
        }
        public static readonly GUILayoutOption[] ScaleSliderOptions = new GUILayoutOption[]
        {
            GUILayout.MinWidth(1f),
        };

        /// 下の枠(フッター) ////////////////////////////////////////////////////////////////// 
        private static GUIStyle m_FooterPaneBoxStyle;
        public static GUIStyle FooterPaneBoxStyle => m_FooterPaneBoxStyle ?? (m_FooterPaneBoxStyle = Create_FooterPaneBoxStyle());
        public static GUIStyle Create_FooterPaneBoxStyle()
        {
            var style = new GUIStyle(GUI.skin.box);
            style.padding = new RectOffset(8, 8, 8, 8);
            style.margin = new RectOffset(0, 0, 0, 0);
            return style;
        }
        public static readonly GUILayoutOption[] FooterPaneBoxOptions = new GUILayoutOption[]
        {
            GUILayout.Height(32),
        };
        
        /// グラデーション ////////////////////////////////////////////////////////////////// 
        public static readonly GUILayoutOption[] GradientOptions = new GUILayoutOption[]
        {
            GUILayout.MaxHeight(9999f),
            GUILayout.MaxWidth(9999f),
            GUILayout.ExpandHeight(true),
            GUILayout.ExpandWidth(true),
        };

        /// テクスチャ ////////////////////////////////////////////////////////////////// 
        private static GUIStyle m_TextureRectStyle;
        public static GUIStyle TextureRectStyle => m_TextureRectStyle ?? (m_TextureRectStyle = Create_TextureRectStyle());
        public static GUIStyle Create_TextureRectStyle()
        {
            var style = new GUIStyle();
            style.alignment = TextAnchor.UpperLeft;
            return style;
        }
        public static readonly GUILayoutOption[] TextureRectOptions = new GUILayoutOption[]
        {
        };

        /// テクスチャを囲む枠 ////////////////////////////////////////////////////////////////// 
        private static GUIStyle m_TextureBoxStyle;
        public static GUIStyle TextureBoxStyle => m_TextureBoxStyle ?? (m_TextureBoxStyle = Create_TextureBoxStyle());
        public static GUIStyle Create_TextureBoxStyle()
        {
            var style = new GUIStyle(GUI.skin.box);
            style.alignment = TextAnchor.MiddleCenter;
            style.margin = new RectOffset(0, 0, 0, 0);
            style.padding.top = 2;
            return style;
        }
        public static readonly GUILayoutOption[] TextureBoxOptions = new GUILayoutOption[]
        {
            GUILayout.ExpandWidth(true),
            GUILayout.ExpandHeight(true),
        };

    }
}