#region

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#endregion

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineOrbitalTransposer))]
    internal class CinemachineOrbitalTransposerEditor : BaseEditor<CinemachineOrbitalTransposer>
    {
        protected override List<string> GetExcludedPropertiesInInspector()
        {
            var excluded = base.GetExcludedPropertiesInInspector();
            if (Target.m_HeadingIsSlave)
            {
                excluded.Add(FieldPath(x => x.m_FollowOffset));
                excluded.Add(FieldPath(x => x.m_BindingMode));
                excluded.Add(FieldPath(x => x.m_Heading));
                excluded.Add(FieldPath(x => x.m_XAxis));
                excluded.Add(FieldPath(x => x.m_RecenterToTargetHeading));
            }

            switch (Target.m_BindingMode)
            {
                default:
                case CinemachineTransposer.BindingMode.LockToTarget:
                    break;
                case CinemachineTransposer.BindingMode.LockToTargetNoRoll:
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    break;
                case CinemachineTransposer.BindingMode.LockToTargetWithWorldUp:
                    excluded.Add(FieldPath(x => x.m_PitchDamping));
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    break;
                case CinemachineTransposer.BindingMode.LockToTargetOnAssign:
                case CinemachineTransposer.BindingMode.WorldSpace:
                    excluded.Add(FieldPath(x => x.m_PitchDamping));
                    excluded.Add(FieldPath(x => x.m_YawDamping));
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    break;
                case CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp:
                    excluded.Add(FieldPath(x => x.m_XDamping));
                    excluded.Add(FieldPath(x => x.m_PitchDamping));
                    excluded.Add(FieldPath(x => x.m_YawDamping));
                    excluded.Add(FieldPath(x => x.m_RollDamping));
                    excluded.Add(FieldPath(x => x.m_Heading));
                    excluded.Add(FieldPath(x => x.m_RecenterToTargetHeading));
                    break;
            }

            return excluded;
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            if (Target.FollowTarget == null)
                EditorGUILayout.HelpBox(
                    "Orbital Transposer requires a Follow target.",
                    MessageType.Warning);
            DrawRemainingPropertiesInInspector();
        }

        [DrawGizmo(GizmoType.Active | GizmoType.Selected, typeof(CinemachineOrbitalTransposer))]
        private static void DrawTransposerGizmos(CinemachineOrbitalTransposer target, GizmoType selectionType)
        {
            if (target.IsValid)
            {
                var originalGizmoColour = Gizmos.color;
                Gizmos.color = CinemachineCore.Instance.IsLive(target.VirtualCamera)
                    ? CinemachineSettings.CinemachineCoreSettings.ActiveGizmoColour
                    : CinemachineSettings.CinemachineCoreSettings.InactiveGizmoColour;

                var up = Vector3.up;
                var brain = CinemachineCore.Instance.FindPotentialTargetBrain(target.VirtualCamera);
                if (brain != null)
                    up = brain.DefaultWorldUp;
                var pos = target.FollowTarget.position;

                var orient = target.GetReferenceOrientation(up);
                up = orient * Vector3.up;
                DrawCircleAtPointWithRadius
                    (pos + up * target.m_FollowOffset.y, orient, target.m_FollowOffset.z);

                Gizmos.color = originalGizmoColour;
            }
        }

        internal static void DrawCircleAtPointWithRadius(Vector3 point, Quaternion orient, float radius)
        {
            var prevMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(point, orient, radius * Vector3.one);

            const int kNumPoints = 25;
            var currPoint = Vector3.forward;
            var rot = Quaternion.AngleAxis(360f / kNumPoints, Vector3.up);
            for (var i = 0; i < kNumPoints + 1; ++i)
            {
                var nextPoint = rot * currPoint;
                Gizmos.DrawLine(currPoint, nextPoint);
                currPoint = nextPoint;
            }

            Gizmos.matrix = prevMatrix;
        }
    }
}