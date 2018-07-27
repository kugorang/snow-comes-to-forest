#region

using System;
using UnityEditor;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNameDrawer : PropertyDrawer
    {
        private readonly string[] k_ScenePathSplitters = {"/", ".unity"};
        private int m_SceneIndex = -1;
        private GUIContent[] m_SceneNames;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (EditorBuildSettings.scenes.Length == 0) return;
            if (m_SceneIndex == -1)
                Setup(property);

            var oldIndex = m_SceneIndex;
            m_SceneIndex = EditorGUI.Popup(position, label, m_SceneIndex, m_SceneNames);

            if (oldIndex != m_SceneIndex)
                property.stringValue = m_SceneNames[m_SceneIndex].text;
        }

        private void Setup(SerializedProperty property)
        {
            var scenes = EditorBuildSettings.scenes;
            m_SceneNames = new GUIContent[scenes.Length];

            for (var i = 0; i < m_SceneNames.Length; i++)
            {
                var path = scenes[i].path;
                var splitPath = path.Split(k_ScenePathSplitters, StringSplitOptions.RemoveEmptyEntries);
                m_SceneNames[i] = new GUIContent(splitPath[splitPath.Length - 1]);
            }

            if (m_SceneNames.Length == 0)
                m_SceneNames = new[] {new GUIContent("[No Scenes In Build Settings]")};

            if (!string.IsNullOrEmpty(property.stringValue))
            {
                var sceneNameFound = false;
                for (var i = 0; i < m_SceneNames.Length; i++)
                    if (m_SceneNames[i].text == property.stringValue)
                    {
                        m_SceneIndex = i;
                        sceneNameFound = true;
                        break;
                    }

                if (!sceneNameFound)
                    m_SceneIndex = 0;
            }
            else
            {
                m_SceneIndex = 0;
            }

            property.stringValue = m_SceneNames[m_SceneIndex].text;
        }
    }
}