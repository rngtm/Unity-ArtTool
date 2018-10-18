using System;
using UnityEngine;
using UnityEditor;

namespace ArtTool
{
    public static partial class CustomUI
    {
        const float RightPaneWidth = 66f;

        /// 右側の枠 ////////////////////////////////////////////////////////////////// 
        private static GUIStyle m_RightPaneBoxStyle;
        public static GUIStyle RightPaneBoxStyle => m_RightPaneBoxStyle ?? (m_RightPaneBoxStyle = Create_RightPaneBoxStyle());
        public static GUIStyle Create_RightPaneBoxStyle()
        {
            var style = new GUIStyle(GUI.skin.box);
            style.padding = new RectOffset(1, 1, 3, 0);
            style.margin = new RectOffset(0, 0, 0, 0);
            return style;
        }
        public static readonly GUILayoutOption[] RightPaneBoxOptions = new GUILayoutOption[]
        {
            GUILayout.Width(RightPaneWidth),
            GUILayout.ExpandHeight(true),
        };

        /// 右枠の中のボタン ////////////////////////////////////////////////////////////////// 
        private static GUIStyle m_RightPaneButtonStyle;
        public static GUIStyle RightPaneButtonStyle => m_RightPaneButtonStyle ?? (m_RightPaneButtonStyle = Create_RightPaneButtonStyle());
        public static GUIStyle Create_RightPaneButtonStyle()
        {
            var style = new GUIStyle(EditorStyles.miniButton);
            style.margin = new RectOffset(0, 0, 0, 1);
            return style;
        }
        public static readonly GUILayoutOption[] RightPaneButtonOptions = new GUILayoutOption[]
        {
            GUILayout.MaxWidth(9999f),
        };

        public static readonly ScaleButtonData[] ScaleButtonDatas = new ScaleButtonData[]
        {
            new ScaleButtonData { Name = "x0.1"  , ScaleDiv = 10 },
            new ScaleButtonData { Name = "x0.2"  , ScaleDiv = 5 },
            new ScaleButtonData { Name = "x0.25" , ScaleDiv = 4 },
            new ScaleButtonData { Name = "x0.33" , ScaleDiv = 3, IsDefault = true },
            new ScaleButtonData { Name = "x0.4"  , ScaleMul = 4, ScaleDiv = 10 },
            new ScaleButtonData { Name = "x0.5"  , ScaleDiv = 2 },
            new ScaleButtonData { Name = "x0.6"  , ScaleMul = 6, ScaleDiv = 10 },
            new ScaleButtonData { Name = "x0.75" , ScaleMul = 3, ScaleDiv = 4 },
            new ScaleButtonData { Name = "x1"    , },
            new ScaleButtonData { Name = "x1.25" , ScaleMul = 125, ScaleDiv = 100 },
            new ScaleButtonData { Name = "x1.5"  , ScaleMul = 3, ScaleDiv = 2 },
            new ScaleButtonData { Name = "x2"    , ScaleMul = 2 },
            new ScaleButtonData { Name = "x3"    , ScaleMul = 3 },
            new ScaleButtonData { Name = "x4"    , ScaleMul = 4 },
        };

    }
}
