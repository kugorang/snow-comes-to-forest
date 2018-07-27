#region

using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEditor;
using UnityEngine;

#endregion

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineMixingCamera))]
    internal sealed class CinemachineMixingCameraEditor
        : CinemachineVirtualCameraBaseEditor<CinemachineMixingCamera>
    {
        protected override List<string> GetExcludedPropertiesInInspector()
        {
            var excluded = base.GetExcludedPropertiesInInspector();
            for (var i = 0; i < CinemachineMixingCamera.MaxCameras; ++i)
                excluded.Add(WeightPropertyName(i));
            return excluded;
        }

        private static string WeightPropertyName(int i)
        {
            return "m_Weight" + i;
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            DrawHeaderInInspector();
            DrawRemainingPropertiesInInspector();

            float totalWeight = 0;
            var children = Target.ChildCameras;
            var numCameras = Mathf.Min(CinemachineMixingCamera.MaxCameras, children.Length);
            for (var i = 0; i < numCameras; ++i)
                if (children[i].isActiveAndEnabled)
                    totalWeight += Target.GetWeight(i);

            if (numCameras == 0)
            {
                EditorGUILayout.HelpBox("There are no Virtual Camera children", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Child Camera Weights", EditorStyles.boldLabel);
                for (var i = 0; i < numCameras; ++i)
                {
                    var prop = serializedObject.FindProperty(WeightPropertyName(i));
                    if (prop != null)
                        EditorGUILayout.PropertyField(prop, new GUIContent(children[i].Name));
                }

                serializedObject.ApplyModifiedProperties();

                if (totalWeight <= UnityVectorExtensions.Epsilon)
                    EditorGUILayout.HelpBox("No input channels are active", MessageType.Warning);

                if (children.Length > numCameras)
                    EditorGUILayout.HelpBox(
                        "There are " + children.Length
                                     + " child cameras.  A maximum of " + numCameras + " is supported.",
                        MessageType.Warning);

                // Camera proportion indicator
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Mix Result", EditorStyles.boldLabel);
                DrawProportionIndicator(children, numCameras, totalWeight);
            }

            // Extensions
            DrawExtensionsWidgetInInspector();
        }

        private void DrawProportionIndicator(
            CinemachineVirtualCameraBase[] children, int numCameras, float totalWeight)
        {
            var style = EditorStyles.centeredGreyMiniLabel;
            var bkg = new Color(0.27f, 0.27f, 0.27f); // ack! no better way than this?
            var fg = Color.Lerp(CinemachineBrain.GetSoloGUIColor(), bkg, 0.8f);
            var totalHeight = (style.lineHeight + style.margin.vertical) * numCameras;
            var r = EditorGUILayout.GetControlRect(true, totalHeight);
            r.height /= numCameras;
            r.height -= 1;
            var fullWidth = r.width;
            for (var i = 0; i < numCameras; ++i)
            {
                float p = 0;
                var label = children[i].Name;
                if (totalWeight > UnityVectorExtensions.Epsilon)
                {
                    if (children[i].isActiveAndEnabled)
                        p = Target.GetWeight(i) / totalWeight;
                    else
                        label += " (disabled)";
                }

                r.width = fullWidth * p;
                EditorGUI.DrawRect(r, fg);

                var r2 = r;
                r2.x += r.width;
                r2.width = fullWidth - r.width;
                EditorGUI.DrawRect(r2, bkg);

                r.width = fullWidth;
                EditorGUI.LabelField(r, label, style);

                r.y += r.height + 1;
            }
        }
    }
}