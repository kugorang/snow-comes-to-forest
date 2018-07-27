#region

using UnityEditor;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    [CustomEditor(typeof(PlayerInput))]
    public class PlayerInputEditor : DataPersisterEditor
    {
        private bool m_IsNotInstance;
        private bool m_IsPrefab;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_IsPrefab = AssetDatabase.Contains(target);
            m_IsNotInstance = PrefabUtility.GetPrefabParent(target) == null;
        }

        public override void OnInspectorGUI()
        {
            if (m_IsPrefab || m_IsNotInstance)
            {
                base.OnInspectorGUI();
            }
            else
            {
                EditorGUILayout.HelpBox("Modify the prefab and not this instance", MessageType.Warning);
                if (GUILayout.Button("Select Prefab")) Selection.activeObject = PrefabUtility.GetPrefabParent(target);
            }
        }
    }
}