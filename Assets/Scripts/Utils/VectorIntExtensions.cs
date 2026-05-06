using UnityEngine;

namespace Utils {
    public static class VectorIntExtensions {
        public static Vector2 ToVector2(this Vector2Int vector2Int) {
            return new Vector2(vector2Int.x, vector2Int.y);
        }
    }
}