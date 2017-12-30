using UnityEngine;

namespace UniPrep.Extensions {
    public static class UnityAPIExtensions {
        // RECT
        public static Rect SetX(this Rect rect, float val) {
            return new Rect(val, rect.y, rect.width, rect.height);
        }

        public static Rect SetY(this Rect rect, float val) {
            return new Rect(rect.x, val, rect.width, rect.height);
        }

        public static Rect SetW(this Rect rect, float val) {
            return new Rect(rect.x, rect.y, val, rect.height);
        }

        public static Rect SetH(this Rect rect, float val) {
            return new Rect(rect.x, rect.y, rect.width, val);
        }

        // VECTOR3
        public static Vector3 SetX(this Vector3 v, float val) {
            return new Vector3(val, v.y, v.z);
        }

        public static Vector3 SetY(this Vector3 v, float val) {
            return new Vector3(v.x, val, v.z);
        }

        public static Vector3 SetZ(this Vector3 v, float val) {
            return new Vector3(v.x, v.y, val);
        }

        // VECTOR2
        public static Vector2 SetX(this Vector2 v, float val) {
            return new Vector2(val, v.y);
        }

        public static Vector2 SetY(this Vector2 v, float val) {
            return new Vector2(v.x, val);
        }

        // GAME OBJECTS
        public static void Destroy(this GameObject gameObject) {
            MonoBehaviour.Destroy(gameObject);
        }

        public static void DestroyImmediate(this GameObject gameObject) {
            MonoBehaviour.DestroyImmediate(gameObject);
        }

        // VIEW GROUP
        public static void Set(this CanvasGroup group, bool status) {
            group.interactable = group.blocksRaycasts = status;
            group.alpha = status ? 1 : 0;
        }
    }
}
