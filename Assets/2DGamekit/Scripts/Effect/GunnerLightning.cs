#region

using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class GunnerLightning : MonoBehaviour
    {
        public Transform end;

        private Transform[] m_Branch;
        private LineRenderer m_LineRenderer;
        private Vector3[] m_Points;
        private float m_UpdateTime;

        public int pointCount = 10;
        public float randomOffset = 0.5f;

        public float updateInterval = 0.5f;

        // Use this for initialization
        private void Start()
        {
            m_LineRenderer = GetComponent<LineRenderer>();
            m_Points = new Vector3[pointCount];
            m_LineRenderer.positionCount = pointCount;
            m_LineRenderer.useWorldSpace = false;
        }

        private void Update()
        {
            if (Time.time >= m_UpdateTime)
            {
                m_LineRenderer.positionCount = pointCount;

                m_Points[0] = Vector3.zero;
                var Segment = (end.position - transform.position) / (pointCount - 1);

                for (var i = 1; i < pointCount - 1; i++)
                {
                    m_Points[i] = Segment * i;
                    m_Points[i].y += Random.Range(-randomOffset, randomOffset);
                    m_Points[i].x += Random.Range(-randomOffset, randomOffset);
                }

                m_Points[pointCount - 1] = end.position - transform.position;
                m_LineRenderer.SetPositions(m_Points);

                m_UpdateTime += updateInterval;
            }
        }
    }
}