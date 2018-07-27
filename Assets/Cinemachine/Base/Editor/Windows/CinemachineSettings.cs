#region

using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

#endregion

namespace Cinemachine.Editor
{
    [InitializeOnLoad]
    internal sealed class CinemachineSettings
    {
        private const string kCinemachineHeaderPath = "cinemachine_header.tif";
        private const string kCinemachineDocURL = @"http://www.cinemachineimagery.com/documentation/";

        private static Texture2D sCinemachineLogoTexture;

        private static Texture2D sCinemachineHeader;

        private static readonly string kCoreSettingsFoldKey = "CNMCN_Core_Folded";
        private static readonly string kComposerSettingsFoldKey = "CNMCN_Composer_Folded";

        //private static readonly GUIContent sCoreShowHiddenObjectsToggle = new GUIContent("Show Hidden Objects", "If checked, Cinemachine hidden objects will be shown in the inspector.  This might be necessary to repair broken script mappings when upgrading from a pre-release version");
        private static readonly GUIContent sCoreActiveGizmosColour = new GUIContent("Active Virtual Camera",
            "The colour for the active virtual camera's gizmos");

        private static readonly GUIContent sCoreInactiveGizmosColour = new GUIContent("Inactive Virtual Camera",
            "The colour for all inactive virtual camera gizmos");

        private static readonly GUIContent sComposerOverlayOpacity = new GUIContent("Overlay Opacity",
            "The alpha of the composer's overlay when a virtual camera is selected with composer module enabled");

        private static readonly GUIContent sComposerHardBoundsOverlay = new GUIContent("Hard Bounds Overlay",
            "The colour of the composer overlay's hard bounds region");

        private static readonly GUIContent sComposerSoftBoundsOverlay = new GUIContent("Soft Bounds Overlay",
            "The colour of the composer overlay's soft bounds region");

        private static readonly GUIContent sComposerTargetOverlay =
            new GUIContent("Composer Target", "The colour of the composer overlay's target");

        private static readonly GUIContent sComposerTargetOverlayPixels = new GUIContent("Composer Target Size(px)",
            "The size of the composer overlay's target box in pixels");

        private static Vector2 sScrollPosition = Vector2.zero;

        static CinemachineSettings()
        {
            if (CinemachineLogoTexture != null) EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static bool ShowCoreSettings
        {
            get { return EditorPrefs.GetBool(kCoreSettingsFoldKey, false); }
            set
            {
                if (value != ShowCoreSettings) EditorPrefs.SetBool(kCoreSettingsFoldKey, value);
            }
        }

        private static bool ShowComposerSettings
        {
            get { return EditorPrefs.GetBool(kComposerSettingsFoldKey, false); }
            set
            {
                if (value != ShowComposerSettings) EditorPrefs.SetBool(kComposerSettingsFoldKey, value);
            }
        }

        internal static Texture2D CinemachineLogoTexture
        {
            get
            {
                if (sCinemachineLogoTexture == null)
                    sCinemachineLogoTexture = Resources.Load<Texture2D>("cm_logo_sm");
                if (sCinemachineLogoTexture != null)
                    sCinemachineLogoTexture.hideFlags = HideFlags.DontSaveInEditor;
                return sCinemachineLogoTexture;
            }
        }

        internal static Texture2D CinemachineHeader
        {
            get
            {
                if (sCinemachineHeader == null)
                    sCinemachineHeader = Resources.Load<Texture2D>("cinemachine_header");
                ;
                if (sCinemachineHeader != null)
                    sCinemachineHeader.hideFlags = HideFlags.DontSaveInEditor;
                return sCinemachineHeader;
            }
        }

        internal static event Action AdditionalCategories;

        [PreferenceItem("Cinemachine")]
        private static void OnGUI()
        {
            if (CinemachineHeader != null)
            {
                const float kWidth = 350f;
                var aspectRatio = CinemachineHeader.height / (float) CinemachineHeader.width;
                GUILayout.BeginScrollView(Vector2.zero, false, false, GUILayout.Width(kWidth),
                    GUILayout.Height(kWidth * aspectRatio));
                var texRect = new Rect(0f, 0f, kWidth, kWidth * aspectRatio);

                GUILayout.BeginArea(texRect);
                GUI.DrawTexture(texRect, CinemachineHeader, ScaleMode.ScaleToFit);
                GUILayout.EndArea();

                GUILayout.EndScrollView();
            }

            sScrollPosition = GUILayout.BeginScrollView(sScrollPosition);

            //CinemachineCore.sShowHiddenObjects
            //    = EditorGUILayout.Toggle("Show Hidden Objects", CinemachineCore.sShowHiddenObjects);

            ShowCoreSettings = EditorGUILayout.Foldout(ShowCoreSettings, "Runtime Settings");
            if (ShowCoreSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                var newActiveGizmoColour =
                    EditorGUILayout.ColorField(sCoreActiveGizmosColour, CinemachineCoreSettings.ActiveGizmoColour);

                if (EditorGUI.EndChangeCheck())
                {
                    CinemachineCoreSettings.ActiveGizmoColour = newActiveGizmoColour;
                    InternalEditorUtility.RepaintAllViews();
                }

                if (GUILayout.Button("Reset"))
                    CinemachineCoreSettings.ActiveGizmoColour = CinemachineCoreSettings.kDefaultActiveColour;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                var newInactiveGizmoColour = EditorGUILayout.ColorField(sCoreInactiveGizmosColour,
                    CinemachineCoreSettings.InactiveGizmoColour);

                if (EditorGUI.EndChangeCheck())
                {
                    CinemachineCoreSettings.InactiveGizmoColour = newInactiveGizmoColour;
                    InternalEditorUtility.RepaintAllViews();
                }

                if (GUILayout.Button("Reset"))
                    CinemachineCoreSettings.InactiveGizmoColour = CinemachineCoreSettings.kDefaultInactiveColour;
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            ShowComposerSettings = EditorGUILayout.Foldout(ShowComposerSettings, "Composer Settings");
            if (ShowComposerSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();

                var overlayOpacity =
                    EditorGUILayout.Slider(sComposerOverlayOpacity, ComposerSettings.OverlayOpacity, 0f, 1f);

                if (EditorGUI.EndChangeCheck()) ComposerSettings.OverlayOpacity = overlayOpacity;

                if (GUILayout.Button("Reset"))
                    ComposerSettings.OverlayOpacity = ComposerSettings.kDefaultOverlayOpacity;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                var newHardEdgeColor = EditorGUILayout.ColorField(sComposerHardBoundsOverlay,
                    ComposerSettings.HardBoundsOverlayColour);

                if (EditorGUI.EndChangeCheck()) ComposerSettings.HardBoundsOverlayColour = newHardEdgeColor;

                if (GUILayout.Button("Reset"))
                    ComposerSettings.HardBoundsOverlayColour = ComposerSettings.kDefaultHardBoundsColour;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                var newSoftEdgeColor = EditorGUILayout.ColorField(sComposerSoftBoundsOverlay,
                    ComposerSettings.SoftBoundsOverlayColour);

                if (EditorGUI.EndChangeCheck()) ComposerSettings.SoftBoundsOverlayColour = newSoftEdgeColor;

                if (GUILayout.Button("Reset"))
                    ComposerSettings.SoftBoundsOverlayColour = ComposerSettings.kDefaultSoftBoundsColour;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                var newTargetColour = EditorGUILayout.ColorField(sComposerTargetOverlay, ComposerSettings.TargetColour);

                if (EditorGUI.EndChangeCheck()) ComposerSettings.TargetColour = newTargetColour;

                if (GUILayout.Button("Reset")) ComposerSettings.TargetColour = ComposerSettings.kDefaultTargetColour;
                EditorGUILayout.EndHorizontal();

                EditorGUI.BeginChangeCheck();
                var targetSide = EditorGUILayout.FloatField(sComposerTargetOverlayPixels, ComposerSettings.TargetSize);

                if (EditorGUI.EndChangeCheck()) ComposerSettings.TargetSize = targetSide;
                EditorGUI.indentLevel--;
            }

            if (AdditionalCategories != null) AdditionalCategories();

            GUILayout.EndScrollView();

            //if (GUILayout.Button("Open Documentation"))
            //{
            //    Application.OpenURL(kCinemachineDocURL);
            //}
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            var instance = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (instance == null) return;

            if (instance.GetComponent<CinemachineBrain>() != null)
            {
                var texRect = new Rect(selectionRect.xMax - selectionRect.height, selectionRect.yMin,
                    selectionRect.height, selectionRect.height);
                GUI.DrawTexture(texRect, CinemachineLogoTexture, ScaleMode.ScaleAndCrop);
            }
        }

        internal static Color UnpackColour(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var bytes = Base64Decode(str);

                if (bytes != null && bytes.Length == 16)
                {
                    var r = BitConverter.ToSingle(bytes, 0);
                    var g = BitConverter.ToSingle(bytes, 4);
                    var b = BitConverter.ToSingle(bytes, 8);
                    var a = BitConverter.ToSingle(bytes, 12);

                    return new Color(r, g, b, a);
                }
            }

            return Color.white;
        }

        internal static string PackColor(Color col)
        {
            var bytes = new byte[16];

            var rBytes = BitConverter.GetBytes(col.r);
            var gBytes = BitConverter.GetBytes(col.g);
            var bBytes = BitConverter.GetBytes(col.b);
            var aBytes = BitConverter.GetBytes(col.a);

            Buffer.BlockCopy(rBytes, 0, bytes, 0, 4);
            Buffer.BlockCopy(gBytes, 0, bytes, 4, 4);
            Buffer.BlockCopy(bBytes, 0, bytes, 8, 4);
            Buffer.BlockCopy(aBytes, 0, bytes, 12, 4);

            return Base64Encode(bytes);
        }

        private static string Base64Encode(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        private static byte[] Base64Decode(string base64EncodedData)
        {
            return Convert.FromBase64String(base64EncodedData);
        }

        public static class CinemachineCoreSettings
        {
            private static readonly string hShowInGameGuidesKey = "CNMCN_Core_ShowInGameGuides";

            private static readonly string kCoreActiveGizmoColourKey = "CNMCN_Core_Active_Gizmo_Colour";
            public static readonly Color kDefaultActiveColour = new Color32(255, 0, 0, 100);

            private static readonly string kCoreInactiveGizmoColourKey = "CNMCN_Core_Inactive_Gizmo_Colour";
            public static readonly Color kDefaultInactiveColour = new Color32(9, 54, 87, 100);

            public static bool ShowInGameGuides
            {
                get { return EditorPrefs.GetBool(hShowInGameGuidesKey, true); }
                set
                {
                    if (ShowInGameGuides != value)
                    {
                        EditorPrefs.SetBool(hShowInGameGuidesKey, value);
                        InternalEditorUtility.RepaintAllViews();
                    }
                }
            }

            public static Color ActiveGizmoColour
            {
                get
                {
                    var packedColour =
                        EditorPrefs.GetString(kCoreActiveGizmoColourKey, PackColor(kDefaultActiveColour));
                    return UnpackColour(packedColour);
                }

                set
                {
                    if (ActiveGizmoColour != value)
                    {
                        var packedColour = PackColor(value);
                        EditorPrefs.SetString(kCoreActiveGizmoColourKey, packedColour);
                    }
                }
            }

            public static Color InactiveGizmoColour
            {
                get
                {
                    var packedColour =
                        EditorPrefs.GetString(kCoreInactiveGizmoColourKey, PackColor(kDefaultInactiveColour));
                    return UnpackColour(packedColour);
                }

                set
                {
                    if (InactiveGizmoColour != value)
                    {
                        var packedColour = PackColor(value);
                        EditorPrefs.SetString(kCoreInactiveGizmoColourKey, packedColour);
                    }
                }
            }
        }

        public static class ComposerSettings
        {
            public const float kDefaultOverlayOpacity = 0.15f;
            private static readonly string kOverlayOpacityKey = "CNMCN_Overlay_Opacity";
            private static readonly string kComposerHardBoundsColourKey = "CNMCN_Composer_HardBounds_Colour";
            private static readonly string kComposerSoftBoundsColourKey = "CNMCN_Composer_SoftBounds_Colour";
            private static readonly string kComposerTargetColourKey = "CNMCN_Composer_Target_Colour";
            private static readonly string kComposerTargetSizeKey = "CNMCN_Composer_Target_Size";
            public static readonly Color kDefaultHardBoundsColour = new Color32(255, 0, 72, 255);
            public static readonly Color kDefaultSoftBoundsColour = new Color32(0, 194, 255, 255);
            public static readonly Color kDefaultTargetColour = new Color32(255, 254, 25, 255);

            public static float OverlayOpacity
            {
                get { return EditorPrefs.GetFloat(kOverlayOpacityKey, kDefaultOverlayOpacity); }
                set
                {
                    if (value != OverlayOpacity) EditorPrefs.SetFloat(kOverlayOpacityKey, value);
                }
            }

            public static Color HardBoundsOverlayColour
            {
                get
                {
                    var packedColour = EditorPrefs.GetString(kComposerHardBoundsColourKey,
                        PackColor(kDefaultHardBoundsColour));
                    return UnpackColour(packedColour);
                }

                set
                {
                    if (HardBoundsOverlayColour != value)
                    {
                        var packedColour = PackColor(value);
                        EditorPrefs.SetString(kComposerHardBoundsColourKey, packedColour);
                    }
                }
            }

            public static Color SoftBoundsOverlayColour
            {
                get
                {
                    var packedColour = EditorPrefs.GetString(kComposerSoftBoundsColourKey,
                        PackColor(kDefaultSoftBoundsColour));
                    return UnpackColour(packedColour);
                }

                set
                {
                    if (SoftBoundsOverlayColour != value)
                    {
                        var packedColour = PackColor(value);
                        EditorPrefs.SetString(kComposerSoftBoundsColourKey, packedColour);
                    }
                }
            }

            public static Color TargetColour
            {
                get
                {
                    var packedColour = EditorPrefs.GetString(kComposerTargetColourKey, PackColor(kDefaultTargetColour));
                    return UnpackColour(packedColour);
                }

                set
                {
                    if (TargetColour != value)
                    {
                        var packedColour = PackColor(value);
                        EditorPrefs.SetString(kComposerTargetColourKey, packedColour);
                    }
                }
            }

            public static float TargetSize
            {
                get { return EditorPrefs.GetFloat(kComposerTargetSizeKey, 5f); }

                set
                {
                    if (TargetSize != value) EditorPrefs.SetFloat(kComposerTargetSizeKey, value);
                }
            }
        }
    }
}