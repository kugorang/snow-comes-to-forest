#region

using UnityEditor;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    [CustomEditor(typeof(MovingPlatform))]
    public class MovingPlatformEditor : Editor
    {
        private MovingPlatform m_MovingPlatform;

        private float m_PreviewPosition;

        private void OnEnable()
        {
            m_PreviewPosition = 0;
            m_MovingPlatform = target as MovingPlatform;

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                MovingPlatformPreview.CreateNewPreview(m_MovingPlatform);
        }

        private void OnDisable()
        {
            MovingPlatformPreview.DestroyPreview();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            m_MovingPlatform.platformCatcher = EditorGUILayout.ObjectField("Platform Catcher",
                m_MovingPlatform.platformCatcher, typeof(PlatformCatcher), true) as PlatformCatcher;
            if (EditorGUI.EndChangeCheck())
                Undo.RecordObject(target, "Changed Catcher");

            EditorGUI.BeginChangeCheck();
            m_PreviewPosition = EditorGUILayout.Slider("Preview position", m_PreviewPosition, 0.0f, 1.0f);
            if (EditorGUI.EndChangeCheck()) MovePreview();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();
            var isStartingMoving = EditorGUILayout.Toggle("Start moving", m_MovingPlatform.isMovingAtStart);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed move at start");
                m_MovingPlatform.isMovingAtStart = isStartingMoving;
            }

            if (isStartingMoving)
            {
                EditorGUI.indentLevel += 1;
                EditorGUI.BeginChangeCheck();
                var startOnlyWhenVisible =
                    EditorGUILayout.Toggle("When becoming visible", m_MovingPlatform.startMovingOnlyWhenVisible);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed move when visible");
                    m_MovingPlatform.startMovingOnlyWhenVisible = startOnlyWhenVisible;
                }

                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            var platformType =
                (MovingPlatform.MovingPlatformType) EditorGUILayout.EnumPopup("Looping", m_MovingPlatform.platformType);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Moving Platform type");
                m_MovingPlatform.platformType = platformType;
            }

            EditorGUI.BeginChangeCheck();
            var newSpeed = EditorGUILayout.FloatField("Speed", m_MovingPlatform.speed);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Speed");
                m_MovingPlatform.speed = newSpeed;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            if (GUILayout.Button("Add Node"))
            {
                Undo.RecordObject(target, "added node");


                var position = m_MovingPlatform.localNodes[m_MovingPlatform.localNodes.Length - 1] + Vector3.right;

                ArrayUtility.Add(ref m_MovingPlatform.localNodes, position);
                ArrayUtility.Add(ref m_MovingPlatform.waitTimes, 0);
            }

            EditorGUIUtility.labelWidth = 64;
            var delete = -1;
            for (var i = 0; i < m_MovingPlatform.localNodes.Length; ++i)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginHorizontal();

                var size = 64;
                EditorGUILayout.BeginVertical(GUILayout.Width(size));
                EditorGUILayout.LabelField("Node " + i, GUILayout.Width(size));
                if (i != 0 && GUILayout.Button("Delete", GUILayout.Width(size))) delete = i;
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                Vector3 newPosition;
                if (i == 0)
                    newPosition = m_MovingPlatform.localNodes[i];
                else
                    newPosition = EditorGUILayout.Vector3Field("Position", m_MovingPlatform.localNodes[i]);

                var newTime = EditorGUILayout.FloatField("Wait Time", m_MovingPlatform.waitTimes[i]);
                EditorGUILayout.EndVertical();


                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "changed time or position");
                    m_MovingPlatform.waitTimes[i] = newTime;
                    m_MovingPlatform.localNodes[i] = newPosition;
                }
            }

            EditorGUIUtility.labelWidth = 0;

            if (delete != -1)
            {
                Undo.RecordObject(target, "Removed point moving platform");

                ArrayUtility.RemoveAt(ref m_MovingPlatform.localNodes, delete);
                ArrayUtility.RemoveAt(ref m_MovingPlatform.waitTimes, delete);
            }
        }

        private void OnSceneGUI()
        {
            MovePreview();

            for (var i = 0; i < m_MovingPlatform.localNodes.Length; ++i)
            {
                Vector3 worldPos;
                if (Application.isPlaying)
                    worldPos = m_MovingPlatform.worldNode[i];
                else
                    worldPos = m_MovingPlatform.transform.TransformPoint(m_MovingPlatform.localNodes[i]);


                var newWorld = worldPos;
                if (i != 0)
                    newWorld = Handles.PositionHandle(worldPos, Quaternion.identity);

                Handles.color = Color.red;

                if (i == 0)
                {
                    if (m_MovingPlatform.platformType != MovingPlatform.MovingPlatformType.LOOP)
                        continue;

                    if (Application.isPlaying)
                        Handles.DrawDottedLine(worldPos,
                            m_MovingPlatform.worldNode[m_MovingPlatform.worldNode.Length - 1], 10);
                    else
                        Handles.DrawDottedLine(worldPos,
                            m_MovingPlatform.transform.TransformPoint(
                                m_MovingPlatform.localNodes[m_MovingPlatform.localNodes.Length - 1]), 10);
                }
                else
                {
                    if (Application.isPlaying)
                        Handles.DrawDottedLine(worldPos, m_MovingPlatform.worldNode[i - 1], 10);
                    else
                        Handles.DrawDottedLine(worldPos,
                            m_MovingPlatform.transform.TransformPoint(m_MovingPlatform.localNodes[i - 1]), 10);

                    if (worldPos != newWorld)
                    {
                        Undo.RecordObject(target, "moved point");
                        m_MovingPlatform.localNodes[i] = m_MovingPlatform.transform.InverseTransformPoint(newWorld);
                    }
                }
            }
        }

        private void MovePreview()
        {
            //compute pos from 0-1 preview pos

            if (Application.isPlaying)
                return;

            var step = 1.0f / (m_MovingPlatform.localNodes.Length - 1);

            var starting = Mathf.FloorToInt(m_PreviewPosition / step);

            if (starting > m_MovingPlatform.localNodes.Length - 2)
                return;

            var localRatio = (m_PreviewPosition - step * starting) / step;

            var localPos = Vector3.Lerp(m_MovingPlatform.localNodes[starting],
                m_MovingPlatform.localNodes[starting + 1], localRatio);

            MovingPlatformPreview.preview.transform.position = m_MovingPlatform.transform.TransformPoint(localPos);

            SceneView.RepaintAll();
        }
    }
}