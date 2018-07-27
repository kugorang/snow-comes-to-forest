#region

using System.Collections.Generic;
using Cinemachine.Editor;
using Cinemachine.Utility;
using UnityEditor;
using UnityEngine;

#endregion

namespace Cinemachine
{
    [CustomEditor(typeof(CinemachineFreeLook))]
    internal sealed class CinemachineFreeLookEditor
        : CinemachineVirtualCameraBaseEditor<CinemachineFreeLook>
    {
        private UnityEditor.Editor[] m_editors;
        private CinemachineVirtualCameraBase[] m_rigs;

        private string[] RigNames;

        protected override List<string> GetExcludedPropertiesInInspector()
        {
            var excluded = base.GetExcludedPropertiesInInspector();
            excluded.Add(FieldPath(x => x.m_Orbits));
            if (!Target.m_CommonLens)
                excluded.Add(FieldPath(x => x.m_Lens));
            if (Target.m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
            {
                excluded.Add(FieldPath(x => x.m_Heading));
                excluded.Add(FieldPath(x => x.m_RecenterToTargetHeading));
            }

            return excluded;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // Must destroy child editors or we get exceptions
            if (m_editors != null)
                foreach (var e in m_editors)
                    if (e != null)
                        DestroyImmediate(e);
        }

        public override void OnInspectorGUI()
        {
            // Ordinary properties
            BeginInspector();
            DrawHeaderInInspector();
            DrawPropertyInInspector(FindProperty(x => x.m_Priority));
            DrawTargetsInInspector(FindProperty(x => x.m_Follow), FindProperty(x => x.m_LookAt));
            DrawRemainingPropertiesInInspector();

            // Orbits
            EditorGUI.BeginChangeCheck();
            var orbits = FindProperty(x => x.m_Orbits);
            for (var i = 0; i < CinemachineFreeLook.RigNames.Length; ++i)
            {
                float hSpace = 3;
                var orbit = orbits.GetArrayElementAtIndex(i);
                var rect = EditorGUILayout.GetControlRect(true);
                rect = EditorGUI.PrefixLabel(rect, new GUIContent(CinemachineFreeLook.RigNames[i]));
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width = rect.width / 2 - hSpace;

                var oldWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = rect.width / 2;
                var heightProp = orbit.FindPropertyRelative(() => Target.m_Orbits[i].m_Height);
                EditorGUI.PropertyField(rect, heightProp, new GUIContent("Height"));
                rect.x += rect.width + hSpace;
                var radiusProp = orbit.FindPropertyRelative(() => Target.m_Orbits[i].m_Radius);
                EditorGUI.PropertyField(rect, radiusProp, new GUIContent("Radius"));
                EditorGUIUtility.labelWidth = oldWidth;
            }

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            // Rigs
            UpdateRigEditors();
            for (var i = 0; i < m_editors.Length; ++i)
            {
                if (m_editors[i] == null)
                    continue;
                EditorGUILayout.Separator();
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField(RigNames[i], EditorStyles.boldLabel);
                ++EditorGUI.indentLevel;
                m_editors[i].OnInspectorGUI();
                --EditorGUI.indentLevel;
                EditorGUILayout.EndVertical();
            }

            // Extensions
            DrawExtensionsWidgetInInspector();
        }

        private void UpdateRigEditors()
        {
            RigNames = CinemachineFreeLook.RigNames;
            if (m_rigs == null)
                m_rigs = new CinemachineVirtualCameraBase[RigNames.Length];
            if (m_editors == null)
                m_editors = new UnityEditor.Editor[RigNames.Length];
            for (var i = 0; i < RigNames.Length; ++i)
            {
                var rig = Target.GetRig(i);
                if (rig == null || rig != m_rigs[i])
                {
                    m_rigs[i] = rig;
                    if (m_editors[i] != null)
                        DestroyImmediate(m_editors[i]);
                    m_editors[i] = null;
                    if (rig != null)
                        CreateCachedEditor(rig, null, ref m_editors[i]);
                }
            }
        }

        [DrawGizmo(GizmoType.Active | GizmoType.Selected, typeof(CinemachineFreeLook))]
        private static void DrawFreeLookGizmos(CinemachineFreeLook vcam, GizmoType selectionType)
        {
            // Standard frustum and logo
            CinemachineBrainEditor.DrawVirtualCameraBaseGizmos(vcam, selectionType);

            var originalGizmoColour = Gizmos.color;
            var isActiveVirtualCam = CinemachineCore.Instance.IsLive(vcam);
            Gizmos.color = isActiveVirtualCam
                ? CinemachineSettings.CinemachineCoreSettings.ActiveGizmoColour
                : CinemachineSettings.CinemachineCoreSettings.InactiveGizmoColour;

            if (vcam.Follow != null)
            {
                var pos = vcam.Follow.position;
                var up = Vector3.up;
                var brain = CinemachineCore.Instance.FindPotentialTargetBrain(vcam);
                if (brain != null)
                    up = brain.DefaultWorldUp;

                var MiddleRig = vcam.GetRig(1).GetCinemachineComponent<CinemachineOrbitalTransposer>();
                var orient = MiddleRig.GetReferenceOrientation(up);
                up = orient * Vector3.up;
                var rotation = vcam.m_XAxis.Value + vcam.m_Heading.m_HeadingBias;
                orient = Quaternion.AngleAxis(rotation, up) * orient;

                CinemachineOrbitalTransposerEditor.DrawCircleAtPointWithRadius(
                    pos + up * vcam.m_Orbits[0].m_Height, orient, vcam.m_Orbits[0].m_Radius);
                CinemachineOrbitalTransposerEditor.DrawCircleAtPointWithRadius(
                    pos + up * vcam.m_Orbits[1].m_Height, orient, vcam.m_Orbits[1].m_Radius);
                CinemachineOrbitalTransposerEditor.DrawCircleAtPointWithRadius(
                    pos + up * vcam.m_Orbits[2].m_Height, orient, vcam.m_Orbits[2].m_Radius);

                DrawCameraPath(pos, orient, vcam);
            }

            Gizmos.color = originalGizmoColour;
        }

        private static void DrawCameraPath(Vector3 atPos, Quaternion orient, CinemachineFreeLook vcam)
        {
            var prevMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(atPos, orient, Vector3.one);

            const int kNumStepsPerPair = 30;
            var currPos = vcam.GetLocalPositionForCameraFromInput(0f);
            for (var i = 1; i < kNumStepsPerPair + 1; ++i)
            {
                var t = i / (float) kNumStepsPerPair;
                var nextPos = vcam.GetLocalPositionForCameraFromInput(t);
                Gizmos.DrawLine(currPos, nextPos);
                Gizmos.DrawWireSphere(nextPos, 0.02f);
                currPos = nextPos;
            }

            Gizmos.matrix = prevMatrix;
        }

        /// <summary>
        ///     Register with CinemachineFreeLook to create the pipeline in an undo-friendly manner
        /// </summary>
        [InitializeOnLoad]
        private class CreateRigWithUndo
        {
            static CreateRigWithUndo()
            {
                CinemachineFreeLook.CreateRigOverride
                    = (vcam, name, copyFrom) =>
                    {
                        // Create a new rig with default components
                        var go = new GameObject(name);
                        Undo.RegisterCreatedObjectUndo(go, "created rig");
                        Undo.SetTransformParent(go.transform, vcam.transform, "parenting rig");
                        var rig = Undo.AddComponent<CinemachineVirtualCamera>(go);
                        Undo.RecordObject(rig, "creating rig");
                        if (copyFrom != null)
                        {
                            ReflectionHelpers.CopyFields(copyFrom, rig);
                        }
                        else
                        {
                            go = rig.GetComponentOwner().gameObject;
                            Undo.RecordObject(Undo.AddComponent<CinemachineOrbitalTransposer>(go), "creating rig");
                            Undo.RecordObject(Undo.AddComponent<CinemachineComposer>(go), "creating rig");
                        }

                        return rig;
                    };
                CinemachineFreeLook.DestroyRigOverride = rig => { Undo.DestroyObjectImmediate(rig); };
            }
        }
    }
}