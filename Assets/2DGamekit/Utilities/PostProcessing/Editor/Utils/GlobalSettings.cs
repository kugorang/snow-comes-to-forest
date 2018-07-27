#region

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

#endregion

namespace UnityEditor.Rendering.PostProcessing
{
    internal static class GlobalSettings
    {
        private static bool m_Loaded;

        private static float m_TrackballSensitivity = 0.2f;

        private static Color m_VolumeGizmoColor = new Color(0.2f, 0.8f, 0.1f, 0.5f);

        private static int m_CurrentChannelMixer;

        private static int m_CurrentCurve;

        static GlobalSettings()
        {
            Load();
        }

        internal static float trackballSensitivity
        {
            get { return m_TrackballSensitivity; }
            set { TrySave(ref m_TrackballSensitivity, value, Keys.trackballSensitivity); }
        }

        internal static Color volumeGizmoColor
        {
            get { return m_VolumeGizmoColor; }
            set { TrySave(ref m_VolumeGizmoColor, value, Keys.volumeGizmoColor); }
        }

        internal static int currentChannelMixer
        {
            get { return m_CurrentChannelMixer; }
            set { TrySave(ref m_CurrentChannelMixer, value, Keys.currentChannelMixer); }
        }

        internal static int currentCurve
        {
            get { return m_CurrentCurve; }
            set { TrySave(ref m_CurrentCurve, value, Keys.currentCurve); }
        }

        [PreferenceItem("PostProcessing")]
        private static void PreferenceGUI()
        {
            if (!m_Loaded)
                Load();

            EditorGUILayout.Space();

            trackballSensitivity = EditorGUILayout.Slider("Trackballs Sensitivity", trackballSensitivity, 0.05f, 1f);
            volumeGizmoColor = EditorGUILayout.ColorField("Volume Gizmo Color", volumeGizmoColor);
        }

        private static void Load()
        {
            m_TrackballSensitivity = EditorPrefs.GetFloat(Keys.trackballSensitivity, 0.2f);
            m_VolumeGizmoColor = GetColor(Keys.volumeGizmoColor, new Color(0.2f, 0.8f, 0.1f, 0.5f));
            m_CurrentChannelMixer = EditorPrefs.GetInt(Keys.currentChannelMixer, 0);
            m_CurrentCurve = EditorPrefs.GetInt(Keys.currentCurve, 0);

            m_Loaded = true;
        }

        private static Color GetColor(string key, Color defaultValue)
        {
            var value = EditorPrefs.GetInt(key, (int) ColorUtilities.ToHex(defaultValue));
            return ColorUtilities.ToRGBA((uint) value);
        }

        private static void TrySave<T>(ref T field, T newValue, string key)
        {
            if (field.Equals(newValue))
                return;

            if (typeof(T) == typeof(float))
                EditorPrefs.SetFloat(key, (float) (object) newValue);
            else if (typeof(T) == typeof(int))
                EditorPrefs.SetInt(key, (int) (object) newValue);
            else if (typeof(T) == typeof(bool))
                EditorPrefs.SetBool(key, (bool) (object) newValue);
            else if (typeof(T) == typeof(string))
                EditorPrefs.SetString(key, (string) (object) newValue);
            else if (typeof(T) == typeof(Color))
                EditorPrefs.SetInt(key, (int) ColorUtilities.ToHex((Color) (object) newValue));

            field = newValue;
        }

        private static class Keys
        {
            internal const string trackballSensitivity = "PostProcessing.Trackball.Sensitivity";
            internal const string volumeGizmoColor = "PostProcessing.Volume.GizmoColor";
            internal const string currentChannelMixer = "PostProcessing.ChannelMixer.CurrentChannel";
            internal const string currentCurve = "PostProcessing.Curve.Current";
        }
    }
}