using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace BrokenVector
{
    public class FastGrid : EditorWindow
    {

        private static EditorWindow window;
        private Texture2D logo;
        private GUISkin skin;

        private const string root = "Assets/BrokenVector/FastGrid/Editor/";
        private const string logoPath = root + "Skin/Images/logo.png";
        private const string skinPath = root + "Skin/skin.guiskin";

        private const float darkerColor = 0.8f;
        private static Color red = new Color(darkerColor, 0f, 0f, 1f);
        private static Color green = new Color(0f, darkerColor, 0f, 1f);
        private static Color blue = new Color(0f, 0f, darkerColor, 1f);

        public static bool snapEnabled = false;
        public static bool drawEnabled = true;

        private static float gridLines = 3;
        private static float gridSize = 0.25f;
        private static float rotateSize = 45f;

        private Vector3 posBefore = Vector3.zero;
        private Vector3 scaleBefore = Vector3.zero;
        private static Quaternion rotBefore;

        private static bool drawPosX, drawPosY, drawPosZ;
        private static bool drawScaleX, drawScaleY, drawScaleZ;

        private static bool forceRotationMode;
        private static PivotRotation rotationModeBefore;

        private static float GridSize
        {
            get
            {
                return gridSize;
            }
            set
            {
                gridSize = value;
                EditorPrefs.SetFloat("FastGrid_gridSize", gridSize);
            }
        }
        private static float PixelsPerUnit
        {
            get
            {
                return 1.0f / gridSize;
            }
            set
            {
                gridSize = 1.0f / value;
            }
        }

        void Awake()
        {
            logo = AssetDatabase.LoadAssetAtPath(logoPath, typeof(Texture2D)) as Texture2D;
            skin = AssetDatabase.LoadAssetAtPath(skinPath, typeof(GUISkin)) as GUISkin;

            snapEnabled = EditorPrefs.GetBool("FastGrid_snapEnabled", false);
            drawEnabled = EditorPrefs.GetBool("FastGrid_drawEnabled", false);
            GridSize = EditorPrefs.GetFloat("FastGrid_gridSize", 0.25f);
            PixelsPerUnit = 1 / GridSize;
            rotateSize = EditorPrefs.GetFloat("FastGrid_rotateSize", 45f);
        }

        [MenuItem("Window/BrokenVector/FastGrid")]
        public static void GetWindow()
        {
            window = EditorWindow.GetWindow<FastGrid>();
            window.title = "FastGrid";
            window.minSize = new Vector2(120, 270);
            window.maxSize = window.minSize;
        }

        [DrawGizmo(GizmoType.Active)]
        private static void DrawGrid(Transform transform, GizmoType gizmoType)
        {
            if (!(snapEnabled && drawEnabled)) return;
            if (Tools.current != Tool.Move && Tools.current != Tool.Scale) return;
            if (transform != Selection.transforms[Selection.transforms.Length - 1]) return;

            if (!SceneView.lastActiveSceneView.in2DMode)
            {
                if (drawPosX || drawPosZ || drawScaleX || drawScaleZ)
                {
                    DrawTranslationGrid(transform.position, transform.position.x, red, Vector3.right, Vector3.forward);
                    DrawTranslationGrid(transform.position, transform.position.z, blue, Vector3.forward, Vector3.right);
                }
                if (drawPosY || drawScaleY)
                {
                    Vector3 camRight = SceneView.currentDrawingSceneView.camera.transform.right;
                    DrawTranslationGrid(transform.position, transform.position.y, green, Vector3.up, camRight);
                }
            }
            else
            {
                if (drawPosX || drawPosY || drawScaleX || drawScaleZ)
                {
                    DrawTranslationGrid(transform.position, transform.position.x, red, Vector3.right, Vector3.up);
                    DrawTranslationGrid(transform.position, transform.position.y, green, Vector3.up, Vector3.right);
                }
            }
        }

        public static void DrawTranslationGrid(Vector3 pos, float dir, Color color, Vector3 levelX, Vector3 levelY)
        {
            for (float x = -gridLines / 2; x <= gridLines / 2; x += GridSize)
            {
                float xx = x + dir;
                if ((int)xx == xx)
                {
                    color.a = 1.0f;
                }
                else
                {
                    color.a = 0.3f;
                }

                Handles.color = color;
                Handles.DrawLine(pos + levelX * x - levelY * 1.5f, pos + levelX * x + levelY * 1.5f);
            }
        }

        void OnGUI()
        {
            if (skin != null) GUI.skin = skin;

            GUILayout.BeginVertical();

            if (logo != null)
            {
                GUI.DrawTexture(new Rect(10, 5, 100, 30), logo, ScaleMode.ScaleToFit);
                GUILayout.Space(45);
            }

            int fontSize = GUI.skin.label.fontSize;
            GUI.skin.label.fontSize = 18;

            #region snap toggle
            GUILayout.BeginHorizontal();
            GUILayout.Label("Snap");
            bool snapBefore = snapEnabled;
            snapEnabled = GUILayout.Toggle(snapEnabled, GUIContent.none, GUILayout.MinWidth(60), GUILayout.MinHeight(29));
            if (snapEnabled != snapBefore)
            {
                EditorPrefs.SetBool("FastGrid_snapEnabled", snapEnabled);
            }
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(5);

            #region grid toggle
            GUILayout.BeginHorizontal();
            GUILayout.Label("Grid");
            bool drawBefore = drawEnabled;
            drawEnabled = GUILayout.Toggle(drawEnabled && snapEnabled, GUIContent.none, GUILayout.MinWidth(60), GUILayout.MinHeight(29));
            if (drawEnabled != drawBefore)
            {
                EditorPrefs.SetBool("FastGrid_drawEnabled", drawEnabled);
            }
            GUILayout.EndHorizontal();
            #endregion

            GUI.skin.label.fontSize = fontSize;

            GUILayout.Space(10);

            #region grid size
            GUILayout.BeginVertical();
            GUILayout.Label("Grid Size");
            float tempGridSize = EditorGUILayout.FloatField(GridSize);
            if (tempGridSize != 0 && !float.IsInfinity(tempGridSize) && tempGridSize >= 0.01f && tempGridSize <= 1f)
                GridSize = tempGridSize;
            GUILayout.EndVertical();
            #endregion

            GUILayout.Space(10);

            #region pixels per unit
            GUILayout.BeginVertical();
            GUILayout.Label("Pixels Per Unit");
            float tempPPU = EditorGUILayout.FloatField(PixelsPerUnit);
            if (tempPPU != 0 && !float.IsInfinity(tempPPU))
                PixelsPerUnit = tempPPU;
            GUILayout.EndVertical();
            #endregion

            #region rotation snap
            GUILayout.BeginVertical();
            GUILayout.Label("Rotation Snap");
            float tempRotateSize = EditorGUILayout.FloatField(rotateSize);
            if (tempRotateSize != 0 && !float.IsInfinity(tempRotateSize))
            {
                rotateSize = tempRotateSize;
                EditorPrefs.SetFloat("FastGrid_rotateSize", rotateSize);
            }
            GUILayout.EndVertical();
            #endregion

            GUILayout.EndHorizontal();
        }

        void Update()
        {
            if (!snapEnabled) return;
            if (Selection.transforms.Length <= 0) return;
            Transform first = Selection.transforms[0];

            if (posBefore != first.position)
            {
                SnapToGrid(Selection.transforms);
                posBefore = first.position;
            }

            if (scaleBefore != first.localScale)
            {
                ScaleToGrid(Selection.transforms);
                scaleBefore = first.localScale;
            }

            if (Tools.current == Tool.Rotate)
            {
                if (!forceRotationMode)
                {
                    rotationModeBefore = Tools.pivotRotation;
                    Tools.pivotRotation = PivotRotation.Local;
                    forceRotationMode = true;
                }
            }
            else if (forceRotationMode)
            {
                forceRotationMode = false;
                Tools.pivotRotation = rotationModeBefore;
            }

            if (rotBefore != first.rotation)
            {
                RotateToAngle(Selection.transforms);
                rotBefore = first.rotation;
            }
        }

        private void SnapToGrid(Transform[] transforms)
        {
            foreach (Transform t in transforms)
            {
                Vector3 pos = t.position;

                Undo.RecordObject(t, "Snap to Grid");
                float newX = RoundToS(t.position.x, GridSize);
                float newY = RoundToS(t.position.y, GridSize);
                float newZ = RoundToS(t.position.z, GridSize);

                drawPosX = pos.x != newX;
                drawPosY = pos.y != newY;
                drawPosZ = pos.z != newZ;

                t.position = new Vector3(newX, newY, newZ);
                EditorUtility.SetDirty(t);
            }
        }

        private void RotateToAngle(Transform[] transforms)
        {
            foreach (Transform t in transforms)
            {
                Quaternion rotQuat = t.rotation;
                Vector3 rot = rotQuat.eulerAngles;

                Undo.RecordObject(t, "Snap to Angle");
                float newX = RoundToS(rot.x, rotateSize);
                float newY = RoundToS(rot.y, rotateSize);
                float newZ = RoundToS(rot.z, rotateSize);

                rot.x = newX;
                rot.y = newY;
                rot.z = newZ;

                t.rotation = Quaternion.Euler(rot);
                EditorUtility.SetDirty(t);
            }
        }

        private void ScaleToGrid(Transform[] transforms)
        {
            foreach (Transform t in transforms)
            {
                Vector3 scale = t.localScale;

                Undo.RecordObject(t, "Scale to Grid");
                float newX = RoundToS(scale.x, gridSize);
                float newY = RoundToS(scale.y, gridSize);
                float newZ = RoundToS(scale.z, gridSize);

                drawScaleX = scale.x != newX;
                drawScaleY = scale.y != newY;
                drawScaleZ = scale.z != newZ;

                t.localScale = new Vector3(newX, newY, newZ);
                EditorUtility.SetDirty(t);
            }
        }

        private float RoundToS(float f, float s)
        {
            return (Mathf.RoundToInt(f / s) * s);
        }

    }
}
