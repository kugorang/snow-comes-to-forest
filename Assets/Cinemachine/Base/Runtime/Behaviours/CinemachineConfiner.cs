#region

using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;

#endregion

namespace Cinemachine
{
    /// <summary>
    ///     An add-on module for Cinemachine Virtual Camera that post-processes
    ///     the final position of the virtual camera. It will confine the virtual
    ///     camera's position to the volume specified in the Bounding Volume field.
    /// </summary>
    [DocumentationSorting(22, DocumentationSortingAttribute.Level.UserRef)]
    [ExecuteInEditMode]
    [AddComponentMenu("")] // Hide in menu
    [SaveDuringPlay]
    public class CinemachineConfiner : CinemachineExtension
    {
        /// <summary>The confiner can operate using a 2D bounding shape or a 3D bounding volume</summary>
        public enum Mode
        {
            Confine2D,
            Confine3D
        }

        /// <summary>The 2D shape within which the camera is to be contained.</summary>
        [Tooltip("The 2D shape within which the camera is to be contained")]
        public Collider2D m_BoundingShape2D;

        /// <summary>The volume within which the camera is to be contained.</summary>
        [Tooltip("The volume within which the camera is to be contained")]
        public Collider m_BoundingVolume;

        /// <summary>The confiner can operate using a 2D bounding shape or a 3D bounding volume</summary>
        [Tooltip("The confiner can operate using a 2D bounding shape or a 3D bounding volume")]
        public Mode m_ConfineMode;

        /// <summary>If camera is orthographic, screen edges will be confined to the volume.</summary>
        [Tooltip(
            "If camera is orthographic, screen edges will be confined to the volume.  If not checked, then only the camera center will be confined")]
        public bool m_ConfineScreenEdges = true;

        /// <summary>How gradually to return the camera to the bounding volume if it goes beyond the borders</summary>
        [Tooltip(
            "How gradually to return the camera to the bounding volume if it goes beyond the borders.  Higher numbers are more gradual.")]
        [Range(0, 10)]
        public float m_Damping;

        private List<List<Vector2>> m_pathCache;

        /// <summary>Check if the bounding volume is defined</summary>
        public bool IsValid
        {
            get
            {
                return m_ConfineMode == Mode.Confine3D && m_BoundingVolume != null
                       || m_ConfineMode == Mode.Confine2D && m_BoundingShape2D != null;
            }
        }

        /// <summary>See whether the virtual camera has been moved by the confiner</summary>
        /// <param name="vcam">
        ///     The virtual camera in question.  This might be different from the
        ///     virtual camera that owns the confiner, in the event that the camera has children
        /// </param>
        /// <returns>True if the virtual camera has been repositioned</returns>
        public bool CameraWasDisplaced(CinemachineVirtualCameraBase vcam)
        {
            return GetExtraState<VcamExtraState>(vcam).confinerDisplacement > 0;
        }

        private void OnValidate()
        {
            m_Damping = Mathf.Max(0, m_Damping);
        }

        /// <summary>Callback to to the camera confining</summary>
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (IsValid)
                if (stage == CinemachineCore.Stage.Body)
                {
                    Vector3 displacement;
                    if (m_ConfineScreenEdges && state.Lens.Orthographic)
                        displacement = ConfineScreenEdges(vcam, ref state);
                    else
                        displacement = ConfinePoint(state.CorrectedPosition);

                    var extra = GetExtraState<VcamExtraState>(vcam);
                    if (m_Damping > 0 && deltaTime >= 0)
                    {
                        var delta = displacement - extra.m_previousDisplacement;
                        delta = Damper.Damp(delta, m_Damping, deltaTime);
                        displacement = extra.m_previousDisplacement + delta;
                    }

                    extra.m_previousDisplacement = displacement;
                    state.PositionCorrection += displacement;
                    extra.confinerDisplacement = displacement.magnitude;
                }
        }

        /// <summary>Call this if the bounding shape's points change at runtime</summary>
        public void InvalidatePathCache()
        {
            m_pathCache = null;
        }

        private bool ValidatePathCache()
        {
            var colliderType = m_BoundingShape2D == null ? null : m_BoundingShape2D.GetType();
            if (colliderType == typeof(PolygonCollider2D))
            {
                var poly = m_BoundingShape2D as PolygonCollider2D;
                if (m_pathCache == null || m_pathCache.Count != poly.pathCount)
                {
                    m_pathCache = new List<List<Vector2>>();
                    for (var i = 0; i < poly.pathCount; ++i)
                    {
                        var path = poly.GetPath(i);
                        var dst = new List<Vector2>();
                        for (var j = 0; j < path.Length; ++j)
                            dst.Add(path[j]);
                        m_pathCache.Add(dst);
                    }
                }

                return true;
            }

            if (colliderType == typeof(CompositeCollider2D))
            {
                var poly = m_BoundingShape2D as CompositeCollider2D;
                if (m_pathCache == null || m_pathCache.Count != poly.pathCount)
                {
                    m_pathCache = new List<List<Vector2>>();
                    var path = new Vector2[poly.pointCount];
                    for (var i = 0; i < poly.pathCount; ++i)
                    {
                        var numPoints = poly.GetPath(i, path);
                        var dst = new List<Vector2>();
                        for (var j = 0; j < numPoints; ++j)
                            dst.Add(path[j]);
                        m_pathCache.Add(dst);
                    }
                }

                return true;
            }

            InvalidatePathCache();
            return false;
        }

        private Vector3 ConfinePoint(Vector3 camPos)
        {
            // 3D version
            if (m_ConfineMode == Mode.Confine3D)
                return m_BoundingVolume.ClosestPoint(camPos) - camPos;

            // 2D version
            if (m_BoundingShape2D.OverlapPoint(camPos))
                return Vector3.zero;

            // Find the nearest point on the shape's boundary
            if (!ValidatePathCache())
                return Vector3.zero;

            Vector2 p = camPos;
            var closest = p;
            var bestDistance = float.MaxValue;
            for (var i = 0; i < m_pathCache.Count; ++i)
            {
                var numPoints = m_pathCache[i].Count;
                if (numPoints > 0)
                {
                    Vector2 v0 = m_BoundingShape2D.transform.TransformPoint(m_pathCache[i][numPoints - 1]);
                    for (var j = 0; j < numPoints; ++j)
                    {
                        Vector2 v = m_BoundingShape2D.transform.TransformPoint(m_pathCache[i][j]);
                        var c = Vector2.Lerp(v0, v, p.ClosestPointOnSegment(v0, v));
                        var d = Vector2.SqrMagnitude(p - c);
                        if (d < bestDistance)
                        {
                            bestDistance = d;
                            closest = c;
                        }

                        v0 = v;
                    }
                }
            }

            return closest - p;
        }

        // Camera must be orthographic
        private Vector3 ConfineScreenEdges(CinemachineVirtualCameraBase vcam, ref CameraState state)
        {
            var rot = Quaternion.Inverse(state.CorrectedOrientation);
            var dy = state.Lens.OrthographicSize;
            var dx = dy * state.Lens.Aspect;
            var vx = rot * Vector3.right * dx;
            var vy = rot * Vector3.up * dy;

            var displacement = Vector3.zero;
            var camPos = state.CorrectedPosition;
            const int kMaxIter = 12;
            for (var i = 0; i < kMaxIter; ++i)
            {
                var d = ConfinePoint(camPos - vy - vx);
                if (d.AlmostZero())
                    d = ConfinePoint(camPos - vy + vx);
                if (d.AlmostZero())
                    d = ConfinePoint(camPos + vy - vx);
                if (d.AlmostZero())
                    d = ConfinePoint(camPos + vy + vx);
                if (d.AlmostZero())
                    break;
                displacement += d;
                camPos += d;
            }

            return displacement;
        }

        private class VcamExtraState
        {
            public float confinerDisplacement;
            public Vector3 m_previousDisplacement;
        }
    }
}