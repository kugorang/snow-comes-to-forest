#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

#endregion

namespace UnityEditor
{
    internal class StandardParticlesShaderGUI : ShaderGUI
    {
        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade, // Old school alpha-blending mode, fresnel does not affect amount of transparency
            Transparent, // Physically plausible transparency mode, implemented as alpha pre-multiply
            Additive,
            Subtractive,
            Modulate
        }

        public enum ColorMode
        {
            Multiply,
            Additive,
            Subtractive,
            Overlay,
            Screen,
            Color,
            Difference
        }

        public enum FlipbookMode
        {
            Simple,
            Blended
        }

        private readonly ColorPickerHDRConfig m_ColorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 1 / 99f, 3f);
        private readonly List<ParticleSystemRenderer> m_RenderersUsingThisMaterial = new List<ParticleSystemRenderer>();

        private MaterialProperty albedoColor;
        private MaterialProperty albedoMap;
        private MaterialProperty alphaCutoff;

        private MaterialProperty blendMode;
        private MaterialProperty bumpMap;
        private MaterialProperty bumpScale;
        private MaterialProperty cameraFadingEnabled;
        private MaterialProperty cameraFarFadeDistance;
        private MaterialProperty cameraNearFadeDistance;
        private MaterialProperty colorMode;
        private MaterialProperty cullMode;
        private MaterialProperty distortionBlend;
        private MaterialProperty distortionEnabled;
        private MaterialProperty distortionStrength;
        private MaterialProperty emissionColorForRendering;
        private MaterialProperty emissionEnabled;
        private MaterialProperty emissionMap;
        private MaterialProperty flipbookMode;

        private bool m_FirstTimeApply = true;

        private MaterialEditor m_MaterialEditor;
        private MaterialProperty metallic;
        private MaterialProperty metallicMap;
        private MaterialProperty smoothness;
        private MaterialProperty softParticlesEnabled;
        private MaterialProperty softParticlesFarFadeDistance;
        private MaterialProperty softParticlesNearFadeDistance;

        public void FindProperties(MaterialProperty[] props)
        {
            blendMode = FindProperty("_Mode", props);
            colorMode = FindProperty("_ColorMode", props, false);
            flipbookMode = FindProperty("_FlipbookMode", props);
            cullMode = FindProperty("_Cull", props);
            distortionEnabled = FindProperty("_DistortionEnabled", props);
            distortionStrength = FindProperty("_DistortionStrength", props);
            distortionBlend = FindProperty("_DistortionBlend", props);
            albedoMap = FindProperty("_MainTex", props);
            albedoColor = FindProperty("_Color", props);
            alphaCutoff = FindProperty("_Cutoff", props);
            metallicMap = FindProperty("_MetallicGlossMap", props, false);
            metallic = FindProperty("_Metallic", props, false);
            smoothness = FindProperty("_Glossiness", props, false);
            bumpScale = FindProperty("_BumpScale", props);
            bumpMap = FindProperty("_BumpMap", props);
            emissionEnabled = FindProperty("_EmissionEnabled", props);
            emissionColorForRendering = FindProperty("_EmissionColor", props);
            emissionMap = FindProperty("_EmissionMap", props);
            softParticlesEnabled = FindProperty("_SoftParticlesEnabled", props);
            cameraFadingEnabled = FindProperty("_CameraFadingEnabled", props);
            softParticlesNearFadeDistance = FindProperty("_SoftParticlesNearFadeDistance", props);
            softParticlesFarFadeDistance = FindProperty("_SoftParticlesFarFadeDistance", props);
            cameraNearFadeDistance = FindProperty("_CameraNearFadeDistance", props);
            cameraFarFadeDistance = FindProperty("_CameraFarFadeDistance", props);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            FindProperties(
                props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
            m_MaterialEditor = materialEditor;
            var material = materialEditor.target as Material;

            // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
            // material to a standard shader.
            // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
            if (m_FirstTimeApply)
            {
                MaterialChanged(material);
                CacheRenderersUsingThisMaterial(material);
                m_FirstTimeApply = false;
            }

            ShaderPropertiesGUI(material);
        }

        public void ShaderPropertiesGUI(Material material)
        {
            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            {
                GUILayout.Label(Styles.blendingOptionsText, EditorStyles.boldLabel);

                BlendModePopup();
                ColorModePopup();

                EditorGUILayout.Space();
                GUILayout.Label(Styles.mainOptionsText, EditorStyles.boldLabel);

                FlipbookModePopup();
                TwoSidedPopup(material);
                FadingPopup(material);
                DistortionPopup(material);

                EditorGUILayout.Space();
                GUILayout.Label(Styles.mapsOptionsText, EditorStyles.boldLabel);

                DoAlbedoArea(material);
                DoSpecularMetallicArea(material);
                DoNormalMapArea(material);
                DoEmissionArea(material);

                if (!flipbookMode.hasMixedValue && (FlipbookMode) flipbookMode.floatValue != FlipbookMode.Blended)
                {
                    EditorGUI.BeginChangeCheck();
                    m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);
                    if (EditorGUI.EndChangeCheck())
                        emissionMap.textureScaleAndOffset =
                            albedoMap
                                .textureScaleAndOffset; // Apply the main texture scale and offset to the emission texture as well, for Enlighten's sake
                }
            }
            if (EditorGUI.EndChangeCheck())
                foreach (var obj in blendMode.targets)
                    MaterialChanged((Material) obj);

            EditorGUILayout.Space();

            GUILayout.Label(Styles.advancedOptionsText, EditorStyles.boldLabel);
            m_MaterialEditor.RenderQueueField();

            EditorGUILayout.Space();

            GUILayout.Label(Styles.requiredVertexStreamsText, EditorStyles.boldLabel);
            DoVertexStreamsArea(material);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            // Sync the lighting flag for the unlit shader
            if (newShader.name.Contains("Unlit"))
                material.SetFloat("_LightingEnabled", 0.0f);
            else
                material.SetFloat("_LightingEnabled", 1.0f);

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission")) material.SetColor("_EmissionColor", material.GetColor("_Emission"));

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialWithBlendMode(material, (BlendMode) material.GetFloat("_Mode"));
                return;
            }

            var blendMode = BlendMode.Opaque;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
                blendMode = BlendMode.Cutout;
            else if (oldShader.name.Contains("/Transparent/")) blendMode = BlendMode.Fade;
            material.SetFloat("_Mode", (float) blendMode);

            MaterialChanged(material);
        }

        private void BlendModePopup()
        {
            EditorGUI.showMixedValue = blendMode.hasMixedValue;
            var mode = (BlendMode) blendMode.floatValue;

            EditorGUI.BeginChangeCheck();
            mode = (BlendMode) EditorGUILayout.Popup(Styles.renderingMode, (int) mode, Styles.blendNames);
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
                blendMode.floatValue = (float) mode;
            }

            EditorGUI.showMixedValue = false;
        }

        private void ColorModePopup()
        {
            if (colorMode != null)
            {
                EditorGUI.showMixedValue = colorMode.hasMixedValue;
                var mode = (ColorMode) colorMode.floatValue;

                EditorGUI.BeginChangeCheck();
                mode = (ColorMode) EditorGUILayout.Popup(Styles.colorMode, (int) mode, Styles.colorNames);
                if (EditorGUI.EndChangeCheck())
                {
                    m_MaterialEditor.RegisterPropertyChangeUndo("Color Mode");
                    colorMode.floatValue = (float) mode;
                }

                EditorGUI.showMixedValue = false;
            }
        }

        private void FlipbookModePopup()
        {
            EditorGUI.showMixedValue = flipbookMode.hasMixedValue;
            var mode = (FlipbookMode) flipbookMode.floatValue;

            EditorGUI.BeginChangeCheck();
            mode = (FlipbookMode) EditorGUILayout.Popup(Styles.flipbookMode, (int) mode, Styles.flipbookNames);
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("Flip-Book Mode");
                flipbookMode.floatValue = (float) mode;
            }

            EditorGUI.showMixedValue = false;
        }

        private void TwoSidedPopup(Material material)
        {
            EditorGUI.showMixedValue = cullMode.hasMixedValue;
            var enabled = cullMode.floatValue == (float) CullMode.Off;

            EditorGUI.BeginChangeCheck();
            enabled = EditorGUILayout.Toggle(Styles.twoSidedEnabled, enabled);
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("Two Sided Enabled");
                cullMode.floatValue = enabled ? (float) CullMode.Off : (float) CullMode.Back;
            }

            EditorGUI.showMixedValue = false;
        }

        private void FadingPopup(Material material)
        {
            // Z write doesn't work with fading
            var hasZWrite = material.GetInt("_ZWrite") != 0;
            if (!hasZWrite)
            {
                // Soft Particles
                {
                    EditorGUI.showMixedValue = softParticlesEnabled.hasMixedValue;
                    var enabled = softParticlesEnabled.floatValue;

                    EditorGUI.BeginChangeCheck();
                    enabled = EditorGUILayout.Toggle(Styles.softParticlesEnabled, enabled != 0.0f) ? 1.0f : 0.0f;
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_MaterialEditor.RegisterPropertyChangeUndo("Soft Particles Enabled");
                        softParticlesEnabled.floatValue = enabled;
                    }

                    if (enabled != 0.0f)
                    {
                        var indentation = 2;
                        m_MaterialEditor.ShaderProperty(softParticlesNearFadeDistance,
                            Styles.softParticlesNearFadeDistanceText, indentation);
                        m_MaterialEditor.ShaderProperty(softParticlesFarFadeDistance,
                            Styles.softParticlesFarFadeDistanceText, indentation);
                    }
                }

                // Camera Fading
                {
                    EditorGUI.showMixedValue = cameraFadingEnabled.hasMixedValue;
                    var enabled = cameraFadingEnabled.floatValue;

                    EditorGUI.BeginChangeCheck();
                    enabled = EditorGUILayout.Toggle(Styles.cameraFadingEnabled, enabled != 0.0f) ? 1.0f : 0.0f;
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_MaterialEditor.RegisterPropertyChangeUndo("Camera Fading Enabled");
                        cameraFadingEnabled.floatValue = enabled;
                    }

                    if (enabled != 0.0f)
                    {
                        var indentation = 2;
                        m_MaterialEditor.ShaderProperty(cameraNearFadeDistance, Styles.cameraNearFadeDistanceText,
                            indentation);
                        m_MaterialEditor.ShaderProperty(cameraFarFadeDistance, Styles.cameraFarFadeDistanceText,
                            indentation);
                    }
                }

                EditorGUI.showMixedValue = false;
            }
        }

        private void DistortionPopup(Material material)
        {
            // Z write doesn't work with distortion
            var hasZWrite = material.GetInt("_ZWrite") != 0;
            if (!hasZWrite)
            {
                EditorGUI.showMixedValue = distortionEnabled.hasMixedValue;
                var enabled = distortionEnabled.floatValue != 0.0f;

                EditorGUI.BeginChangeCheck();
                enabled = EditorGUILayout.Toggle(Styles.distortionEnabled, enabled);
                if (EditorGUI.EndChangeCheck())
                {
                    m_MaterialEditor.RegisterPropertyChangeUndo("Distortion Enabled");
                    distortionEnabled.floatValue = enabled ? 1.0f : 0.0f;
                }

                if (enabled)
                {
                    var indentation = 2;
                    m_MaterialEditor.ShaderProperty(distortionStrength, Styles.distortionStrengthText, indentation);
                    m_MaterialEditor.ShaderProperty(distortionBlend, Styles.distortionBlendText, indentation);
                }

                EditorGUI.showMixedValue = false;
            }
        }

        private void DoAlbedoArea(Material material)
        {
            m_MaterialEditor.TexturePropertyWithHDRColor(Styles.albedoText, albedoMap, albedoColor,
                m_ColorPickerHDRConfig, true);
            if ((BlendMode) material.GetFloat("_Mode") == BlendMode.Cutout)
                m_MaterialEditor.ShaderProperty(alphaCutoff, Styles.alphaCutoffText,
                    MaterialEditor.kMiniTextureFieldLabelIndentLevel);
        }

        private void DoEmissionArea(Material material)
        {
            // Emission
            EditorGUI.showMixedValue = emissionEnabled.hasMixedValue;
            var enabled = emissionEnabled.floatValue != 0.0f;

            EditorGUI.BeginChangeCheck();
            enabled = EditorGUILayout.Toggle(Styles.emissionEnabled, enabled);
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("Emission Enabled");
                emissionEnabled.floatValue = enabled ? 1.0f : 0.0f;
            }

            if (enabled)
            {
                var hadEmissionTexture = emissionMap.textureValue != null;

                // Texture and HDR color controls
                m_MaterialEditor.TexturePropertyWithHDRColor(Styles.emissionText, emissionMap,
                    emissionColorForRendering, m_ColorPickerHDRConfig, false);

                // If texture was assigned and color was black set color to white
                var brightness = emissionColorForRendering.colorValue.maxColorComponent;
                if (emissionMap.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                    emissionColorForRendering.colorValue = Color.white;
            }
        }

        private void DoSpecularMetallicArea(Material material)
        {
            if (metallicMap == null)
                return;

            var useLighting = material.GetFloat("_LightingEnabled") > 0.0f;
            if (useLighting)
            {
                var hasGlossMap = metallicMap.textureValue != null;
                m_MaterialEditor.TexturePropertySingleLine(Styles.metallicMapText, metallicMap,
                    hasGlossMap ? null : metallic);

                var indentation = 2; // align with labels of texture properties
                var showSmoothnessScale = hasGlossMap;
                m_MaterialEditor.ShaderProperty(smoothness,
                    showSmoothnessScale ? Styles.smoothnessScaleText : Styles.smoothnessText, indentation);
            }
        }

        private void DoNormalMapArea(Material material)
        {
            var hasZWrite = material.GetInt("_ZWrite") != 0;
            var useLighting = material.GetFloat("_LightingEnabled") > 0.0f;
            var useDistortion = material.GetFloat("_DistortionEnabled") > 0.0f && !hasZWrite;
            if (useLighting || useDistortion)
                m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapText, bumpMap,
                    bumpMap.textureValue != null ? bumpScale : null);
        }

        private void DoVertexStreamsArea(Material material)
        {
            // Display list of streams required to make this shader work
            var useLighting = material.GetFloat("_LightingEnabled") > 0.0f;
            var useFlipbookBlending = material.GetFloat("_FlipbookMode") > 0.0f;
            var useTangents = material.GetTexture("_BumpMap") && useLighting;

            GUILayout.Label(Styles.streamPositionText, EditorStyles.label);

            if (useLighting)
                GUILayout.Label(Styles.streamNormalText, EditorStyles.label);

            GUILayout.Label(Styles.streamColorText, EditorStyles.label);
            GUILayout.Label(Styles.streamUVText, EditorStyles.label);

            if (useFlipbookBlending)
            {
                GUILayout.Label(Styles.streamUV2Text, EditorStyles.label);
                GUILayout.Label(Styles.streamAnimBlendText, EditorStyles.label);
            }

            if (useTangents)
                GUILayout.Label(Styles.streamTangentText, EditorStyles.label);

            // Build the list of expected vertex streams
            var streams = new List<ParticleSystemVertexStream>();
            streams.Add(ParticleSystemVertexStream.Position);

            if (useLighting)
                streams.Add(ParticleSystemVertexStream.Normal);

            streams.Add(ParticleSystemVertexStream.Color);
            streams.Add(ParticleSystemVertexStream.UV);

            if (useFlipbookBlending)
            {
                streams.Add(ParticleSystemVertexStream.UV2);
                streams.Add(ParticleSystemVertexStream.AnimBlend);
            }

            if (useTangents)
                streams.Add(ParticleSystemVertexStream.Tangent);

            // Set the streams on all systems using this material
            if (GUILayout.Button(Styles.streamApplyToAllSystemsText, EditorStyles.miniButton,
                GUILayout.ExpandWidth(false)))
                foreach (var renderer in m_RenderersUsingThisMaterial)
                    renderer.SetActiveVertexStreams(streams);

            // Display a warning if any renderers have incorrect vertex streams
            var Warnings = "";
            var rendererStreams = new List<ParticleSystemVertexStream>();
            foreach (var renderer in m_RenderersUsingThisMaterial)
            {
                renderer.GetActiveVertexStreams(rendererStreams);
                if (!rendererStreams.SequenceEqual(streams))
                    Warnings += "  " + renderer.name + "\n";
            }

            if (Warnings != "")
                EditorGUILayout.HelpBox(
                    "The following Particle System Renderers are using this material with incorrect Vertex Streams:\n" +
                    Warnings + "Use the Apply to Systems button to fix this", MessageType.Warning, true);

            EditorGUILayout.Space();
        }

        public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    material.SetOverrideTag("RenderType", "");
                    material.SetInt("_BlendOp", (int) BlendOp.Add);
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.DisableKeyword("_ALPHAMODULATE_ON");
                    material.renderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetInt("_BlendOp", (int) BlendOp.Add);
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.DisableKeyword("_ALPHAMODULATE_ON");
                    material.renderQueue = (int) RenderQueue.AlphaTest;
                    break;
                case BlendMode.Fade:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_BlendOp", (int) BlendOp.Add);
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.DisableKeyword("_ALPHAMODULATE_ON");
                    material.renderQueue = (int) RenderQueue.Transparent;
                    break;
                case BlendMode.Transparent:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_BlendOp", (int) BlendOp.Add);
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.DisableKeyword("_ALPHAMODULATE_ON");
                    material.renderQueue = (int) RenderQueue.Transparent;
                    break;
                case BlendMode.Additive:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_BlendOp", (int) BlendOp.Add);
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.DisableKeyword("_ALPHAMODULATE_ON");
                    material.renderQueue = (int) RenderQueue.Transparent;
                    break;
                case BlendMode.Subtractive:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_BlendOp", (int) BlendOp.ReverseSubtract);
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.DisableKeyword("_ALPHAMODULATE_ON");
                    material.renderQueue = (int) RenderQueue.Transparent;
                    break;
                case BlendMode.Modulate:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_BlendOp", (int) BlendOp.Multiply);
                    material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.DstColor);
                    material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.EnableKeyword("_ALPHAMODULATE_ON");
                    material.renderQueue = (int) RenderQueue.Transparent;
                    break;
            }
        }

        public static void SetupMaterialWithColorMode(Material material, ColorMode colorMode)
        {
            switch (colorMode)
            {
                case ColorMode.Multiply:
                    material.DisableKeyword("_COLOROVERLAY_ON");
                    material.DisableKeyword("_COLORCOLOR_ON");
                    material.DisableKeyword("_COLORDIFFERENCE_ON");
                    material.DisableKeyword("_COLORADDITIVE_ON");
                    material.DisableKeyword("_COLORSUBTRACTIVE_ON");
                    material.DisableKeyword("_COLORSCREEN_ON");
                    break;
                case ColorMode.Overlay:
                    material.DisableKeyword("_COLORCOLOR_ON");
                    material.DisableKeyword("_COLORSCREEN_ON");
                    material.DisableKeyword("_COLORDIFFERENCE_ON");
                    material.DisableKeyword("_COLORADDITIVE_ON");
                    material.DisableKeyword("_COLORSUBTRACTIVE_ON");
                    material.EnableKeyword("_COLOROVERLAY_ON");
                    break;
                case ColorMode.Color:
                    material.DisableKeyword("_COLOROVERLAY_ON");
                    material.DisableKeyword("_COLORSCREEN_ON");
                    material.DisableKeyword("_COLORDIFFERENCE_ON");
                    material.DisableKeyword("_COLORADDITIVE_ON");
                    material.DisableKeyword("_COLORSUBTRACTIVE_ON");
                    material.EnableKeyword("_COLORCOLOR_ON");
                    break;
                case ColorMode.Screen:
                    material.DisableKeyword("_COLOROVERLAY_ON");
                    material.DisableKeyword("_COLORCOLOR_ON");
                    material.DisableKeyword("_COLORDIFFERENCE_ON");
                    material.DisableKeyword("_COLORADDITIVE_ON");
                    material.DisableKeyword("_COLORSUBTRACTIVE_ON");
                    material.EnableKeyword("_COLORSCREEN_ON");
                    break;
                case ColorMode.Difference:
                    material.DisableKeyword("_COLOROVERLAY_ON");
                    material.DisableKeyword("_COLORCOLOR_ON");
                    material.DisableKeyword("_COLORSCREEN_ON");
                    material.DisableKeyword("_COLORADDITIVE_ON");
                    material.DisableKeyword("_COLORSUBTRACTIVE_ON");
                    material.EnableKeyword("_COLORDIFFERENCE_ON");
                    break;
                case ColorMode.Additive:
                    material.DisableKeyword("_COLOROVERLAY_ON");
                    material.DisableKeyword("_COLORCOLOR_ON");
                    material.DisableKeyword("_COLORSCREEN_ON");
                    material.DisableKeyword("_COLORDIFFERENCE_ON");
                    material.DisableKeyword("_COLORSUBTRACTIVE_ON");
                    material.EnableKeyword("_COLORADDITIVE_ON");
                    break;
                case ColorMode.Subtractive:
                    material.DisableKeyword("_COLOROVERLAY_ON");
                    material.DisableKeyword("_COLORCOLOR_ON");
                    material.DisableKeyword("_COLORSCREEN_ON");
                    material.DisableKeyword("_COLORDIFFERENCE_ON");
                    material.DisableKeyword("_COLORADDITIVE_ON");
                    material.EnableKeyword("_COLORSUBTRACTIVE_ON");
                    break;
            }
        }

        private void SetMaterialKeywords(Material material)
        {
            // Z write doesn't work with distortion/fading
            var hasZWrite = material.GetInt("_ZWrite") != 0;

            // Lit shader?
            var useLighting = material.GetFloat("_LightingEnabled") > 0.0f;

            // Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
            // (MaterialProperty value might come from renderer material property block)
            var useDistortion = material.GetFloat("_DistortionEnabled") > 0.0f && !hasZWrite;
            SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap") && (useLighting || useDistortion));
            SetKeyword(material, "_METALLICGLOSSMAP", material.GetTexture("_MetallicGlossMap") != null && useLighting);

            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            SetKeyword(material, "_EMISSION", material.GetFloat("_EmissionEnabled") > 0.0f);

            // Set the define for flipbook blending
            var useFlipbookBlending = material.GetFloat("_FlipbookMode") > 0.0f;
            SetKeyword(material, "_REQUIRE_UV2", useFlipbookBlending);

            // Clamp fade distances
            var useSoftParticles = material.GetFloat("_SoftParticlesEnabled") > 0.0f;
            var useCameraFading = material.GetFloat("_CameraFadingEnabled") > 0.0f;
            var softParticlesNearFadeDistance = material.GetFloat("_SoftParticlesNearFadeDistance");
            var softParticlesFarFadeDistance = material.GetFloat("_SoftParticlesFarFadeDistance");
            var cameraNearFadeDistance = material.GetFloat("_CameraNearFadeDistance");
            var cameraFarFadeDistance = material.GetFloat("_CameraFarFadeDistance");

            if (softParticlesNearFadeDistance < 0.0f)
            {
                softParticlesNearFadeDistance = 0.0f;
                material.SetFloat("_SoftParticlesNearFadeDistance", 0.0f);
            }

            if (softParticlesFarFadeDistance < 0.0f)
            {
                softParticlesFarFadeDistance = 0.0f;
                material.SetFloat("_SoftParticlesFarFadeDistance", 0.0f);
            }

            if (cameraNearFadeDistance < 0.0f)
            {
                cameraNearFadeDistance = 0.0f;
                material.SetFloat("_CameraNearFadeDistance", 0.0f);
            }

            if (cameraFarFadeDistance < 0.0f)
            {
                cameraFarFadeDistance = 0.0f;
                material.SetFloat("_CameraFarFadeDistance", 0.0f);
            }

            // Set the define for fading
            var useFading = (useSoftParticles || useCameraFading) && !hasZWrite;
            SetKeyword(material, "_FADING_ON", useFading);
            if (useSoftParticles)
                material.SetVector("_SoftParticleFadeParams",
                    new Vector4(softParticlesNearFadeDistance,
                        1.0f / (softParticlesFarFadeDistance - softParticlesNearFadeDistance), 0.0f, 0.0f));
            else
                material.SetVector("_SoftParticleFadeParams", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
            if (useCameraFading)
                material.SetVector("_CameraFadeParams",
                    new Vector4(cameraNearFadeDistance, 1.0f / (cameraFarFadeDistance - cameraNearFadeDistance), 0.0f,
                        0.0f));
            else
                material.SetVector("_CameraFadeParams", new Vector4(0.0f, Mathf.Infinity, 0.0f, 0.0f));

            // Set the define for distortion + grabpass
            SetKeyword(material, "_DISTORTION_ON", useDistortion);
            material.SetShaderPassEnabled("Always", useDistortion);
            if (useDistortion)
                material.SetFloat("_DistortionStrengthScaled",
                    material.GetFloat("_DistortionStrength") *
                    0.1f); // more friendly number scale than 1 unit per size of the screen
        }

        private void MaterialChanged(Material material)
        {
            SetupMaterialWithBlendMode(material, (BlendMode) material.GetFloat("_Mode"));
            if (colorMode != null)
                SetupMaterialWithColorMode(material, (ColorMode) material.GetFloat("_ColorMode"));
            SetMaterialKeywords(material);
        }

        private void CacheRenderersUsingThisMaterial(Material material)
        {
            m_RenderersUsingThisMaterial.Clear();

            var renderers = Object.FindObjectsOfType(typeof(ParticleSystemRenderer)) as ParticleSystemRenderer[];
            foreach (var renderer in renderers)
                if (renderer.sharedMaterial == material)
                    m_RenderersUsingThisMaterial.Add(renderer);
        }

        private static void SetKeyword(Material m, string keyword, bool state)
        {
            if (state)
                m.EnableKeyword(keyword);
            else
                m.DisableKeyword(keyword);
        }

        private static class Styles
        {
            public static readonly GUIContent albedoText =
                new GUIContent("Albedo", "Albedo (RGB) and Transparency (A).");

            public static readonly GUIContent alphaCutoffText =
                new GUIContent("Alpha Cutoff", "Threshold for alpha cutoff.");

            public static readonly GUIContent metallicMapText =
                new GUIContent("Metallic", "Metallic (R) and Smoothness (A).");

            public static readonly GUIContent smoothnessText = new GUIContent("Smoothness", "Smoothness value.");

            public static readonly GUIContent smoothnessScaleText =
                new GUIContent("Smoothness", "Smoothness scale factor.");

            public static readonly GUIContent normalMapText = new GUIContent("Normal Map", "Normal Map.");
            public static readonly GUIContent emissionText = new GUIContent("Color", "Emission (RGB).");

            public static readonly GUIContent renderingMode = new GUIContent("Rendering Mode",
                "Determines the transparency and blending method for drawing the object to the screen.");

            public static readonly GUIContent[] blendNames =
                Array.ConvertAll(Enum.GetNames(typeof(BlendMode)), item => new GUIContent(item));

            public static readonly GUIContent colorMode = new GUIContent("Color Mode",
                "Determines the blending mode between the particle color and the texture albedo.");

            public static readonly GUIContent[] colorNames =
                Array.ConvertAll(Enum.GetNames(typeof(ColorMode)), item => new GUIContent(item));

            public static readonly GUIContent flipbookMode = new GUIContent("Flip-Book Mode",
                "Determines the blending mode used for animated texture sheets.");

            public static readonly GUIContent[] flipbookNames =
                Array.ConvertAll(Enum.GetNames(typeof(FlipbookMode)), item => new GUIContent(item));

            public static readonly GUIContent twoSidedEnabled = new GUIContent("Two Sided",
                "Render both front and back faces of the particle geometry.");

            public static readonly GUIContent distortionEnabled = new GUIContent("Enable Distortion",
                "Use a grab pass and normal map to simulate refraction.");

            public static readonly GUIContent distortionStrengthText =
                new GUIContent("Strength", "Distortion Strength.");

            public static readonly GUIContent distortionBlendText =
                new GUIContent("Blend", "Weighting between albedo and grab pass.");

            public static readonly GUIContent softParticlesEnabled = new GUIContent("Enable Soft Particles",
                "Fade out particle geometry when it gets close to the surface of objects written into the depth buffer.");

            public static readonly GUIContent softParticlesNearFadeDistanceText =
                new GUIContent("Near fade", "Soft Particles near fade distance.");

            public static readonly GUIContent softParticlesFarFadeDistanceText =
                new GUIContent("Far fade", "Soft Particles far fade distance.");

            public static readonly GUIContent cameraFadingEnabled = new GUIContent("Enable Camera Fading",
                "Fade out particle geometry when it gets close to the camera.");

            public static readonly GUIContent cameraNearFadeDistanceText =
                new GUIContent("Near fade", "Camera near fade distance.");

            public static readonly GUIContent cameraFarFadeDistanceText =
                new GUIContent("Far fade", "Camera far fade distance.");

            public static readonly GUIContent emissionEnabled = new GUIContent("Emission");

            public static readonly string blendingOptionsText = "Blending Options";
            public static readonly string mainOptionsText = "Main Options";
            public static readonly string mapsOptionsText = "Maps";
            public static readonly string advancedOptionsText = "Advanced Options";
            public static readonly string requiredVertexStreamsText = "Required Vertex Streams";

            public static readonly string streamPositionText = "Position (POSITION.xyz)";
            public static readonly string streamNormalText = "Normal (NORMAL.xyz)";
            public static readonly string streamColorText = "Color (COLOR.xyzw)";
            public static readonly string streamUVText = "UV (TEXCOORD0.xy)";
            public static readonly string streamUV2Text = "UV2 (TEXCOORD0.zw)";
            public static readonly string streamAnimBlendText = "AnimBlend (TEXCOORD1.x)";
            public static readonly string streamTangentText = "Tangent (TANGENT.xyzw)";

            public static readonly GUIContent streamApplyToAllSystemsText = new GUIContent("Apply to Systems",
                "Apply the vertex stream layout to all Particle Systems using this material");
        }
    }
} // namespace UnityEditor