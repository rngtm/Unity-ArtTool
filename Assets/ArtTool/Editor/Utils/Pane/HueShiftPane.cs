using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;

namespace ArtTool
{
    public class HueShiftPane : PaneComponentBase
    {
        private int m_ShiftHue = 0; // (-180~+179)
        const int MinHue = -180;
        const int MaxHue = 179;
        readonly GUIContent TitleContent = new GUIContent("色相ずらし");
        readonly GUIContent ButtonContent = new GUIContent("Hue\nShift");
        readonly GUIContent ConfirmButtonContent = new GUIContent("確定");
        readonly GUIContent CancelButtonContent = new GUIContent("キャンセル");
        readonly GUIContent SliderLabelContent = new GUIContent("Hue");

        private Gradient m_InitialGradient;
        private Gradient m_OutputGradient;

        public HueShiftPane(Action<Gradient> onChangeGradient)
            : base(onChangeGradient)
        {
        }

        public override GUIContent GetButtonContent() => ButtonContent;

        public override void OnOpenPane(Gradient gradient)
        {
            base.OnOpenPane(gradient);
            m_OutputGradient = new Gradient();
            GradientUtility.Copy(m_Gradient, m_OutputGradient);
            m_ShiftHue = 0;
        }

        public override void OnChangeGradient(Gradient gradient)
        {
            base.OnChangeGradient(gradient);
            m_OutputGradient = new Gradient();
            GradientUtility.Copy(m_Gradient, m_OutputGradient);
            m_ShiftHue = 0;
        }

        public override void DrawPane()
        {
            var defaultBG = GUI.backgroundColor;
            GUI.backgroundColor = ColorRegistry.SubPaneColor; // change color
            EditorGUILayout.BeginVertical(CustomUI.SubLeftPaneBoxStyle, CustomUI.SubLeftPaneBoxOptions); // begin box
            {
                GUI.backgroundColor = defaultBG; // reset color

                EditorGUILayout.LabelField(TitleContent, CustomUI.LeftPaneTitleLabelStyle, CustomUI.LeftSubPaneTitleLabelOptions);

                EditorGUI.BeginChangeCheck();
                CustomUI.DrawLeftPaneIntSlider(SliderLabelContent, ref m_ShiftHue, MinHue, MaxHue);
                bool changed = EditorGUI.EndChangeCheck();
                if (changed)
                {
                    OnChangeHue();
                }

                EditorGUILayout.BeginHorizontal();
                /// draw buttons
                if (GUILayout.Button(ConfirmButtonContent, CustomUI.SubLeftPaneButtonStyle, CustomUI.SubLeftPaneButtonOptions))
                {
                    m_ShiftHue = 0;
                    GradientUtility.Copy(m_OutputGradient, m_Gradient);  // 現在の出力Gradientをソースにする
                    OnChangeHue();
                }
                if (GUILayout.Button(CancelButtonContent, CustomUI.SubLeftPaneButtonStyle, CustomUI.SubLeftPaneButtonOptions))
                {
                    m_ShiftHue = 0;
                    OnChangeHue();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = defaultBG;
        }

        /// <summary>
        /// グラデーションの色相ずらし
        /// </summary>
        private void OnChangeHue()
        {
            var colorKeys = new GradientColorKey[m_Gradient.colorKeys.Length];
            var alphaKeys = new GradientAlphaKey[m_Gradient.alphaKeys.Length];
            for (int colorIndex = 0; colorIndex < colorKeys.Length; colorIndex++)
            {
                var time = m_Gradient.colorKeys[colorIndex].time;
                var color = m_Gradient.colorKeys[colorIndex].color;
                float h, s, v;
                Color.RGBToHSV(color, out h, out s, out v);
                h = (h + m_ShiftHue / 360f + 1f) % 1f; /// 色相をズラす
                var newColor = Color.HSVToRGB(h, s, v);

                colorKeys[colorIndex] = new GradientColorKey(newColor, time);
            }


            for (int alphaIndex = 0; alphaIndex < alphaKeys.Length; alphaIndex++)
            {
                alphaKeys[alphaIndex] = m_Gradient.alphaKeys[alphaIndex];
            }

            m_OutputGradient = new Gradient();
            m_OutputGradient.colorKeys = colorKeys;
            m_OutputGradient.alphaKeys = alphaKeys;
            m_OnChangeGradient(m_OutputGradient);
        }
    }
}