using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;

namespace ArtTool
{
    public class HueRandomizerPane : PaneComponentBase
    {
        readonly GUIContent TitleContent = new GUIContent("色相ランダム化");
        readonly GUIContent ButtonContent = new GUIContent("Hue\nRandomizer");
        //readonly GUIContent RandomizeButtonContent = new GUIContent("色相ランダマイズ");
        readonly GUIContent RandomizeButtonContent = new GUIContent("実行");
        //readonly GUIContent RandomizeButtonContent = new GUIContent("Randomize Hue");

        public HueRandomizerPane(Action<Gradient> onChangeGradient)
            : base(onChangeGradient)
        {
        }

        public override GUIContent GetButtonContent() => ButtonContent;

        public override void DrawPane()
        {
            var defaultBG = GUI.backgroundColor;
            GUI.backgroundColor = ColorRegistry.SubPaneColor; // change color
            EditorGUILayout.BeginVertical(CustomUI.SubLeftPaneBoxStyle, CustomUI.SubLeftPaneBoxOptions); // begin box
            {
                EditorGUILayout.LabelField(TitleContent, CustomUI.LeftPaneTitleLabelStyle, CustomUI.LeftSubPaneTitleLabelOptions);

                GUI.backgroundColor = defaultBG; // reset color
                if (GUILayout.Button(RandomizeButtonContent, CustomUI.SubLeftPaneButtonStyle, CustomUI.SubLeftPaneButtonOptions))
                {
                    RandomizeGradientColor();
                }
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = defaultBG;
        }

        /// <summary>
        /// 色相ランダム化
        /// </summary>
        private void RandomizeGradientColor()
        {
            int colorCount = m_Gradient.colorKeys.Length;
            var colorKeys = new GradientColorKey[colorCount];
            for (int colorIndex = 0; colorIndex < colorCount; colorIndex++)
            {
                var time = m_Gradient.colorKeys[colorIndex].time;
                var color = m_Gradient.colorKeys[colorIndex].color;
                float h, s, v;
                Color.RGBToHSV(color, out h, out s, out v);
                h = Random.value;
                var newColor = Color.HSVToRGB(h, s, v);

                colorKeys[colorIndex] = new GradientColorKey(newColor, time);
            }

            //var newGradient = new Gradient();
            m_Gradient.colorKeys = colorKeys;
            m_OnChangeGradient(m_Gradient);
        }

    }
}