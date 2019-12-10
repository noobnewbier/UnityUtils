using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace UnityUtils
{
    public static class TextureExtensions
    {
        #region scrap anc clean up fromhttps://forum.unity.com/threads/rotate-a-texture-with-an-arbitrary-angle.23904/

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static Texture2D RotateTexture(this Texture2D tex, float angle)
        {
            var rotImage = new Texture2D(tex.width, tex.height);
            int x;

            var w = tex.width;
            var h = tex.height;
            var x0 = rot_x(angle, -w / 2.0f, -h / 2.0f) + w / 2.0f;
            var y0 = rot_y(angle, -w / 2.0f, -h / 2.0f) + h / 2.0f;

            var dx_x = rot_x(angle, 1.0f, 0.0f);
            var dx_y = rot_y(angle, 1.0f, 0.0f);
            var dy_x = rot_x(angle, 0.0f, 1.0f);
            var dy_y = rot_y(angle, 0.0f, 1.0f);


            var x1 = x0;
            var y1 = y0;

            for (x = 0; x < tex.width; x++)
            {
                var x2 = x1;
                var y2 = y1;
                int y;
                for (y = 0; y < tex.height; y++)
                {          
                    x2 += dx_x; //rot_x(angle, x1, y1);
                    y2 += dx_y; //rot_y(angle, x1, y1);
                    rotImage.SetPixel((int) Mathf.Floor(x), (int) Mathf.Floor(y), GetPixel(tex, x2, y2));
                }

                x1 += dy_x;
                y1 += dy_y;
            }

            rotImage.Apply();
            return rotImage;
        }

        private static Color GetPixel(Texture2D tex, float x, float y)
        {
            Color pix;
            var x1 = (int) Mathf.Floor(x);
            var y1 = (int) Mathf.Floor(y);

            if (x1 > tex.width || x1 < 0 ||
                y1 > tex.height || y1 < 0)
            {
                pix = Color.clear;
            }
            else
            {
                pix = tex.GetPixel(x1, y1);
            }

            return pix;
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