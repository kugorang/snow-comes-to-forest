#region

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    [CustomEditor(typeof(CharacterStateSetter))]
    public class CharacterStateSetterEditor : Editor
    {
        private GUIContent m_AnimatorContent;
        private SerializedProperty m_AnimatorProp;
        private GUIContent m_AnimatorStateNameContent;
        private SerializedProperty m_AnimatorStateNameProp;

        private string[] m_AnimatorStateNames;
        private GUIContent m_CharacterVelocityContent;
        private SerializedProperty m_CharacterVelocityProp;
        private GUIContent m_FaceLeftContent;
        private SerializedProperty m_FaceLeftProp;
        private int m_ParameterNameIndex;

        private string[] m_ParameterNames;

        private GUIContent m_ParameterSetterNameContent;
        private GUIContent m_ParameterSettersContent;
        private SerializedProperty m_ParameterSettersProp;
        private GUIContent m_ParameterSetterValueContent;
        private CharacterStateSetter.ParameterSetter.ParameterType[] m_ParameterTypes;
        private GUIContent m_PlayerCharacterContent;
        private SerializedProperty m_PlayerCharacterProp;

        private GUIContent m_SetCharacterFacingContent;

        private SerializedProperty m_SetCharacterFacingProp;

        private GUIContent m_SetCharacterVelocityContent;

        private SerializedProperty m_SetCharacterVelocityProp;

        private GUIContent m_SetParametersContent;

        private SerializedProperty m_SetParametersProp;

        private GUIContent m_SetStateContent;

        private SerializedProperty m_SetStateProp;
        private int m_StateNamesIndex;

        private void OnEnable()
        {
            m_AnimatorProp = serializedObject.FindProperty("animator");

            m_SetCharacterVelocityProp = serializedObject.FindProperty("setCharacterVelocity");
            m_PlayerCharacterProp = serializedObject.FindProperty("playerCharacter");
            m_CharacterVelocityProp = serializedObject.FindProperty("characterVelocity");

            m_SetCharacterFacingProp = serializedObject.FindProperty("setCharacterFacing");
            m_FaceLeftProp = serializedObject.FindProperty("faceLeft");

            m_SetStateProp = serializedObject.FindProperty("setState");
            m_AnimatorStateNameProp = serializedObject.FindProperty("animatorStateName");

            m_SetParametersProp = serializedObject.FindProperty("setParameters");
            m_ParameterSettersProp = serializedObject.FindProperty("parameterSetters");

            m_AnimatorContent = new GUIContent("Animator");

            m_SetCharacterVelocityContent = new GUIContent("Set Character Velocity");
            m_PlayerCharacterContent = new GUIContent("Player Character");
            m_CharacterVelocityContent = new GUIContent("Character Velocity");

            m_SetCharacterFacingContent = new GUIContent("Set Character Facing Content");
            m_FaceLeftContent = new GUIContent("Face Left");

            m_SetStateContent = new GUIContent("Set State");
            m_AnimatorStateNameContent = new GUIContent("Animator State Name");

            m_SetParametersContent = new GUIContent("Set Parameters");
            m_ParameterSettersContent = new GUIContent("Parameter Settings");

            m_ParameterSetterNameContent = new GUIContent("Name");
            m_ParameterSetterValueContent = new GUIContent("Value");

            SetAnimatorStateNames();
            if (m_AnimatorStateNames != null)
                for (var i = 0; i < m_AnimatorStateNames.Length; i++)
                    if (m_AnimatorStateNames[i] == m_AnimatorStateNameProp.stringValue)
                        m_StateNamesIndex = i;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_SetCharacterVelocityProp, m_SetCharacterVelocityContent);
            if (m_SetCharacterVelocityProp.boolValue)
            {
                EditorGUILayout.PropertyField(m_PlayerCharacterProp, m_PlayerCharacterContent);
                EditorGUILayout.PropertyField(m_CharacterVelocityProp, m_CharacterVelocityContent);
            }

            EditorGUILayout.PropertyField(m_SetCharacterFacingProp, m_SetCharacterFacingContent);
            if (m_SetCharacterFacingProp.boolValue)
            {
                EditorGUILayout.PropertyField(m_PlayerCharacterProp, m_PlayerCharacterContent);
                EditorGUILayout.PropertyField(m_FaceLeftProp, m_FaceLeftContent);
            }

            EditorGUILayout.PropertyField(m_SetStateProp, m_SetStateContent);
            if (m_SetStateProp.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_AnimatorProp, m_AnimatorContent);
                if (EditorGUI.EndChangeCheck()) SetAnimatorStateNames();

                if (m_AnimatorProp.objectReferenceValue == null ||
                    ((Animator) m_AnimatorProp.objectReferenceValue).runtimeAnimatorController == null)
                {
                    EditorGUILayout.HelpBox(
                        "An animator controller has not been found and so state names cannot be chosen.",
                        MessageType.Warning);
                    m_AnimatorStateNameProp.stringValue = "";
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    m_StateNamesIndex = EditorGUILayout.Popup(m_AnimatorStateNameContent.text, m_StateNamesIndex,
                        m_AnimatorStateNames);
                    if (EditorGUI.EndChangeCheck())
                        m_AnimatorStateNameProp.stringValue = m_AnimatorStateNames[m_StateNamesIndex];
                }
            }

            EditorGUILayout.PropertyField(m_SetParametersProp, m_SetParametersContent);
            if (m_SetParametersProp.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_AnimatorProp, m_AnimatorContent);
                if (EditorGUI.EndChangeCheck()) SetAnimatorStateNames();

                if (m_AnimatorProp.objectReferenceValue == null ||
                    ((Animator) m_AnimatorProp.objectReferenceValue).runtimeAnimatorController == null)
                {
                    EditorGUILayout.HelpBox(
                        "An animator controller has not been found and so state names cannot be chosen.",
                        MessageType.Warning);
                    m_ParameterSettersProp.arraySize = 0;
                }
                else
                {
                    m_ParameterSettersProp.arraySize =
                        EditorGUILayout.IntField(m_ParameterSettersContent, m_ParameterSettersProp.arraySize);
                    EditorGUI.indentLevel++;
                    for (var i = 0; i < m_ParameterSettersProp.arraySize; i++)
                    {
                        var elementProp = m_ParameterSettersProp.GetArrayElementAtIndex(i);
                        ParameterSetterGUI(elementProp);
                    }

                    EditorGUI.indentLevel--;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SetAnimatorStateNames()
        {
            if (m_AnimatorProp.objectReferenceValue == null)
            {
                m_AnimatorStateNames = null;
                m_ParameterNames = null;
                m_ParameterTypes = null;
                return;
            }

            var animator = m_AnimatorProp.objectReferenceValue as Animator;

            if (animator.runtimeAnimatorController == null)
            {
                m_AnimatorStateNames = null;
                m_ParameterNames = null;
                m_ParameterTypes = null;
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

            var stateNamesList = new List<string>();

            for (var i = 0; i < animatorController.layers.Length; i++)
            {
                for (var j = 0; j < animatorController.layers[i].stateMachine.states.Length; j++)
                    stateNamesList.Add(animatorController.layers[i].stateMachine.states[j].state.name);

                GetStateMachinesFromStateMachineAndAddNames(stateNamesList, animatorController.layers[i].stateMachine);
            }

            m_AnimatorStateNames = stateNamesList.ToArray();
        }

        private static void GetStateMachinesFromStateMachineAndAddNames(List<string> stateNamesList,
            AnimatorStateMachine stateMachine)
        {
            var stateMachines = new AnimatorStateMachine[stateMachine.stateMachines.Length];

            for (var i = 0; i < stateMachines.Length; i++)
            {
                stateMachines[i] = stateMachine.stateMachines[i].stateMachine;

                for (var j = 0; j < stateMachines[i].states.Length; j++)
                    stateNamesList.Add(stateMachines[i].states[j].state.name);

                GetStateMachinesFromStateMachineAndAddNames(stateNamesList, stateMachines[i]);
            }
        }

        private void ParameterSetterGUI(SerializedProperty parameterSetterProp)
        {
            var parameterNameProp = parameterSetterProp.FindPropertyRelative("parameterName");
            var parameterTypeProp = parameterSetterProp.FindPropertyRelative("parameterType");
            var boolValueProp = parameterSetterProp.FindPropertyRelative("boolValue");
            var floatValueProp = parameterSetterProp.FindPropertyRelative("floatValue");
            var intValueProp = parameterSetterProp.FindPropertyRelative("intValue");

            for (var i = 0; i < m_ParameterNames.Length; i++)
                if (m_ParameterNames[i] == parameterNameProp.stringValue)
                {
                    m_ParameterNameIndex = i;
                    parameterTypeProp.enumValueIndex = (int) m_ParameterTypes[i];
                }

            var position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
            var nameLabelRect = new Rect(position.x, position.y, position.width * 0.2f,
                EditorGUIUtility.singleLineHeight);
            var nameControlRect = new Rect(nameLabelRect.x + nameLabelRect.width, position.y, position.width * 0.3f,
                position.height);
            var valueLabelRect = new Rect(nameControlRect.x + nameControlRect.width, position.y, position.width * 0.2f,
                position.height);
            var valueControlRect = new Rect(valueLabelRect.x + valueLabelRect.width, position.y, position.width * 0.3f,
                position.height);

            EditorGUI.LabelField(nameLabelRect, m_ParameterSetterNameContent);
            m_ParameterNameIndex = EditorGUI.Popup(nameControlRect, GUIContent.none.text, m_ParameterNameIndex,
                m_ParameterNames);
            parameterNameProp.stringValue = m_ParameterNames[m_ParameterNameIndex];
            parameterTypeProp.enumValueIndex = (int) m_ParameterTypes[m_ParameterNameIndex];

            switch ((CharacterStateSetter.ParameterSetter.ParameterType) parameterTypeProp.enumValueIndex)
            {
                case CharacterStateSetter.ParameterSetter.ParameterType.Bool:
                    EditorGUI.LabelField(valueLabelRect, m_ParameterSetterValueContent);
                    EditorGUI.PropertyField(valueControlRect, boolValueProp, GUIContent.none);
                    break;
                case CharacterStateSetter.ParameterSetter.ParameterType.Float:
                    EditorGUI.LabelField(valueLabelRect, m_ParameterSetterValueContent);
                    EditorGUI.PropertyField(valueControlRect, floatValueProp, GUIContent.none);
                    break;
                case CharacterStateSetter.ParameterSetter.ParameterType.Int:
                    EditorGUI.LabelField(valueLabelRect, m_ParameterSetterValueContent);
                    EditorGUI.PropertyField(valueControlRect, intValueProp, GUIContent.none);
                    break;
            }
        }
    }
}