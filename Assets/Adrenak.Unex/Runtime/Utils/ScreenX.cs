using UnityEngine;

namespace Adrenak.Unex {
    public class ScreenX {
        public static int Height {
            get { return (int)WindowSize.x; }
        }

        public static int Width {
            get { return (int)WindowSize.y; }
        }

        /// <summary>
        /// Gets the current window size.
        /// In the build, this returns Screen.width, Screen.height
        /// In the editor, this returns the size of the Game Window using reflection
        /// </summary>
        public static Vector2 WindowSize {
            get {
                // Screen.width and Screen.height sometimes return the dimensions of the inspector
                // window when Screen.width originates from a ContextMenu or Button attribute
                // We use reflection to get the actual dimensions. During runtime we simply use Screen again
#if UNITY_EDITOR
                System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
                System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
                return (Vector2)Res;
#else
                return new Vector2(
                    Screen.width,
                    Screen.height
                );
#endif
            }
        }
    }
}
