#region

using System;
using UnityEngine.Assertions;

#endregion

namespace UnityEngine.Rendering.PostProcessing
{
    [Serializable]
    public sealed class Dithering
    {
        private int m_NoiseTextureIndex;

        internal void Render(PostProcessRenderContext context)
        {
            var blueNoise = context.resources.blueNoise64;
            Assert.IsTrue(blueNoise != null && blueNoise.Length > 0);

#if POSTFX_DEBUG_STATIC_DITHERING // Used by QA for automated testing
            m_NoiseTextureIndex = 0;
            float rndOffsetX = 0f;
            float rndOffsetY = 0f;
        #else
            if (++m_NoiseTextureIndex >= blueNoise.Length)
                m_NoiseTextureIndex = 0;

            var rndOffsetX = Random.value;
            var rndOffsetY = Random.value;
#endif

            var noiseTex = blueNoise[m_NoiseTextureIndex];
            var uberSheet = context.uberSheet;

            uberSheet.properties.SetTexture(ShaderIDs.DitheringTex, noiseTex);
            uberSheet.properties.SetVector(ShaderIDs.Dithering_Coords, new Vector4(
                context.screenWidth / (float) noiseTex.width,
                context.screenHeight / (float) noiseTex.height,
                rndOffsetX,
                rndOffsetY
            ));
        }
    }
}