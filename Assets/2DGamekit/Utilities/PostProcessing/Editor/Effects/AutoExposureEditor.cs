#region

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

#endregion

namespace UnityEditor.Rendering.PostProcessing
{
    [PostProcessEditor(typeof(AutoExposure))]
    public sealed class AutoExposureEditor : PostProcessEffectEditor<AutoExposure>
    {
        private SerializedParameterOverride m_EyeAdaptation;
        private SerializedParameterOverride m_Filtering;
        private SerializedParameterOverride m_KeyValue;
        private SerializedParameterOverride m_MaxLuminance;

        private SerializedParameterOverride m_MinLuminance;
        private SerializedParameterOverride m_SpeedDown;
        private SerializedParameterOverride m_SpeedUp;

        public override void OnEnable()
        {
            m_Filtering = FindParameterOverride(x => x.filtering);

            m_MinLuminance = FindParameterOverride(x => x.minLuminance);
            m_MaxLuminance = FindParameterOverride(x => x.maxLuminance);
            m_KeyValue = FindParameterOverride(x => x.keyValue);

            m_EyeAdaptation = FindParameterOverride(x => x.eyeAdaptation);
            m_SpeedUp = FindParameterOverride(x => x.speedUp);
            m_SpeedDown = FindParameterOverride(x => x.speedDown);
        }

        public override void OnInspectorGUI()
        {
            if (!SystemInfo.supportsComputeShaders)
                EditorGUILayout.HelpBox("Auto exposure requires compute shader support.", MessageType.Warning);

            EditorUtilities.DrawHeaderLabel("Exposure");

            PropertyField(m_Filtering);
            PropertyField(m_MinLuminance);
            PropertyField(m_MaxLuminance);

            // Clamp min/max adaptation values
            var minLum = m_MinLuminance.value.floatValue;
            var maxLum = m_MaxLuminance.value.floatValue;
            m_MinLuminance.value.floatValue = Mathf.Min(minLum, maxLum);
            m_MaxLuminance.value.floatValue = Mathf.Max(minLum, maxLum);

            PropertyField(m_KeyValue);

            EditorGUILayout.Space();
            EditorUtilities.DrawHeaderLabel("Adaptation");

            PropertyField(m_EyeAdaptation);

            if (m_EyeAdaptation.value.intValue == (int) EyeAdaptation.Progressive)
            {
                PropertyField(m_SpeedUp);
                PropertyField(m_SpeedDown);
            }
        }
    }
}