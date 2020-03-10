using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Object = System.Object;

namespace Adrenak.Unex.Ed {
    public class Hotkeys {

        // ================================================
        // GLOBAL POSITION
        // ================================================
        // Moves transform to global origin
        // USE CTRL + SHIFT + O
        [MenuItem("Edit/HotKeys/Move To Origin %#o")]
        static void MoveToOrigin() {
            if (Selection.activeTransform != null)
                Selection.activeTransform.position = new Vector3(0, 0, 0);
        }

        // Sets transforms global Y position to 0
        // USE CTRL + SHIFT + Y
        [MenuItem("Edit/HotKeys/Move To Global XZ Plane %#Y")]
        static void MoveToXZPlane() {
            if (Selection.activeTransform != null) {
                Vector3 pos = Selection.activeTransform.position;
                Selection.activeTransform.position = new Vector3(pos.x, 0, pos.z);
            }
        }

        // Sets transforms global Z position to 0
        // USE CTRL + SHIFT + Z
        [MenuItem("Edit/HotKeys/Move To Global XY Plane %#Z")]
        static void MoveToXYPlane() {
            if (Selection.activeTransform != null) {
                Vector3 pos = Selection.activeTransform.position;
                Selection.activeTransform.position = new Vector3(pos.x, pos.y, 0);
            }
        }

        // Sets transforms global X position to 0
        // USE CTRL + SHIFT + X
        [MenuItem("Edit/HotKeys/Move To Global YZ Plane %#X")]
        static void MoveToYZPlane() {
            if (Selection.activeTransform != null) {
                Vector3 pos = Selection.activeTransform.position;
                Selection.activeTransform.position = new Vector3(0, pos.y, pos.z);
            }
        }

        // ================================================
        // LOCAL POSITION
        // ================================================
        // Moves transform to local 0, 0, 0 position
        // USE CTRL + ALT + O
        [MenuItem("Edit/HotKeys/Move To Local Origin %&o")]
        static void MoveToLocalOrigin() {
            if (Selection.activeTransform != null)
                Selection.activeTransform.localPosition = new Vector3(0, 0, 0);
        }

        // Sets transforms local Y position to 0
        // USE CTRL + ALT + Y
        [MenuItem("Edit/HotKeys/Move To Local XZ Plane %&Y")]
        static void MoveToLocalXZPlane() {
            if (Selection.activeTransform != null) {
                Vector3 pos = Selection.activeTransform.position;
                Selection.activeTransform.localPosition = new Vector3(pos.x, 0, pos.z);
            }
        }

        // Sets transforms local Z position to 0
        // USE CTRL + ALT + Z
        [MenuItem("Edit/HotKeys/Move To Local XY Plane %&Z")]
        static void MoveToLocalXYPlane() {
            if (Selection.activeTransform != null) {
                Vector3 pos = Selection.activeTransform.position;
                Selection.activeTransform.localPosition = new Vector3(pos.x, pos.y, 0);
            }
        }

        // Sets transforms local X position to 0
        // USE CTRL + ALT + X
        [MenuItem("Edit/HotKeys/Move To Local YZ Plane %&X")]
        static void MoveToLocalYZPlane() {
            if (Selection.activeTransform != null) {
                Vector3 pos = Selection.activeTransform.position;
                Selection.activeTransform.localPosition = new Vector3(0, pos.y, pos.z);
            }
        }

        // ================================================
        // TRANSFORMS
        // ================================================
        // Groups the selected transforms under a new parent located at the combined center of the selection
        // PRESS CTRL + G
        [MenuItem("Edit/HotKeys/Group Transforms %G")]
        static void GroupTransforms() {
            if (Selection.transforms != null && Selection.transforms.Length > 0) {
                GameObject grouped = new GameObject("Grouped");

                // We calculate the center of the objects and move the new root there
                Vector3 posSum = new Vector3(0, 0, 0);
                foreach (Transform t in Selection.transforms)
                    posSum += t.position;
                posSum /= Selection.transforms.Length;
                grouped.transform.position = posSum;

                // Move all transforms into the new root
                foreach (Transform t in Selection.transforms)
                    t.parent = grouped.transform;
                Selection.activeTransform = grouped.transform;
            }
        }

        // Changes the scale of a transform to 1, 1, 1.
        // Changing scale may have unintended affects!
        // PRESS CTRL + SHIFT+ U
        [MenuItem("Edit/HotKeys/Scale To (1, 1, 1) %#U")]
        static void ScaleTransformToUnit() {
            Transform activeTransform = Selection.activeTransform;
            if (activeTransform == null)
                return;

            // Create a new gameobject at the position of the transform to be scaled
            GameObject createdGO = new GameObject("Unit Scaled Object");
            createdGO.transform.position = activeTransform.position;

            // Move all children of the transform to the new gameobject created above
            while (activeTransform.childCount != 0)
                activeTransform.GetChild(0).parent = createdGO.transform;

            // Scale the original transform to 1, 1, 1
            activeTransform.localScale = new Vector3(1, 1, 1);

            // Move all children back to the object again
            while (createdGO.transform.childCount != 0)
                createdGO.transform.GetChild(0).parent = activeTransform;

			// Delete the temp object
			MonoBehaviour.DestroyImmediate(createdGO);
        }

#if !UNITY_2018
		[MenuItem("Edit/HotKeys/Deselect All &A")]
		static void DeselectAll() {
			Selection.activeGameObject = null;
		}
#endif

		// ================================================
		// uGUI
		// See uGUITools.cs online
		// ================================================
		// Sets the anchors of the RectTransform to it's corner bounds
		[MenuItem("Edit/HotKeys/Anchors to Corners %[")]
        static void AnchorsToCorners() {
            RectTransform t = Selection.activeTransform as RectTransform;
            RectTransform pt = Selection.activeTransform.parent as RectTransform;

            if (t == null || pt == null)
                return;

            Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
                                                t.anchorMin.y + t.offsetMin.y / pt.rect.height);
            Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
                                                t.anchorMax.y + t.offsetMax.y / pt.rect.height);

            t.anchorMin = newAnchorsMin;
            t.anchorMax = newAnchorsMax;
            t.offsetMin = t.offsetMax = new Vector2(0, 0);
        }

        // Scale the bo9nds of the RestTransform to the anchors
        [MenuItem("Edit/HotKeys/Corners to Anchors %]")]
        static void CornersToAnchors() {
            RectTransform t = Selection.activeTransform as RectTransform;

            if (t == null)
                return;

            t.offsetMin = t.offsetMax = new Vector2(0, 0);
        }

        // ================================================
        // EDITOR
        // ================================================
        // https://forum.unity.com/threads/shortcut-key-for-lock-inspector.95815/
        private static EditorWindow _mouseOverWindow;
        [MenuItem("Edit/HotKeys/Toggle Lock &q")]
        static void ToggleInspectorLock() {
            if (_mouseOverWindow == null) {
                if (!EditorPrefs.HasKey("LockableInspectorIndex"))
                    EditorPrefs.SetInt("LockableInspectorIndex", 0);
                int i = EditorPrefs.GetInt("LockableInspectorIndex");

                Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
                Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll(type);
                _mouseOverWindow = (EditorWindow)findObjectsOfTypeAll[i];
            }

            if (_mouseOverWindow != null && _mouseOverWindow.GetType().Name == "InspectorWindow") {
                Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
                PropertyInfo propertyInfo = type.GetProperty("isLocked");
                bool value = (bool)propertyInfo.GetValue(_mouseOverWindow, null);
                propertyInfo.SetValue(_mouseOverWindow, !value, null);
                _mouseOverWindow.Repaint();
            }
        }

        [MenuItem("Edit/HotKeys/Clear Console %&c")]
        static void ClearConsole() {
            Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditorInternal.LogEntries");
            type.GetMethod("Clear").Invoke(null, null);
        }
    }
}