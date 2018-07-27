#region

using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

#endregion

namespace UnityEditor.Rendering.PostProcessing
{
    // CUBE lut specs:
    // http://wwwimages.adobe.com/content/dam/Adobe/en/products/speedgrade/cc/pdfs/cube-lut-specification-1.0.pdf
    internal static class CubeLutAssetFactory
    {
        private const int kVersion = 1;
        private const int kSize = 33;

#if POSTFX_DEBUG_MENUS
        [MenuItem("Tools/Post-processing/Create Utility Luts")]
#endif
        private static void CreateLuts()
        {
            Dump("Linear to Unity Log r" + kVersion, ColorUtilities.LinearToLogC);
            Dump("Unity Log to Linear r" + kVersion, ColorUtilities.LogCToLinear);
            Dump("sRGB to Unity Log r" + kVersion, x => ColorUtilities.LinearToLogC(Mathf.GammaToLinearSpace(x)));
            Dump("Unity Log to sRGB r" + kVersion, x => Mathf.LinearToGammaSpace(ColorUtilities.LogCToLinear(x)));
            Dump("Linear to sRGB r" + kVersion, Mathf.LinearToGammaSpace);
            Dump("sRGB to Linear r" + kVersion, Mathf.GammaToLinearSpace);

            AssetDatabase.Refresh();
        }

        private static void Dump(string title, Func<float, float> eval)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("TITLE \"{0}\"\n", title);
            sb.AppendFormat("LUT_3D_SIZE {0}\n", kSize);
            sb.AppendFormat("DOMAIN_MIN {0} {0} {0}\n", 0f);
            sb.AppendFormat("DOMAIN_MAX {0} {0} {0}\n", 1f);

            const float kSizeMinusOne = (float) kSize - 1f;

            for (var x = 0; x < kSize; x++)
            for (var y = 0; y < kSize; y++)
            for (var z = 0; z < kSize; z++)
            {
                var ox = eval(x / kSizeMinusOne);
                var oy = eval(y / kSizeMinusOne);
                var oz = eval(z / kSizeMinusOne);

                // Resolve & Photoshop use BGR as default, let's make it easier for users
                sb.AppendFormat("{0} {1} {2}\n", oz, oy, ox);
            }

            var content = sb.ToString();
            var path = Path.Combine(Application.dataPath, string.Format("{0}.cube", title));
            File.WriteAllText(path, content);
        }
    }
}