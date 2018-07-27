#region

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

#endregion

namespace UnityEditor.Rendering.PostProcessing
{
    [PostProcessEditor(typeof(AmbientOcclusion))]
    public sealed class AmbientOcclusionEditor : PostProcessEffectEditor<AmbientOcclusion>
    {
        private SerializedParameterOverride m_AmbientOnly;
        private SerializedParameterOverride m_Color;
        private SerializedParameterOverride m_DirectLightingStrength;
        private SerializedParameterOverride m_Intensity;
        private SerializedParameterOverride m_Mode;
        private SerializedParameterOverride m_Quality;
        private SerializedParameterOverride m_Radius;
        private SerializedParameterOverride m_ThicknessModifier;

        public override void OnEnable()
        {
            m_Mode = FindParameterOverride(x => x.mode);
            m_Intensity = FindParameterOverride(x => x.intensity);
            m_Color = FindParameterOverride(x => x.color);
            m_AmbientOnly = FindParameterOverride(x => x.ambientOnly);
            m_ThicknessModifier = FindParameterOverride(x => x.thicknessModifier);
            m_DirectLightingStrength = FindParameterOverride(x => x.directLightingStrength);
            m_Quality = FindParameterOverride(x => x.quality);
            m_Radius = FindParameterOverride(x => x.radius);
        }

        public override void OnInspectorGUI()
        {
            PropertyField(m_Mode);
            var aoMode = m_Mode.value.intValue;

            if (RuntimeUtilities.scriptableRenderPipelineActive &&
                aoMode == (int) AmbientOcclusionMode.ScalableAmbientObscurance)
            {
                EditorGUILayout.HelpBox("Scalable ambient obscurance doesn't work with scriptable render pipelines.",
                    MessageType.Warning);
                return;
            }

#if !UNITY_2017_1_OR_NEWER
            if (aoMode == (int)AmbientOcclusionMode.MultiScaleVolumetricObscurance)
            {
                EditorGUILayout.HelpBox("Multi-scale volumetric obscurance requires Unity 2017.1 or more.", MessageType.Warning);
                return;
            }
#endif

            PropertyField(m_Intensity);

            if (aoMode == (int) AmbientOcclusionMode.ScalableAmbientObscurance)
            {
                PropertyField(m_Radius);
                PropertyField(m_Quality);
            }
            else if (aoMode == (int) AmbientOcclusionMode.MultiScaleVolumetricObscurance)
            {
                if (!SystemInfo.supportsComputeShaders)
                    EditorGUILayout.HelpBox("Multi-scale volumetric obscurance requires compute shader support.",
                        MessageType.Warning);

                PropertyField(m_ThicknessModifier);

                if (RuntimeUtilities.scriptableRenderPipelineActive)
                    PropertyField(m_DirectLightingStrength);
            }

            PropertyField(m_Color);

            if (Camera.main != null && Camera.main.actualRenderingPath == RenderingPath.DeferredShading &&
                Camera.main.allowHDR)
                PropertyField(m_AmbientOnly);
        }
    }
}