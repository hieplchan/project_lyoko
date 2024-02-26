using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StartledSeal
{
    public static class VectorExtensions
    {
        public static Vector3 XOY(this Vector2 vector2) => new Vector3(vector2.x, 0, vector2.y);
    }
}
