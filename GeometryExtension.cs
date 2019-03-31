﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeometryExtesnion
{
    public static class TransformExtension
    {
        //Seriously, I forgot the math behind it. Perhaps I should ask
        public static bool InFrontOf(this Transform self, Transform value)
        {
            return Vector3.Dot((value.position - self.position).normalized, self.forward) < 0;
        }
    }
}