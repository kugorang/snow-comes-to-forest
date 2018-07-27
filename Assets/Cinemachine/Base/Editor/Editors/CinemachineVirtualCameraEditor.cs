#region

using System;
using System.Collections.Generic;
using System.Reflection;
using Cinemachine.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

#endregion

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineVirtualCamera))]
    internal class CinemachineVirtualCameraEditor
        : CinemachineVirtualCameraBaseEditor<CinemachineVirtualCamera>
    {
        private static StageData[] sStageData;
        private UnityEditor.Editor[] m_componentEditors;
        private CinemachineComponentBase[] m_components;
        private bool[] m_stageError;

        // Instance data - call UpdateInstanceData() to refresh this
        private int[] m_stageState;

        private Vector3 mPreviousPosition;

        protected override void OnEnable()
        {
            // Build static menu arrays via reflection
            base.OnEnable();
            UpdateStaticData();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            // Must destroy editors or we get exceptions
            if (m_componentEditors != null)
                foreach (var e in m_componentEditors)
                    if (e != null)
                        DestroyImmediate(e);
        }

        private void OnSceneGUI()
        {
            if (!Target.UserIsDragging)
                mPreviousPosition = Target.transform.position;
            if (Selection.Contains(Target.gameObject) && Tools.current == Tool.Move
                                                      && Event.current.type == EventType.MouseDrag)
            {
                // User might be dragging our position handle
                Target.UserIsDragging = true;
                var delta = Target.transform.position - mPreviousPosition;
                if (!delta.AlmostZero())
                {
                    Undo.RegisterFullObjectHierarchyUndo(Target.gameObject, "Camera drag");
                    Target.OnPositionDragged(delta);
                    mPreviousPosition = Target.transform.position;
                }
            }
            else if (GUIUtility.hotControl == 0 && Target.UserIsDragging)
            {
                // We're not dragging anything now, but we were
                InternalEditorUtility.RepaintAllViews();
                Target.UserIsDragging = false;
            }
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            DrawHeaderInInspector();
            DrawPropertyInInspector(FindProperty(x => x.m_Priority));
            DrawTargetsInInspector(FindProperty(x => x.m_Follow), FindProperty(x => x.m_LookAt));
            DrawRemainingPropertiesInInspector();
            DrawPipelineInInspector();
            DrawExtensionsWidgetInInspector();
        }

        protected void DrawPipelineInInspector()
        {
            UpdateInstanceData();
            foreach (CinemachineCore.Stage stage in Enum.GetValues(typeof(CinemachineCore.Stage)))
            {
                var index = (int) stage;

                // Skip pipeline stages that have no implementations
                if (sStageData[index].PopupOptions.Length <= 1)
                    continue;

                const float indentOffset = 6;

                var stageBoxStyle = GUI.skin.box;
                EditorGUILayout.BeginVertical(stageBoxStyle);
                var rect = EditorGUILayout.GetControlRect(true);

                // Don't use PrefixLabel() because it will link the enabled status of field and label
                var label = new GUIContent(NicifyName(stage.ToString()));
                if (m_stageError[index])
                    label.image = EditorGUIUtility.IconContent("console.warnicon.sml").image;
                var labelWidth = EditorGUIUtility.labelWidth - (indentOffset + EditorGUI.indentLevel * 15);
                var r = rect;
                r.width = labelWidth;
                EditorGUI.LabelField(r, label);
                r = rect;
                r.width -= labelWidth;
                r.x += labelWidth;
                GUI.enabled = !StageIsLocked(stage);
                var newSelection = EditorGUI.Popup(r, m_stageState[index], sStageData[index].PopupOptions);
                GUI.enabled = true;

                var type = sStageData[index].types[newSelection];
                if (newSelection != m_stageState[index])
                {
                    SetPipelineStage(stage, type);
                    if (newSelection != 0)
                        sStageData[index].IsExpanded = true;
                    UpdateInstanceData(); // because we changed it
                    return;
                }

                if (type != null)
                {
                    var stageRect = new Rect(
                        rect.x - indentOffset, rect.y, rect.width + indentOffset, rect.height);
                    sStageData[index].IsExpanded = EditorGUI.Foldout(
                        stageRect, sStageData[index].IsExpanded, GUIContent.none);
                    if (sStageData[index].IsExpanded)
                    {
                        // Make the editor for that stage
                        var e = GetEditorForPipelineStage(stage);
                        if (e != null)
                        {
                            ++EditorGUI.indentLevel;
                            EditorGUILayout.Separator();
                            e.OnInspectorGUI();
                            EditorGUILayout.Separator();
                            --EditorGUI.indentLevel;
                        }
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }

        private bool StageIsLocked(CinemachineCore.Stage stage)
        {
            var locked = Target.m_LockStageInInspector;
            if (locked != null)
                for (var i = 0; i < locked.Length; ++i)
                    if (locked[i] == stage)
                        return true;
            return false;
        }

        private UnityEditor.Editor GetEditorForPipelineStage(CinemachineCore.Stage stage)
        {
            foreach (var e in m_componentEditors)
                if (e != null)
                {
                    var c = e.target as CinemachineComponentBase;
                    if (c != null && c.Stage == stage)
                        return e;
                }

            return null;
        }

        private void SetPipelineStage(CinemachineCore.Stage stage, Type type)
        {
            Undo.SetCurrentGroupName("Cinemachine pipeline change");

            // Get the existing components
            var owner = Target.GetComponentOwner();

            var components = owner.GetComponents<CinemachineComponentBase>();
            if (components == null)
                components = new CinemachineComponentBase[0];

            // Find an appropriate insertion point
            var numComponents = components.Length;
            var insertPoint = 0;
            for (insertPoint = 0; insertPoint < numComponents; ++insertPoint)
                if (components[insertPoint].Stage >= stage)
                    break;

            // Remove the existing components at that stage
            for (var i = numComponents - 1; i >= 0; --i)
                if (components[i].Stage == stage)
                {
                    Undo.DestroyObjectImmediate(components[i]);
                    components[i] = null;
                    --numComponents;
                    if (i < insertPoint)
                        --insertPoint;
                }

            // Add the new stage
            if (type != null)
            {
                var b = Undo.AddComponent(owner.gameObject, type) as MonoBehaviour;
                while (numComponents-- > insertPoint)
                    ComponentUtility.MoveComponentDown(b);
            }
        }

        // This code dynamically discovers eligible classes and builds the menu
        // data for the various component pipeline stages.
        private void UpdateStaticData()
        {
            if (sStageData != null)
                return;
            sStageData = new StageData[Enum.GetValues(typeof(CinemachineCore.Stage)).Length];

            var stageTypes = new List<Type>[Enum.GetValues(typeof(CinemachineCore.Stage)).Length];
            for (var i = 0; i < stageTypes.Length; ++i)
            {
                sStageData[i].Name = ((CinemachineCore.Stage) i).ToString();
                stageTypes[i] = new List<Type>();
            }

            // Get all ICinemachineComponents
            var allTypes
                = ReflectionHelpers.GetTypesInAllLoadedAssemblies(
                    t => t.IsSubclassOf(typeof(CinemachineComponentBase)));

            // Create a temp game object so we can instance behaviours
            var go = new GameObject("Cinemachine Temp Object");
            go.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
            foreach (var t in allTypes)
            {
                var b = go.AddComponent(t) as MonoBehaviour;
                var c = b != null ? (CinemachineComponentBase) b : null;
                if (c != null)
                {
                    var stage = c.Stage;
                    stageTypes[(int) stage].Add(t);
                }
            }

            DestroyImmediate(go);

            // Create the static lists
            for (var i = 0; i < stageTypes.Length; ++i)
            {
                stageTypes[i].Insert(0, null); // first item is "none"
                sStageData[i].types = stageTypes[i].ToArray();
                var names = new GUIContent[sStageData[i].types.Length];
                for (var n = 0; n < names.Length; ++n)
                    if (n == 0)
                    {
                        var useSimple
                            = i == (int) CinemachineCore.Stage.Aim
                              || i == (int) CinemachineCore.Stage.Body;
                        names[n] = new GUIContent(useSimple ? "Do nothing" : "none");
                    }
                    else
                    {
                        names[n] = new GUIContent(NicifyName(sStageData[i].types[n].Name));
                    }

                sStageData[i].PopupOptions = names;
            }
        }

        private string NicifyName(string name)
        {
            if (name.StartsWith("Cinemachine"))
                name = name.Substring(11); // Trim the prefix
            return ObjectNames.NicifyVariableName(name);
        }

        private void UpdateInstanceData()
        {
            // Invalidate the target's cache - this is to support Undo
            Target.InvalidateComponentPipeline();
            UpdateComponentEditors();
            UpdateStageState(m_components);
        }

        // This code dynamically builds editors for the pipeline components.
        // Expansion state is cached statically to preserve foldout state.
        private void UpdateComponentEditors()
        {
            var components = Target.GetComponentPipeline();
            var numComponents = components != null ? components.Length : 0;
            if (m_components == null || m_components.Length != numComponents)
                m_components = new CinemachineComponentBase[numComponents];
            var dirty = numComponents == 0;
            for (var i = 0; i < numComponents; ++i)
                if (components[i] != m_components[i])
                {
                    dirty = true;
                    m_components[i] = components[i];
                }

            if (dirty)
            {
                // Destroy the subeditors
                if (m_componentEditors != null)
                    foreach (var e in m_componentEditors)
                        if (e != null)
                            DestroyImmediate(e);

                // Create new editors
                m_componentEditors = new UnityEditor.Editor[numComponents];
                for (var i = 0; i < numComponents; ++i)
                {
                    var b = components[i];
                    if (b != null)
                        CreateCachedEditor(b, null, ref m_componentEditors[i]);
                }
            }
        }

        private void UpdateStageState(CinemachineComponentBase[] components)
        {
            m_stageState = new int[Enum.GetValues(typeof(CinemachineCore.Stage)).Length];
            m_stageError = new bool[Enum.GetValues(typeof(CinemachineCore.Stage)).Length];
            foreach (var c in components)
            {
                var stage = c.Stage;
                var index = 0;
                for (index = sStageData[(int) stage].types.Length - 1; index > 0; --index)
                    if (sStageData[(int) stage].types[index] == c.GetType())
                        break;
                m_stageState[(int) stage] = index;
                m_stageError[(int) stage] = !c.IsValid;
            }
        }

        [DrawGizmo(GizmoType.Active | GizmoType.InSelectionHierarchy, typeof(CinemachineVirtualCamera))]
        internal static void DrawVirtualCameraGizmos(CinemachineVirtualCamera vcam, GizmoType selectionType)
        {
            var pipeline = vcam.GetComponentPipeline();
            foreach (var c in pipeline)
            {
                MethodInfo method;
                if (CollectGizmoDrawers.m_GizmoDrawers.TryGetValue(c.GetType(), out method))
                    method.Invoke(null, new object[] {c, selectionType});
            }
        }

        // Static state and caches - Call UpdateStaticData() to refresh this
        private struct StageData
        {
            private string ExpandedKey
            {
                get { return "CNMCN_Core_Vcam_Expanded_" + Name; }
            }

            public bool IsExpanded
            {
                get { return EditorPrefs.GetBool(ExpandedKey, false); }
                set { EditorPrefs.SetBool(ExpandedKey, value); }
            }

            public string Name;
            public Type[] types; // first entry is null
            public GUIContent[] PopupOptions;
        }

        /// <summary>
        ///     Register with CinemachineVirtualCamera to create the pipeline in an undo-friendly manner
        /// </summary>
        [InitializeOnLoad]
        private class CreatePipelineWithUndo
        {
            static CreatePipelineWithUndo()
            {
                CinemachineVirtualCamera.CreatePipelineOverride =
                    (vcam, name, copyFrom) =>
                    {
                        // Create a new pipeline
                        var go = new GameObject(name);
                        Undo.RegisterCreatedObjectUndo(go, "created pipeline");
                        Undo.SetTransformParent(go.transform, vcam.transform, "parenting pipeline");
                        Undo.AddComponent<CinemachinePipeline>(go);

                        // If copying, transfer the components
                        if (copyFrom != null)
                            foreach (Component c in copyFrom)
                            {
                                var copy = Undo.AddComponent(go, c.GetType());
                                Undo.RecordObject(copy, "copying pipeline");
                                ReflectionHelpers.CopyFields(c, copy);
                            }

                        return go.transform;
                    };
                CinemachineVirtualCamera.DestroyPipelineOverride = pipeline =>
                {
                    Undo.DestroyObjectImmediate(pipeline);
                };
            }
        }

        // Because the cinemachine components are attached to hidden objects, their
        // gizmos don't get drawn by default.  We have to do it explicitly.
        [InitializeOnLoad]
        private static class CollectGizmoDrawers
        {
            public static readonly Dictionary<Type, MethodInfo> m_GizmoDrawers;

            static CollectGizmoDrawers()
            {
                m_GizmoDrawers = new Dictionary<Type, MethodInfo>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                    try
                    {
                        var added = false;
                        foreach (var method in type.GetMethods(
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                        {
                            if (added)
                                break;
                            if (!method.IsStatic)
                                continue;
                            var attributes = method.GetCustomAttributes(typeof(DrawGizmo), true) as DrawGizmo[];
                            foreach (var a in attributes)
                                if (typeof(CinemachineComponentBase).IsAssignableFrom(a.drawnType))
                                {
                                    m_GizmoDrawers.Add(a.drawnType, method);
                                    added = true;
                                    break;
                                }
                        }
                    }
                    catch (Exception)
                    {
                        // screw it
                    }
            }
        }
    }
}