using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StartledSeal
{
    public static class FloatExtensions
    {
        public static Vector3 XOY(this Vector2 vector2) => new Vector3(vector2.x, 0, vector2.y);

        // https://catlikecoding.com/unity/tutorials/movement/reactive-environment/#3.4
        public static float SmoothStep(this float value) => 3f * value * value - 2f * value * value * value;
    }
}
