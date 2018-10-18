using System;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

namespace ArtTool
{
    public partial class ArtWindow : EditorWindow
    {
        static Gradient s_Gradient = null;
        static bool s_NeedBlitGradient = false;
        static Gradient DefaultGradient => new Gradient()
        {
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey
                {
                    color = Color.black,
                    time = 0f,
                },
                new GradientColorKey
                {
                    color = Color.white,
                    time = 1f,
                }
            }
        };
        readonly PaneComponentBase[] PaneTable = new PaneComponentBase[]
        {
            new SimpleRandomizerPane(gradient => OnChangeGradient(gradient)),
            new HueRandomizerPane(gradient => OnChangeGradient(gradient)),
            new HueShiftPane(gradient => OnChangeGradient(gradient)), // 色相ずらし
        };

        readonly ScaleButtonData[] ScaleButtonDatas = CustomUI.ScaleButtonDatas;
        const int m_BlitRTInterval = 2;
        const int m_BlitGradientInterval = 1; 
        const string ShaderName = "ArtTool/Unlit/GradationMap"; // グラデーションマップ用のシェーダー名
        const int TextureW = 256;
        private Texture m_SrcTexture;
        [SerializeField] private bool m_UseAlphaClip = true; // Alphaクリッピングするかどうか
        //[SerializeField] private bool m_IsOpenRandomizeSettings = false;
        [SerializeField] private Shader m_FxShader;
        [SerializeField] private Texture2D m_GradationTexture; // グラデーションテクスチャ
        [SerializeField] private RenderTexture m_RT; // マテリアル適用後のテクスチャ
        [SerializeField] private Gradient m_Gradient = DefaultGradient; // グラデーション
        [SerializeField] private int m_ScaleSelection = -1;
        [SerializeField] private SimpleRandomizerSettings m_RandomizeSettings = new SimpleRandomizerSettings();
        private Vector2 m_ScrollPosition = new Vector2(0f, 0f);
        private Material m_FxMaterial;
        private int m_UpdateCounter = 0;
        private int m_GradationTexturePropertyID = -1; // シェーダープロパティID
        private int m_UseAlphaClipPropertyID = -1; // シェーダープロパティID
        private bool m_ShowDebugLog = true;
        private PaneComponentBase m_CurrentPane = null;
        private Action m_OnGUIAction = null;

        private int MaxButtonIndex => CustomUI.ScaleButtonDatas.Length - 1;
        public Gradient Gradient => m_Gradient;

        private static void OnChangeGradient(Gradient gradient)
        {
            s_Gradient = gradient;
            s_NeedBlitGradient = true;
        }
        private void OnEnable()
        {
            Debug.SetActive(m_ShowDebugLog);

            if (m_GradationTexturePropertyID == -1) m_GradationTexturePropertyID = Shader.PropertyToID("_GradationMap");
            if (m_UseAlphaClipPropertyID == -1) m_UseAlphaClipPropertyID = Shader.PropertyToID("_UseAlphaClip");

            if (m_FxShader == null)
            {
                m_FxShader = Shader.Find(ShaderName);
                if (m_FxShader == null)
                {
                    Debug.LogError($"Shader not found: {ShaderName}");
                }
            }

            CreateRT();
            UpdateMaterial();

            if (m_ScaleSelection == -1)
            {
                for (int i = 0; i < CustomUI.ScaleButtonDatas.Length; i++)
                {
                    if (CustomUI.ScaleButtonDatas[i].IsDefault)
                    {
                        m_ScaleSelection = i;
                        break;
                    }
                }
            }

            s_NeedBlitGradient = true;
        }
        private void Update()
        {
            if (m_UpdateCounter % m_BlitRTInterval == 0)
            {
                BlitRT();
            }

            if (m_UpdateCounter % m_BlitGradientInterval == 0)
            {
                if (s_NeedBlitGradient)
                {
                    s_NeedBlitGradient = false;
                    BlitGradient();
                }
            }

            m_UpdateCounter++;
        }
        private void OnGUI()
        {
            HandleMouseWheelEvent(); // マウスホイール

            if (m_GradationTexture == null)
            {
                m_GradationTexture = new Texture2D(TextureW, 1);
            }

            DrawHeader();
            DrawInputFields();

            EditorGUILayout.BeginHorizontal();
            {
                DrawLeftPane();
                //if (m_IsOpenRandomizeSettings) { DrawSubPane_SimpleRandomizer(); }

                if (m_CurrentPane != null)
                {
                    m_CurrentPane.DrawPane();
                }

                EditorGUILayout.BeginVertical();
                {
                    DrawFooterPane();
                    DrawGUICenter();
                }
                EditorGUILayout.EndVertical();
                DrawRightPane();
            }
            EditorGUILayout.EndHorizontal();

            m_FxMaterial.SetFloat(m_UseAlphaClipPropertyID, m_UseAlphaClip ? 0.0f : 1.0f);

            if (m_OnGUIAction != null)
            {
                m_OnGUIAction.Invoke();
                //m_OnGUIAction = null;
            }
        }
        private void OnDestroy()
        {
            if (m_RT != null)
            {
                m_RT.Release();
            }

            if (m_FxMaterial != null)
            {
                DestroyImmediate(m_FxMaterial);
            }
        }
        /// <summary>
        /// 下側の枠(フッター)
        /// </summary>
        private void DrawFooterPane()
        {
            var defaultBG = GUI.backgroundColor;
            GUI.backgroundColor = ColorRegistry.FooterPaneColor;
            EditorGUILayout.BeginVertical(CustomUI.FooterPaneBoxStyle, CustomUI.FooterPaneBoxOptions); // begin box
            {
                GUI.backgroundColor = defaultBG; // reset color
                DrawInputGradient();
            }
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// マウスホイール時の処理
        /// </summary>
        private void HandleMouseWheelEvent()
        {
            if (Event.current.type != EventType.ScrollWheel) { return; }

            if (Event.current.delta.y < 0f)
            {
                m_ScaleSelection++;
                m_ScaleSelection = Mathf.Clamp(m_ScaleSelection, 0, MaxButtonIndex);
                Repaint();
            }
            else
            if (Event.current.delta.y > 0f)
            {
                m_ScaleSelection--;
                m_ScaleSelection = Mathf.Clamp(m_ScaleSelection, 0, MaxButtonIndex);
                Repaint();
            }
        }
        /// <summary>
        /// 画面中央の描画
        /// </summary>
        private void DrawGUICenter()
        {
            var defaultBG = GUI.backgroundColor;
            GUI.backgroundColor = ColorRegistry.BgColor;
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition, CustomUI.TextureBoxStyle, CustomUI.TextureBoxOptions);
            {
                GUI.backgroundColor = defaultBG;
                DrawTextures();
            }
            EditorGUILayout.EndScrollView();
        }
        /// <summary>
        /// ウィンドウ左側の枠
        /// </summary>
        private void DrawLeftPane()
        {
            var defaultBG = GUI.backgroundColor;
            GUI.backgroundColor = ColorRegistry.PaneColor;
            EditorGUILayout.BeginVertical(CustomUI.LeftPaneBoxStyle, CustomUI.LeftPaneBoxOptions); // begin box
            {
                GUI.backgroundColor = defaultBG; /// reset color

                /// button
                GUI.backgroundColor = ColorRegistry.ResetButtonColor;
                if (GUILayout.Button(CustomUI.RootButtonContent_ResetColor, CustomUI.LeftPaneButtonStyle, CustomUI.LeftPaneButtonOptions))
                {
                    ResetGradientColor();
                }
                GUI.backgroundColor = defaultBG; /// reset color

                /// toggle randomizer
                foreach (var pane in PaneTable)
                {
                    bool isSelecting = m_CurrentPane == pane;
                    bool clicked = GUILayout.Toggle(
                        isSelecting,
                        pane.GetButtonContent(),
                        CustomUI.LeftPaneButtonStyle,
                        CustomUI.LeftPaneButtonOptions) != isSelecting;

                    if (clicked)
                    {
                        if (!isSelecting)
                        {
                            if (m_CurrentPane != pane)
                            {
                                pane.OnOpenPane(m_Gradient);
                            }
                            m_CurrentPane = pane;
                        }
                        else
                        {
                            m_CurrentPane = null; /// 同じものをクリックした場合は非選択
                        }
                    }
                }

                GUILayout.FlexibleSpace();

                // toggle alpha clip
                if (GUILayout.Button(m_UseAlphaClip ? 
                    CustomUI.AlphaClipContentOn : CustomUI.AlphaClipContentOff, 
                    CustomUI.LeftPaneButtonStyle, CustomUI.LeftPaneButtonOptions))
                {
                    m_UseAlphaClip = !m_UseAlphaClip;
                }
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = defaultBG;
        }
        /// <summary>
        /// ウィンドウ右側の枠
        /// </summary>
        private void DrawRightPane()
        {
            var defaultBG = GUI.backgroundColor;
            GUI.backgroundColor = ColorRegistry.PaneColor;
            EditorGUILayout.BeginVertical(CustomUI.RightPaneBoxStyle, CustomUI.RightPaneBoxOptions); // begin box
            {
                GUI.backgroundColor = defaultBG; // reset color

                // draw scale buttons
                for (int scaleIndex = 0; scaleIndex < ScaleButtonDatas.Length; scaleIndex++)
                {
                    var scale = ScaleButtonDatas[scaleIndex];
                    bool on = scaleIndex == m_ScaleSelection;
                    if (GUILayout.Toggle(on, scale.Name, CustomUI.RightPaneButtonStyle, CustomUI.RightPaneButtonOptions) != on) // draw button
                    {
                        m_ScaleSelection = scaleIndex;
                    }
                }

                GUILayout.FlexibleSpace();

                // draw scale slider
                var rect = GUILayoutUtility.GetRect(6f, 16f, CustomUI.ScaleSliderStyle, CustomUI.ScaleSliderOptions);
                m_ScaleSelection = Mathf.RoundToInt(GUI.HorizontalSlider(rect, m_ScaleSelection, 0, MaxButtonIndex));
                m_ScaleSelection = Mathf.Clamp(m_ScaleSelection, 0, MaxButtonIndex);
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = defaultBG;
        }
        private void DrawInputGradient()
        {
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    m_Gradient = EditorGUILayout.GradientField(m_Gradient, CustomUI.GradientOptions);
                }
                EditorGUILayout.EndHorizontal();
            }

            bool change = EditorGUI.EndChangeCheck();
            if (change)
            {
                m_CurrentPane.OnChangeGradient(m_Gradient);
                s_NeedBlitGradient = true;
            }
        }
        /// <summary>
        /// グラデーションテクスチャをシェーダーに転送
        /// </summary>
        public void BlitGradient()
        {
            if (m_GradationTexture == null) { return; }

            if (s_Gradient != null)
            {
                GradientUtility.Copy(s_Gradient, m_Gradient);
                s_Gradient = null;

                Debug.Log($"use {nameof(s_Gradient)}");
            }
            else
            {
                Debug.Log($"use {nameof(m_Gradient)}");
            }


            for (int i = 0; i < TextureW; i++)
            {
                var color = m_Gradient.Evaluate((float)i / (TextureW - 1));
                m_GradationTexture.SetPixel(i, 0, color);
            }
            m_GradationTexture.Apply();
            m_FxMaterial.SetTexture(m_GradationTexturePropertyID, m_GradationTexture);
            Repaint();
        }
        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                var content = new GUIContent("Show DebugLog");
                GUILayout.FlexibleSpace();
                if (m_ShowDebugLog != GUILayout.Toggle(m_ShowDebugLog, content, EditorStyles.toolbarButton))
                {
                    m_ShowDebugLog = !m_ShowDebugLog;
                    Debug.SetActive(m_ShowDebugLog);

                    UnityEngine.Debug.Log(m_ShowDebugLog);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        private void ResetGradientColor()
        {
            s_Gradient = null;
            GradientUtility.Copy(DefaultGradient, m_Gradient);
            Repaint();
            s_NeedBlitGradient = true;
        }
        [MenuItem(EditorSettings.MENU_TEXT, false, EditorSettings.MENU_ORDER)]
        private static void Open()
        {
            GetWindow<ArtWindow>(EditorSettings.WINDOW_TITLE);
        }
        /// <summary>
        /// RenderTextureへテクスチャをコピー
        /// </summary>
        private void BlitRT()
        {
            if (m_RT == null) { return; } // m_RTがnullだとProjectビューになぜか影響が出る
            Graphics.Blit(m_SrcTexture, m_RT, m_FxMaterial);

            Repaint();
        }
        private void DrawInputFields()
        {
            EditorGUILayout.BeginVertical(CustomUI.InputFieldBoxStyle, CustomUI.InputFieldBoxOptions);
            {
                DrawInputTexture();
                DrawInputShader();
                DrawInputMaterial();
                DrawInputRenderTexture();

                GUILayout.Space(2f);
            }
            EditorGUILayout.EndVertical();
        }
        private void DrawInputRenderTexture()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.LabelField(CustomUI.InputRenderTextureLabelContent, CustomUI.InputLabelOptions);
                EditorGUILayout.ObjectField(m_RT, typeof(RenderTexture), false);
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
        }
        private void DrawInputMaterial()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.LabelField(CustomUI.InputMaterialLabelContent, EditorStyles.label, CustomUI.InputLabelOptions);
                EditorGUILayout.ObjectField(m_FxMaterial, typeof(Material), false);
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
        }
        private void DrawInputShader()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.LabelField(CustomUI.InputShaderLabelContent, EditorStyles.label, CustomUI.InputLabelOptions);
                m_FxShader = EditorGUILayout.ObjectField(m_FxShader, typeof(Shader), false) as Shader;
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
        }
        private void DrawInputTexture()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(CustomUI.InputTextureLabelContent, EditorStyles.label, CustomUI.InputLabelOptions);
                m_SrcTexture = EditorGUILayout.ObjectField(m_SrcTexture, typeof(Texture), false) as Texture;
            }
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck() && m_SrcTexture != null)
            {
                if (m_SrcTexture.width == 0 || m_SrcTexture.height == 0)
                {
                    Debug.Log(m_SrcTexture.GetType().ToString());
                    m_SrcTexture = null;
                }
                else
                {
                    Debug.Log($"Set Texture {m_SrcTexture}");
                    CreateRT();
                    s_NeedBlitGradient = true;
                }
            }
        }
        private void UpdateMaterial()
        {
            if (m_FxShader == null) return;

            if (m_FxMaterial != null)
            {
                Material.DestroyImmediate(m_FxMaterial);
            }

            m_FxMaterial = new Material(m_FxShader);
            s_NeedBlitGradient = true;
            // BlitGradient();
        }
        private void DrawTextures()
        {
            if (m_SrcTexture == null) return;
            if (m_GradationTexture == null) return;
            if (m_RT == null) return;
            if (m_FxShader == null) return;
            if (m_FxMaterial == null) return;

            DrawSourceTexture();
            //DrawGradationTexture();
            DrawRenderTexture();
        }
        private void DrawGradationTexture()
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical(CustomUI.TextureBoxStyle); /// begin box
                {
                    uint mul = ScaleButtonDatas[m_ScaleSelection].ScaleMul;
                    uint div = ScaleButtonDatas[m_ScaleSelection].ScaleDiv;
                    var rect = GUILayoutUtility.GetRect(
                        m_GradationTexture.width * mul / div,
                        //m_GradationTexture.height * mul / div,
                        m_GradationTexture.height * 32,
                        CustomUI.TextureRectStyle,
                        CustomUI.TextureRectOptions);
                    GUI.DrawTexture(rect, m_GradationTexture);
                }
                EditorGUILayout.EndVertical(); /// end box
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }
        private void DrawRenderTexture()
        {
            if (m_RT.IsCreated())
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginVertical(CustomUI.TextureBoxStyle); /// begin box
                    {
                        uint mul = ScaleButtonDatas[m_ScaleSelection].ScaleMul;
                        uint div = ScaleButtonDatas[m_ScaleSelection].ScaleDiv;
                        var rect = GUILayoutUtility.GetRect(
                            m_RT.width * mul / div,
                            m_RT.height * mul / div,
                            CustomUI.TextureRectStyle,
                            CustomUI.TextureRectOptions);
                        GUI.DrawTexture(rect, m_RT);
                    }
                    EditorGUILayout.EndVertical(); /// end box
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        private void DrawSourceTexture()
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical(CustomUI.TextureBoxStyle); /// begin box
                {
                    uint mul = ScaleButtonDatas[m_ScaleSelection].ScaleMul;
                    uint div = ScaleButtonDatas[m_ScaleSelection].ScaleDiv;
                    var rect = GUILayoutUtility.GetRect(
                        m_SrcTexture.width * mul / div,
                        m_SrcTexture.height * mul / div,
                        CustomUI.TextureRectStyle,
                        CustomUI.TextureRectOptions);
                    GUI.DrawTexture(rect, m_SrcTexture);
                }
                EditorGUILayout.EndVertical(); /// end box
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }
        private void CreateRT()
        {
            if (m_SrcTexture == null) { return; }

            if (m_RT == null)
            {
                m_RT = new RenderTexture(m_SrcTexture.width, m_SrcTexture.height, 0);
                m_RT.Create();
            }
            else
            if (m_RT.width != m_SrcTexture.width || m_RT.height != m_SrcTexture.height)
            {
                m_RT.Release();

                m_RT.width = m_SrcTexture.width;
                m_RT.height = m_SrcTexture.height;
                m_RT.Create();
            }
        }
    }
}

