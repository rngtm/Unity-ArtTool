using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;

namespace ArtTool
{
    public class SimpleRandomizerPane : PaneComponentBase
    {
        readonly GUIContent TitleContent = new GUIContent("色のランダム化");
        readonly GUIContent ButtonContent = new GUIContent("Simple\nRandomizer");
        readonly GUIContent ColorNumberContent = new GUIContent("Color N");
        //readonly GUIContent RandomizeButtonContent = new GUIContent("色のランダム化");
        readonly GUIContent RandomizeButtonContent = new GUIContent("実行");

        [SerializeField]
        private SimpleRandomizerSettings m_RandomizeSettings = new SimpleRandomizerSettings();

        public SimpleRandomizerPane(Action<Gradient> onChangeGradient)
            : base(onChangeGradient)
        {
        }

        public override GUIContent GetButtonContent()
        {
            return ButtonContent;
        }

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

                /// color number
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(ColorNumberContent, EditorStyles.whiteMiniLabel, GUILayout.Width(44f));

                    /// draw slider
                    var rect = GUILayoutUtility.GetRect(6f, 16f, CustomUI.ScaleSliderStyle, CustomUI.ScaleSliderOptions);
                    m_RandomizeSettings.GradationColorCount = Mathf.FloorToInt(GUI.HorizontalSlider(rect,
                        m_RandomizeSettings.GradationColorCount,
                        SimpleRandomizerSettings.MinColorCount,
                        SimpleRandomizerSettings.MaxColorCount
                    ));
                    EditorGUILayout.LabelField(m_RandomizeSettings.GradationColorCount.ToString(), EditorStyles.whiteMiniLabel, GUILayout.Width(16f));
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = defaultBG;
        }

        /// <summary>
        /// グラデーションランダム化
        /// </summary>
        private void RandomizeGradientColor()
        {
            var colorKeys = new GradientColorKey[m_RandomizeSettings.GradationColorCount];
            for (int colorIndex = 0; colorIndex < m_RandomizeSettings.GradationColorCount; colorIndex++)
            {
                var color = new Color(Random.value, Random.value, Random.value, 1f);
                var time = (float)colorIndex / (float)(m_RandomizeSettings.GradationColorCount - 1);
                colorKeys[colorIndex] = new GradientColorKey(color, time);
            }

            var gradient = new Gradient();
            gradient.colorKeys = colorKeys;
            gradient.alphaKeys = m_Gradient.alphaKeys;
            m_OnChangeGradient(gradient);
        }

    }
}