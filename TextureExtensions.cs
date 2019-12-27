using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace UnityUtils
{
    public static class TextureExtensions
    {
        #region scrap and clean up from https://forum.unity.com/threads/rotate-a-texture-with-an-arbitrary-angle.23904/

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static void RotateTexture(this Texture2D tex, float angle, Color defaultColor = default)
        {

            var w = tex.width;
            var h = tex.height;
            var pixelsCount = w * h;
            var originalPixels = tex.GetPixels();
            var newPixels = new Color[pixelsCount];
            
            
            var x0 = rot_x(angle, -w / 2.0f, -h / 2.0f) + w / 2.0f;
            var y0 = rot_y(angle, -w / 2.0f, -h / 2.0f) + h / 2.0f;

            var dx_x = rot_x(angle, 1.0f, 0.0f);
            var dx_y = rot_y(angle, 1.0f, 0.0f);
            var dy_x = rot_x(angle, 0.0f, 1.0f);
            var dy_y = rot_y(angle, 0.0f, 1.0f);

            var x1 = x0;
            var y1 = y0;

            for (var x = 0; x < w; x++)
            {
                var x2 = x1;
                var y2 = y1;
                for (var y = 0; y < h; y++)
                {          
                    x2 += dx_x;
                    y2 += dx_y; 

                    var x2Int = (int) x2;
                    var y2Int = (int) y2;

                    Color color;
                    
                    if (x2Int >= w || x2Int < 0 ||
                        y2Int >= h || y2Int < 0)
                    {
                        color = defaultColor;
                    }
                    else
                    {
                        color = originalPixels[y2Int * w + x2Int];
                    }
                    
                    newPixels[y * w + x] = color;
                }

                x1 += dy_x;
                y1 += dy_y;
            }
            
            tex.SetPixels(newPixels);
            tex.Apply();
        }

        private static float rot_x(float angle, float x, float y)
        {
            var cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
            var sin = Mathf.Sin(angle / 180.0f * Mathf.PI);
            return x * cos + y * -sin;
        }

        private static float rot_y(float angle, float x, float y)
        {
            var cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
            var sin = Mathf.Sin(angle / 180.0f * Mathf.PI);
            return x * sin + y * cos;
        }

        #endregion
    }
}