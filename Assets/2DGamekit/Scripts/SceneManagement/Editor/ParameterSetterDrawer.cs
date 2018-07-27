#region

using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    [CustomPropertyDrawer(typeof(CharacterStateSetter.ParameterSetter))]
    public class ParameterSetterDrawer : PropertyDrawer
    {
        private SerializedProperty m_AnimatorProp;
        private SerializedProperty m_BoolValueProp;
        private SerializedProperty m_FloatValueProp;
        private SerializedProperty m_IntValueProp;
        private int m_ParameterNameIndex;
        private SerializedProperty m_ParameterNameProp;
        private string[] m_ParameterNames;
        private SerializedProperty m_ParameterTypeProp;
        private CharacterStateSetter.ParameterSetter.ParameterType[] m_ParameterTypes;
        private bool m_SetupCalled;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (m_AnimatorProp == null)
                return 0f;

            if (m_AnimatorProp.objectReferenceValue == null)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight * 3f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!m_SetupCalled || m_ParameterNames == null)
                ParameterSetterSetup(property);

            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, m_AnimatorProp);

            if (m_AnimatorProp.objectReferenceValue == null)
                return;

            position.y += position.height;
            m_ParameterNameIndex = EditorGUI.Popup(position, m_ParameterNameIndex, m_ParameterNames);
            m_ParameterNameProp.stringValue = m_ParameterNames[m_ParameterNameIndex];
            m_ParameterTypeProp.enumValueIndex = (int) m_ParameterTypes[m_ParameterNameIndex];

            position.y += position.height;
            switch ((CharacterStateSetter.ParameterSetter.ParameterType) m_ParameterTypeProp.enumValueIndex)
            {
                case CharacterStateSetter.ParameterSetter.ParameterType.Bool:
                    EditorGUI.PropertyField(position, m_BoolValueProp);
                    break;
                case CharacterStateSetter.ParameterSetter.ParameterType.Float:
                    EditorGUI.PropertyField(position, m_FloatValueProp);
                    break;
                case CharacterStateSetter.ParameterSetter.ParameterType.Int:
                    EditorGUI.PropertyField(position, m_IntValueProp);
                    break;
            }
        }

        private void ParameterSetterSetup(SerializedProperty property)
        {
            m_SetupCalled = true;

            m_AnimatorProp = property.FindPropertyRelative("animator");
            m_ParameterNameProp = property.FindPropertyRelative("parameterName");
            m_ParameterTypeProp = property.FindPropertyRelative("parameterType");
            m_BoolValueProp = property.FindPropertyRelative("boolValue");
            m_FloatValueProp = property.FindPropertyRelative("floatValue");
            m_IntValueProp = property.FindPropertyRelative("intValue");

            if (m_AnimatorProp.objectReferenceValue == null)
            {
                m_ParameterNames = null;
                return;
            }

            var animator = m_AnimatorProp.objectReferenceValue as Animator;

            if (animator.runtimeAnimatorController == null)
            {
                m_ParameterNames = null;
                return;
            }

            var animatorController = animator.runtimeAnimatorController as AnimatorController;

            var parameters = animatorController.parameters;

            m_ParameterNames = new string[parameters.Length];
            m_ParameterTypes = new CharacterStateSetter.ParameterSetter.ParameterType[parameters.Length];

            for (var i = 0; i < m_ParameterNames.Length; i++)
            {
                m_ParameterNames[i] = parameters[i].name;

                switch (parameters[i].type)
                {
                    case AnimatorControllerParameterType.Float:
                        m_ParameterTypes[i] = CharacterStateSetter.ParameterSetter.ParameterType.Float;
                        break;
                    case AnimatorControllerParameterType.Int:
                        m_ParameterTypes[i] = CharacterStateSetter.ParameterSetter.ParameterType.Int;
                        break;
                    case AnimatorControllerParameterType.Bool:
                        m_ParameterTypes[i] = CharacterStateSetter.ParameterSetter.ParameterType.Bool;
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        m_ParameterTypes[i] = CharacterStateSetter.ParameterSetter.ParameterType.Trigger;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            for (var i = 0; i < m_ParameterNames.Length; i++)
                if (m_ParameterNames[i] == m_ParameterNameProp.stringValue)
                {
                    m_ParameterNameIndex = i;
                    m_ParameterTypeProp.enumValueIndex = (int) m_ParameterTypes[i];
                }
        }
    }
}