#region

using System;

#endregion

namespace UnityEngine.Rendering.PostProcessing
{
    // This asset is used to store references to shaders and other resources we might need at
    // runtime without having to use a `Resources` folder. This allows for better memory management,
    // better dependency tracking and better interoperability with asset bundles.
    public sealed class PostProcessResources : ScriptableObject
    {
        public Texture2D[] blueNoise256;

        public Texture2D[] blueNoise64;
        public ComputeShaders computeShaders;
        public Shaders shaders;
        public SMAALuts smaaLuts;

        [Serializable]
        public sealed class Shaders
        {
            public Shader autoExposure;
            public Shader bloom;
            public Shader copy;
            public Shader copyStd;
            public Shader debugOverlays;
            public Shader deferredFog;
            public Shader depthOfField;
            public Shader discardAlpha;
            public Shader finalPass;
            public Shader gammaHistogram;
            public Shader grainBaker;
            public Shader lightMeter;
            public Shader lut2DBaker;
            public Shader motionBlur;
            public Shader multiScaleAO;
            public Shader scalableAO;
            public Shader screenSpaceReflections;
            public Shader subpixelMorphologicalAntialiasing;
            public Shader temporalAntialiasing;
            public Shader texture2dLerp;
            public Shader uber;
            public Shader vectorscope;
            public Shader waveform;
        }

        [Serializable]
        public sealed class ComputeShaders
        {
            public ComputeShader exposureHistogram;
            public ComputeShader gammaHistogram;
            public ComputeShader gaussianDownsample;
            public ComputeShader lut3DBaker;
            public ComputeShader multiScaleAODownsample1;
            public ComputeShader multiScaleAODownsample2;
            public ComputeShader multiScaleAORender;
            public ComputeShader multiScaleAOUpsample;
            public ComputeShader texture3dLerp;
            public ComputeShader vectorscope;
            public ComputeShader waveform;
        }

        [Serializable]
        public sealed class SMAALuts
        {
            public Texture2D area;
            public Texture2D search;
        }
    }
}