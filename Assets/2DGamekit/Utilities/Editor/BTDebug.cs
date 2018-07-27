#region

using BTAI;
using UnityEditor;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    public class BTDebug : EditorWindow
    {
        protected Root _currentRoot;


        [MenuItem("Kit Tools/Behaviour Tree Debug")]
        private static void OpenWindow()
        {
            var btdebug = GetWindow<BTDebug>();
            btdebug.Show();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Only work during play mode.", MessageType.Info);
            }
            else
            {
                if (_currentRoot == null)
                    FindRoot();
                else
                    RecursiveTreeParsing(_currentRoot, 0, true);
            }
        }

        private void Update()
        {
            Repaint();
        }

        private void RecursiveTreeParsing(Branch branch, int indent, bool parentIsActive)
        {
            var nodes = branch.Children();

            for (var i = 0; i < nodes.Count; ++i)
            {
                EditorGUI.indentLevel = indent;

                var isActiveChild = branch.ActiveChild() == i;
                GUI.color = isActiveChild && parentIsActive ? Color.green : Color.white;
                EditorGUILayout.LabelField(nodes[i].ToString());

                if (nodes[i] is Branch)
                    RecursiveTreeParsing(nodes[i] as Branch, indent + 1, isActiveChild);
            }
        }

        private void FindRoot()
        {
            if (Selection.activeGameObject == null)
            {
                _currentRoot = null;
                return;
            }

            var debugable = Selection.activeGameObject.GetComponentInChildren<IBTDebugable>();

            if (debugable != null) _currentRoot = debugable.GetAIRoot();
        }
    }
}