using System;
using UnityEngine;

namespace ArtTool
{
    public abstract class PaneComponentBase
    {
        static readonly GUIContent EmptyContent = new GUIContent("");
        protected Action<Gradient> m_OnChangeGradient;
        protected Gradient m_Gradient;

        public PaneComponentBase(Action<Gradient> onChangeGradient)
        {
            m_OnChangeGradient = onChangeGradient;
        }

        public virtual void OnChangeGradient(Gradient gradient)
        {
            m_Gradient = new Gradient();
            m_Gradient.colorKeys = gradient.colorKeys;
            m_Gradient.alphaKeys = gradient.alphaKeys;
        }

        public virtual void OnOpenPane(Gradient gradient)
        {
            m_Gradient = new Gradient();
            m_Gradient.colorKeys = gradient.colorKeys;
            m_Gradient.alphaKeys = gradient.alphaKeys;
        }

        /// <summary>
        /// GUIの描画
        /// </summary>
        public virtual void DrawPane()
        {
        }

        /// <summary>
        /// ボタンラベルの取得
        /// </summary>
        public virtual GUIContent GetButtonContent()
        {
            return EmptyContent;
        }
    }

}