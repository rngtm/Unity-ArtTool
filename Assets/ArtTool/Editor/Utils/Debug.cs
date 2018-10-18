using System;

namespace ArtTool
{
    internal static class Debug
    {
        static readonly System.Action<string> EmptyAction = (s) => {};
        static readonly System.Action<string> LogAction = (s) => UnityEngine.Debug.Log(s);
        static System.Action<string> m_LogAction = LogAction;

        public static void SetActive(bool active)
        {
            if (active)
            {
                m_LogAction = LogAction;
            }
            else
            {
                m_LogAction = EmptyAction;
            }
        }
        public static void Log(string message)
        {
            m_LogAction(message);
        }

        internal static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}