#region

using UnityEditor;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    [CustomEditor(typeof(TransitionPoint))]
    public class TransitionStartEditor : Editor
    {
        private SerializedProperty m_DestinationTransformProp;
        private SerializedProperty m_InventoryCheckProp;

        private GUIContent[] m_InventoryControllerItems = new GUIContent[0];
        private SerializedProperty m_InventoryControllerProp;
        private SerializedProperty m_InventoryItemsProp;
        private SerializedProperty m_NewSceneNameProp;
        private SerializedProperty m_OnDoesNotHaveItemProp;
        private SerializedProperty m_OnHasItemProp;
        private SerializedProperty m_RequiresInventoryCheckProp;
        private SerializedProperty m_ResetInputValuesOnTransitionProp;
        private SerializedProperty m_TransitionDestinationTagProp;
        private SerializedProperty m_TransitioningGameObjectProp;
        private SerializedProperty m_TransitionTypeProp;
        private SerializedProperty m_TransitionWhenProp;

        private void OnEnable()
        {
            m_TransitioningGameObjectProp = serializedObject.FindProperty("transitioningGameObject");
            m_TransitionTypeProp = serializedObject.FindProperty("transitionType");
            m_NewSceneNameProp = serializedObject.FindProperty("newSceneName");
            m_TransitionDestinationTagProp = serializedObject.FindProperty("transitionDestinationTag");
            m_DestinationTransformProp = serializedObject.FindProperty("destinationTransform");
            m_TransitionWhenProp = serializedObject.FindProperty("transitionWhen");
            m_ResetInputValuesOnTransitionProp = serializedObject.FindProperty("resetInputValuesOnTransition");
            m_RequiresInventoryCheckProp = serializedObject.FindProperty("requiresInventoryCheck");
            m_InventoryControllerProp = serializedObject.FindProperty("inventoryController");
            m_InventoryCheckProp = serializedObject.FindProperty("inventoryCheck");
            m_InventoryItemsProp = m_InventoryCheckProp.FindPropertyRelative("inventoryItems");
            m_OnHasItemProp = m_InventoryCheckProp.FindPropertyRelative("OnHasItem");
            m_OnDoesNotHaveItemProp = m_InventoryCheckProp.FindPropertyRelative("OnDoesNotHaveItem");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_TransitioningGameObjectProp);

            EditorGUILayout.PropertyField(m_TransitionTypeProp);
            EditorGUI.indentLevel++;
            if ((TransitionPoint.TransitionType) m_TransitionTypeProp.enumValueIndex ==
                TransitionPoint.TransitionType.SameScene)
            {
                EditorGUILayout.PropertyField(m_DestinationTransformProp);
            }
            else
            {
                EditorGUILayout.PropertyField(m_NewSceneNameProp);
                EditorGUILayout.PropertyField(m_TransitionDestinationTagProp);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(m_TransitionWhenProp);
            EditorGUILayout.PropertyField(m_ResetInputValuesOnTransitionProp);

            EditorGUILayout.PropertyField(m_RequiresInventoryCheckProp);
            if (m_RequiresInventoryCheckProp.boolValue)
            {
                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_InventoryControllerProp);
                if (EditorGUI.EndChangeCheck() || m_InventoryControllerProp.objectReferenceValue != null &&
                    m_InventoryControllerItems.Length == 0) SetupInventoryItemGUI();

                if (m_InventoryControllerProp.objectReferenceValue != null)
                {
                    var controller = m_InventoryControllerProp.objectReferenceValue as InventoryController;
                    m_InventoryItemsProp.arraySize =
                        EditorGUILayout.IntField("Inventory Items", m_InventoryItemsProp.arraySize);
                    EditorGUI.indentLevel++;
                    for (var i = 0; i < m_InventoryItemsProp.arraySize; i++)
                    {
                        var elementProp = m_InventoryItemsProp.GetArrayElementAtIndex(i);

                        var itemIndex = ItemIndexFromController(controller, elementProp.stringValue);
                        if (itemIndex == -1)
                        {
                            EditorGUILayout.LabelField("No items found in controller");
                        }
                        else if (itemIndex == -2)
                        {
                            elementProp.stringValue = m_InventoryControllerItems[0].text;
                        }
                        else if (itemIndex == -3)
                        {
                            Debug.LogWarning("Previously listed item to check not found, resetting to item index 0");
                            elementProp.stringValue = m_InventoryControllerItems[0].text;
                        }
                        else
                        {
                            itemIndex = EditorGUILayout.Popup(new GUIContent("Item " + i), itemIndex,
                                m_InventoryControllerItems);
                            elementProp.stringValue = m_InventoryControllerItems[itemIndex].text;
                        }
                    }

                    EditorGUI.indentLevel--;

                    EditorGUILayout.PropertyField(m_OnHasItemProp);
                    EditorGUILayout.PropertyField(m_OnDoesNotHaveItemProp);
                }
                else
                {
                    for (var i = 0; i < m_InventoryItemsProp.arraySize; i++)
                    {
                        var elementProp = m_InventoryItemsProp.GetArrayElementAtIndex(i);
                        elementProp.stringValue = "";
                    }
                }

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SetupInventoryItemGUI()
        {
            if (m_InventoryControllerProp.objectReferenceValue == null)
                return;

            var inventoryController = m_InventoryControllerProp.objectReferenceValue as InventoryController;

            m_InventoryControllerItems = new GUIContent[inventoryController.inventoryEvents.Length];
            for (var i = 0; i < inventoryController.inventoryEvents.Length; i++)
                m_InventoryControllerItems[i] = new GUIContent(inventoryController.inventoryEvents[i].key);
        }

        private int ItemIndexFromController(InventoryController controller, string itemName)
        {
            if (controller.inventoryEvents.Length == 0)
                return -1;

            if (string.IsNullOrEmpty(itemName))
                return -2;

            for (var i = 0; i < controller.inventoryEvents.Length; i++)
                if (controller.inventoryEvents[i].key == itemName)
                    return i;
            return -3;
        }
    }
}