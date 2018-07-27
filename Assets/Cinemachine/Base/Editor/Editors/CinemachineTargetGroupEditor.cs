#region

using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

#endregion

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineTargetGroup))]
    internal sealed class CinemachineTargetGroupEditor : BaseEditor<CinemachineTargetGroup>
    {
        private ReorderableList mTargetList;

        protected override List<string> GetExcludedPropertiesInInspector()
        {
            var excluded = base.GetExcludedPropertiesInInspector();
            excluded.Add(FieldPath(x => x.m_Targets));
            return excluded;
        }

        private void OnEnable()
        {
            mTargetList = null;
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            DrawRemainingPropertiesInInspector();

            if (mTargetList == null)
                SetupTargetList();
            EditorGUI.BeginChangeCheck();
            mTargetList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        private void SetupTargetList()
        {
            float vSpace = 2;
            var floatFieldWidth = EditorGUIUtility.singleLineHeight * 3f;
            var hBigSpace = EditorGUIUtility.singleLineHeight * 2 / 3;

            mTargetList = new ReorderableList(
                serializedObject, FindProperty(x => x.m_Targets),
                true, true, true, true);

            // Needed for accessing field names as strings
            var def = new CinemachineTargetGroup.Target();

            mTargetList.drawHeaderCallback = rect =>
            {
                rect.width -= EditorGUIUtility.singleLineHeight + 2 * (floatFieldWidth + hBigSpace);
                var pos = rect.position;
                pos.x += EditorGUIUtility.singleLineHeight;
                rect.position = pos;
                EditorGUI.LabelField(rect, "Target");

                pos.x += rect.width + hBigSpace;
                rect.width = floatFieldWidth;
                rect.position = pos;
                EditorGUI.LabelField(rect, "Weight");

                pos.x += rect.width + hBigSpace;
                rect.position = pos;
                EditorGUI.LabelField(rect, "Radius");
            };

            mTargetList.drawElementCallback
                = (rect, index, isActive, isFocused) =>
                {
                    var elemProp = mTargetList.serializedProperty.GetArrayElementAtIndex(index);

                    rect.y += vSpace;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    var pos = rect.position;
                    //rect.width -= hSpace + 2 * EditorGUIUtility.singleLineHeight;
                    rect.width -= 2 * (floatFieldWidth + hBigSpace);
                    EditorGUI.PropertyField(rect, elemProp.FindPropertyRelative(() => def.target), GUIContent.none);

                    var oldWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = EditorGUIUtility.singleLineHeight;
                    pos.x += rect.width;
                    rect.width = floatFieldWidth + hBigSpace;
                    rect.position = pos;
                    EditorGUI.PropertyField(rect, elemProp.FindPropertyRelative(() => def.weight), new GUIContent(" "));
                    pos.x += rect.width;
                    rect.position = pos;
                    EditorGUI.PropertyField(rect, elemProp.FindPropertyRelative(() => def.radius), new GUIContent(" "));
                    EditorGUIUtility.labelWidth = oldWidth;
                };

            mTargetList.onAddCallback = l =>
            {
                var index = l.serializedProperty.arraySize;
                ++l.serializedProperty.arraySize;
                var elemProp = mTargetList.serializedProperty.GetArrayElementAtIndex(index);
                elemProp.FindPropertyRelative(() => def.weight).floatValue = 1;
            };
        }
    }
}