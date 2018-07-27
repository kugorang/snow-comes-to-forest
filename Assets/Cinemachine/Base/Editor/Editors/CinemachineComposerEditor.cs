#region

using Cinemachine.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

#endregion

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineComposer))]
    internal class CinemachineComposerEditor : BaseEditor<CinemachineComposer>
    {
        private CinemachineScreenComposerGuides mScreenGuideEditor;

        protected virtual void OnEnable()
        {
            mScreenGuideEditor = new CinemachineScreenComposerGuides();
            mScreenGuideEditor.GetHardGuide = () => { return Target.HardGuideRect; };
            mScreenGuideEditor.GetSoftGuide = () => { return Target.SoftGuideRect; };
            mScreenGuideEditor.SetHardGuide = r => { Target.HardGuideRect = r; };
            mScreenGuideEditor.SetSoftGuide = r => { Target.SoftGuideRect = r; };
            mScreenGuideEditor.Target = () => { return serializedObject; };

            Target.OnGUICallback += OnGUI;
            InternalEditorUtility.RepaintAllViews();
        }

        protected virtual void OnDisable()
        {
            if (Target != null)
                Target.OnGUICallback -= OnGUI;
            InternalEditorUtility.RepaintAllViews();
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            if (Target.LookAtTarget == null)
                EditorGUILayout.HelpBox(
                    "A LookAt target is required.  Change Aim to Do Nothing if you don't want a LookAt target.",
                    MessageType.Warning);

            // First snapshot some settings
            var oldHard = Target.HardGuideRect;
            var oldSoft = Target.SoftGuideRect;

            // Draw the properties
            DrawRemainingPropertiesInInspector();
            mScreenGuideEditor.SetNewBounds(oldHard, oldSoft, Target.HardGuideRect, Target.SoftGuideRect);
        }

        protected virtual void OnGUI()
        {
            // Draw the camera guides
            if (!Target.IsValid || !CinemachineSettings.CinemachineCoreSettings.ShowInGameGuides)
                return;

            var brain = CinemachineCore.Instance.FindPotentialTargetBrain(Target.VirtualCamera);
            if (brain == null || brain.OutputCamera.activeTexture != null)
                return;

            var isLive = CinemachineCore.Instance.IsLive(Target.VirtualCamera);

            // Screen guides
            mScreenGuideEditor.OnGUI_DrawGuides(isLive, brain.OutputCamera, Target.VcamState.Lens, true);

            // Draw an on-screen gizmo for the target
            if (Target.LookAtTarget != null && isLive)
            {
                var targetScreenPosition = brain.OutputCamera.WorldToScreenPoint(Target.TrackedPoint);
                if (targetScreenPosition.z > 0)
                {
                    targetScreenPosition.y = Screen.height - targetScreenPosition.y;

                    GUI.color = CinemachineSettings.ComposerSettings.TargetColour;
                    var r = new Rect(targetScreenPosition, Vector2.zero);
                    var size = (CinemachineSettings.ComposerSettings.TargetSize
                                + CinemachineScreenComposerGuides.kGuideBarWidthPx) / 2;
                    GUI.DrawTexture(r.Inflated(new Vector2(size, size)), Texture2D.whiteTexture);
                    size -= CinemachineScreenComposerGuides.kGuideBarWidthPx;
                    if (size > 0)
                    {
                        var overlayOpacityScalar
                            = new Vector4(1f, 1f, 1f, CinemachineSettings.ComposerSettings.OverlayOpacity);
                        GUI.color = Color.black * overlayOpacityScalar;
                        GUI.DrawTexture(r.Inflated(new Vector2(size, size)), Texture2D.whiteTexture);
                    }
                }
            }
        }
    }
}