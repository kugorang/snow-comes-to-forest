#region

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

#endregion

namespace UnityEditor.Rendering.PostProcessing
{
    [PostProcessEditor(typeof(ScreenSpaceReflections))]
    public sealed class ScreenSpaceReflectionsEditor : PostProcessEffectEditor<ScreenSpaceReflections>
    {
        private SerializedParameterOverride m_DistanceFade;
        private SerializedParameterOverride m_MaximumIterationCount;
        private SerializedParameterOverride m_MaximumMarchDistance;
        private SerializedParameterOverride m_Preset;
        private SerializedParameterOverride m_Resolution;
        private SerializedParameterOverride m_Thickness;
        private SerializedParameterOverride m_Vignette;

        public override void OnEnable()
        {
            m_Preset = FindParameterOverride(x => x.preset);
            m_MaximumIterationCount = FindParameterOverride(x => x.maximumIterationCount);
            m_Thickness = FindParameterOverride(x => x.thickness);
            m_Resolution = FindParameterOverride(x => x.resolution);
            m_MaximumMarchDistance = FindParameterOverride(x => x.maximumMarchDistance);
            m_DistanceFade = FindParameterOverride(x => x.distanceFade);
            m_Vignette = FindParameterOverride(x => x.vignette);
        }

        public override void OnInspectorGUI()
        {
            if (RuntimeUtilities.scriptableRenderPipelineActive)
            {
                EditorGUILayout.HelpBox("This effect doesn't work with scriptable render pipelines yet.",
                    MessageType.Warning);
                return;
            }

            if (Camera.main != null && Camera.main.actualRenderingPath != RenderingPath.DeferredShading)
                EditorGUILayout.HelpBox("This effect only works with the deferred rendering path.",
                    MessageType.Warning);

            if (!SystemInfo.supportsComputeShaders)
                EditorGUILayout.HelpBox("This effect requires compute shader support.", MessageType.Warning);

            PropertyField(m_Preset);

            if (m_Preset.value.intValue == (int) ScreenSpaceReflectionPreset.Custom)
            {
                PropertyField(m_MaximumIterationCount);
                PropertyField(m_Thickness);
                PropertyField(m_Resolution);

                EditorGUILayout.Space();
            }

            PropertyField(m_MaximumMarchDistance);
            PropertyField(m_DistanceFade);
            PropertyField(m_Vignette);
        }
    }
}