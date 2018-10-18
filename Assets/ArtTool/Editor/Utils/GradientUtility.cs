using UnityEngine;

namespace ArtTool
{
    public static class GradientUtility
    {
        public static void Copy(Gradient src, Gradient dst)
        {
            var colorKeys = new GradientColorKey[src.colorKeys.Length];
            for (int ci = 0; ci < src.colorKeys.Length; ci++)
            {
                colorKeys[ci] = new GradientColorKey();
                colorKeys[ci].color = src.colorKeys[ci].color;
                colorKeys[ci].time = src.colorKeys[ci].time;
            }

            var alphaKeys = new GradientAlphaKey[src.alphaKeys.Length];
            for (int ai = 0; ai < src.alphaKeys.Length; ai++)
            {
                alphaKeys[ai] = new GradientAlphaKey();
                alphaKeys[ai].time = src.alphaKeys[ai].time;
                alphaKeys[ai].alpha = src.alphaKeys[ai].alpha;
            }
            dst.SetKeys(colorKeys, alphaKeys);

            //dst.colorKeys = colorKeys;
            //dst.alphaKeys = alphaKeys;
        }
    }
}
