#region

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#endregion

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineCollider))]
    public sealed class CinemachineColliderEditor : BaseEditor<CinemachineCollider>
    {
        protected override List<string> GetExcludedPropertiesInInspector()
        {
            var excluded = base.GetExcludedPropertiesInInspector();
            if (!Target.m_AvoidObstacles)
            {
                excluded.Add(FieldPath(x => x.m_DistanceLimit));
                excluded.Add(FieldPath(x => x.m_CameraRadius));
                excluded.Add(FieldPath(x => x.m_Strategy));
                excluded.Add(FieldPath(x => x.m_MaximumEffort));
                excluded.Add(FieldPath(x => x.m_Damping));
            }
            else if (Target.m_Strategy == CinemachineCollider.ResolutionStrategy.PullCameraForward)
            {
                excluded.Add(FieldPath(x => x.m_MaximumEffort));
            }

            return excluded;
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();

            if (Target.m_AvoidObstacles && !Target.VirtualCamera.State.HasLookAt)
                EditorGUILayout.HelpBox(
                    "Preserve Line Of Sight requires a LookAt target.",
                    MessageType.Warning);

            DrawRemainingPropertiesInInspector();
        }

        [DrawGizmo(GizmoType.Active | GizmoType.Selected, typeof(CinemachineCollider))]
        private static void DrawColliderGizmos(CinemachineCollider collider, GizmoType type)
        {
            var vcam = collider != null ? collider.VirtualCamera : null;
            if (vcam != null && collider.enabled)
            {
                var oldColor = Gizmos.color;
                var pos = vcam.State.FinalPosition;
                if (collider.m_AvoidObstacles && vcam.State.HasLookAt)
                {
                    Gizmos.color = CinemachineColliderPrefs.FeelerColor;
                    if (collider.m_CameraRadius > 0)
                        Gizmos.DrawWireSphere(pos, collider.m_CameraRadius);

                    var forwardFeelerVector = (vcam.State.ReferenceLookAt - pos).normalized;
                    var distance = collider.m_DistanceLimit;
                    Gizmos.DrawLine(pos, pos + forwardFeelerVector * distance);

                    // Show the avoidance path, for debugging
                    var debugPaths = collider.DebugPaths;
                    foreach (var path in debugPaths)
                    {
                        Gizmos.color = CinemachineColliderPrefs.FeelerHitColor;
                        var p0 = vcam.State.ReferenceLookAt;
                        foreach (var p in path)
                        {
                            Gizmos.DrawLine(p0, p);
                            p0 = p;
                        }

                        Gizmos.DrawLine(p0, pos);
                    }
                }

                Gizmos.color = oldColor;
            }
        }
    }
}