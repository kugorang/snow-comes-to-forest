#region

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

#endregion

namespace UnityEditor.Rendering.PostProcessing
{
    [PostProcessEditor(typeof(Vignette))]
    public sealed class VignetteEditor : PostProcessEffectEditor<Vignette>
    {
        private SerializedParameterOverride m_Center;
        private SerializedParameterOverride m_Color;
        private SerializedParameterOverride m_Intensity;

        private SerializedParameterOverride m_Mask;
        private SerializedParameterOverride m_Mode;
        private SerializedParameterOverride m_Opacity;
        private SerializedParameterOverride m_Rounded;
        private SerializedParameterOverride m_Roundness;
        private SerializedParameterOverride m_Smoothness;

        public override void OnEnable()
        {
            m_Mode = FindParameterOverride(x => x.mode);
            m_Color = FindParameterOverride(x => x.color);

            m_Center = FindParameterOverride(x => x.center);
            m_Intensity = FindParameterOverride(x => x.intensity);
            m_Smoothness = FindParameterOverride(x => x.smoothness);
            m_Roundness = FindParameterOverride(x => x.roundness);
            m_Rounded = FindParameterOverride(x => x.rounded);

            m_Mask = FindParameterOverride(x => x.mask);
            m_Opacity = FindParameterOverride(x => x.opacity);
        }

        public override void OnInspectorGUI()
        {
            PropertyField(m_Mode);
            PropertyField(m_Color);

            if (m_Mode.value.intValue == (int) VignetteMode.Classic)
            {
                PropertyField(m_Center);
                PropertyField(m_Intensity);
                PropertyField(m_Smoothness);
                PropertyField(m_Roundness);
                PropertyField(m_Rounded);
            }
            else
            {
                PropertyField(m_Mask);

                var mask = (target as Vignette).mask.value;

                // Checks import settings on the mask
                if (mask != null)
                {
                    var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mask)) as TextureImporter;

                    // Fails when using an internal texture as you can't change import settings on
                    // builtin resources, thus the check for null
                    if (importer != null)
                    {
                        var valid = importer.anisoLevel == 0
                                    && importer.mipmapEnabled == false
                                    && importer.alphaSource == TextureImporterAlphaSource.FromGrayScale
                                    && importer.textureCompression == TextureImporterCompression.Uncompressed
                                    && importer.wrapMode == TextureWrapMode.Clamp;

                        if (!valid)
                            EditorUtilities.DrawFixMeBox("Invalid mask import settings.",
                                () => SetMaskImportSettings(importer));
                    }
                }

                PropertyField(m_Opacity);
            }
        }

        private void SetMaskImportSettings(TextureImporter importer)
        {
            importer.textureType = TextureImporterType.SingleChannel;
            importer.alphaSource = TextureImporterAlphaSource.FromGrayScale;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.anisoLevel = 0;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.SaveAndReimport();
            AssetDatabase.Refresh();
        }
    }
}