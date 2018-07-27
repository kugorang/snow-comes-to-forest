#region

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using BlendTree = UnityEditor.Animations.BlendTree;

#endregion

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineStateDrivenCamera))]
    internal sealed class CinemachineStateDrivenCameraEditor
        : CinemachineVirtualCameraBaseEditor<CinemachineStateDrivenCamera>
    {
        private EmbeddeAssetEditor<CinemachineBlenderSettings> m_BlendsEditor;

        private string[] mCameraCandidates;
        private Dictionary<CinemachineVirtualCameraBase, int> mCameraIndexLookup;

        private ReorderableList mChildList;
        private ReorderableList mInstructionList;

        private string[] mLayerNames;
        private Dictionary<int, int> mStateIndexLookup;
        private string[] mTargetStateNames;
        private int[] mTargetStates;

        protected override List<string> GetExcludedPropertiesInInspector()
        {
            var excluded = base.GetExcludedPropertiesInInspector();
            excluded.Add(FieldPath(x => x.m_CustomBlends));
            excluded.Add(FieldPath(x => x.m_Instructions));
            return excluded;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_BlendsEditor = new EmbeddeAssetEditor<CinemachineBlenderSettings>(
                FieldPath(x => x.m_CustomBlends), this);
            m_BlendsEditor.OnChanged = b => { InternalEditorUtility.RepaintAllViews(); };
            m_BlendsEditor.OnCreateEditor = ed =>
            {
                var editor = ed as CinemachineBlenderSettingsEditor;
                if (editor != null)
                    editor.GetAllVirtualCameras = () => { return Target.ChildCameras; };
            };
            mChildList = null;
            mInstructionList = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (m_BlendsEditor != null)
                m_BlendsEditor.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            if (mInstructionList == null)
                SetupInstructionList();
            if (mChildList == null)
                SetupChildList();

            if (Target.m_AnimatedTarget == null)
                EditorGUILayout.HelpBox("An Animated Target is required", MessageType.Warning);

            // Ordinary properties
            DrawHeaderInInspector();
            DrawPropertyInInspector(FindProperty(x => x.m_Priority));
            DrawTargetsInInspector(FindProperty(x => x.m_Follow), FindProperty(x => x.m_LookAt));
            DrawPropertyInInspector(FindProperty(x => x.m_AnimatedTarget));

            // Layer index
            EditorGUI.BeginChangeCheck();
            UpdateTargetStates();
            UpdateCameraCandidates();
            var layerProp = FindAndExcludeProperty(x => x.m_LayerIndex);
            var currentLayer = layerProp.intValue;
            var layerSelection = EditorGUILayout.Popup("Layer", currentLayer, mLayerNames);
            if (currentLayer != layerSelection)
                layerProp.intValue = layerSelection;
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                Target.ValidateInstructions();
            }

            DrawRemainingPropertiesInInspector();

            // Blends
            m_BlendsEditor.DrawEditorCombo(
                "Create New Blender Asset",
                Target.gameObject.name + " Blends", "asset", string.Empty,
                "Custom Blends", false);

            // Instructions
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Separator();
            mInstructionList.DoLayoutList();

            // vcam children
            EditorGUILayout.Separator();
            mChildList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                Target.ValidateInstructions();
            }

            // Extensions
            DrawExtensionsWidgetInInspector();
        }

        private void UpdateTargetStates()
        {
            // Scrape the Animator Controller for states
            var ac = Target.m_AnimatedTarget == null
                ? null
                : Target.m_AnimatedTarget.runtimeAnimatorController as AnimatorController;
            var collector = new StateCollector();
            collector.CollectStates(ac, Target.m_LayerIndex);
            mTargetStates = collector.mStates.ToArray();
            mTargetStateNames = collector.mStateNames.ToArray();
            mStateIndexLookup = collector.mStateIndexLookup;

            if (ac == null)
            {
                mLayerNames = new string[0];
            }
            else
            {
                mLayerNames = new string[ac.layers.Length];
                for (var i = 0; i < ac.layers.Length; ++i)
                    mLayerNames[i] = ac.layers[i].name;
            }

            // Create the parent map in the target
            var parents
                = new List<CinemachineStateDrivenCamera.ParentHash>();
            foreach (var i in collector.mStateParentLookup)
                parents.Add(new CinemachineStateDrivenCamera.ParentHash(i.Key, i.Value));
            Target.m_ParentHash = parents.ToArray();
        }

        private int GetStateHashIndex(int stateHash)
        {
            if (stateHash == 0)
                return 0;
            if (!mStateIndexLookup.ContainsKey(stateHash))
                return 0;
            return mStateIndexLookup[stateHash];
        }

        private void UpdateCameraCandidates()
        {
            var vcams = new List<string>();
            mCameraIndexLookup = new Dictionary<CinemachineVirtualCameraBase, int>();
            vcams.Add("(none)");
            var children = Target.ChildCameras;
            foreach (var c in children)
            {
                mCameraIndexLookup[c] = vcams.Count;
                vcams.Add(c.Name);
            }

            mCameraCandidates = vcams.ToArray();
        }

        private int GetCameraIndex(Object obj)
        {
            if (obj == null || mCameraIndexLookup == null)
                return 0;
            var vcam = obj as CinemachineVirtualCameraBase;
            if (vcam == null)
                return 0;
            if (!mCameraIndexLookup.ContainsKey(vcam))
                return 0;
            return mCameraIndexLookup[vcam];
        }

        private void SetupInstructionList()
        {
            mInstructionList = new ReorderableList(serializedObject,
                serializedObject.FindProperty(() => Target.m_Instructions),
                true, true, true, true);

            // Needed for accessing field names as strings
            var def = new CinemachineStateDrivenCamera.Instruction();

            float vSpace = 2;
            float hSpace = 3;
            var floatFieldWidth = EditorGUIUtility.singleLineHeight * 2.5f;
            var hBigSpace = EditorGUIUtility.singleLineHeight * 2 / 3;
            mInstructionList.drawHeaderCallback = rect =>
            {
                var sharedWidth = rect.width - EditorGUIUtility.singleLineHeight
                                             - 2 * (hBigSpace + floatFieldWidth) - hSpace;
                rect.x += EditorGUIUtility.singleLineHeight;
                rect.width = sharedWidth / 2;
                EditorGUI.LabelField(rect, "State");

                rect.x += rect.width + hSpace;
                EditorGUI.LabelField(rect, "Camera");

                rect.x += rect.width + hBigSpace;
                rect.width = floatFieldWidth;
                EditorGUI.LabelField(rect, "Wait");

                rect.x += rect.width + hBigSpace;
                EditorGUI.LabelField(rect, "Min");
            };

            mInstructionList.drawElementCallback
                = (rect, index, isActive, isFocused) =>
                {
                    var instProp
                        = mInstructionList.serializedProperty.GetArrayElementAtIndex(index);
                    var sharedWidth = rect.width - 2 * (hBigSpace + floatFieldWidth) - hSpace;
                    rect.y += vSpace;
                    rect.height = EditorGUIUtility.singleLineHeight;

                    rect.width = sharedWidth / 2;
                    var stateSelProp = instProp.FindPropertyRelative(() => def.m_FullHash);
                    var currentState = GetStateHashIndex(stateSelProp.intValue);
                    var stateSelection = EditorGUI.Popup(rect, currentState, mTargetStateNames);
                    if (currentState != stateSelection)
                        stateSelProp.intValue = mTargetStates[stateSelection];

                    rect.x += rect.width + hSpace;
                    var vcamSelProp = instProp.FindPropertyRelative(() => def.m_VirtualCamera);
                    var currentVcam = GetCameraIndex(vcamSelProp.objectReferenceValue);
                    var vcamSelection = EditorGUI.Popup(rect, currentVcam, mCameraCandidates);
                    if (currentVcam != vcamSelection)
                        vcamSelProp.objectReferenceValue = vcamSelection == 0
                            ? null
                            : Target.ChildCameras[vcamSelection - 1];

                    var oldWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = hBigSpace;

                    rect.x += rect.width;
                    rect.width = floatFieldWidth + hBigSpace;
                    var activeAfterProp = instProp.FindPropertyRelative(() => def.m_ActivateAfter);
                    EditorGUI.PropertyField(rect, activeAfterProp, new GUIContent(" ", activeAfterProp.tooltip));

                    rect.x += rect.width;
                    var minDurationProp = instProp.FindPropertyRelative(() => def.m_MinDuration);
                    EditorGUI.PropertyField(rect, minDurationProp, new GUIContent(" ", minDurationProp.tooltip));

                    EditorGUIUtility.labelWidth = oldWidth;
                };

            mInstructionList.onAddDropdownCallback = (buttonRect, l) =>
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("New State"),
                    false, data =>
                    {
                        ++mInstructionList.serializedProperty.arraySize;
                        serializedObject.ApplyModifiedProperties();
                        Target.ValidateInstructions();
                    },
                    null);
                menu.AddItem(new GUIContent("All Unhandled States"),
                    false, data =>
                    {
                        var target = Target;
                        var len = mInstructionList.serializedProperty.arraySize;
                        for (var i = 0; i < mTargetStates.Length; ++i)
                        {
                            var hash = mTargetStates[i];
                            if (hash == 0)
                                continue;
                            var alreadyThere = false;
                            for (var j = 0; j < len; ++j)
                                if (target.m_Instructions[j].m_FullHash == hash)
                                {
                                    alreadyThere = true;
                                    break;
                                }

                            if (!alreadyThere)
                            {
                                var index = mInstructionList.serializedProperty.arraySize;
                                ++mInstructionList.serializedProperty.arraySize;
                                var p = mInstructionList.serializedProperty.GetArrayElementAtIndex(index);
                                p.FindPropertyRelative(() => def.m_FullHash).intValue = hash;
                            }
                        }

                        serializedObject.ApplyModifiedProperties();
                        Target.ValidateInstructions();
                    },
                    null);
                menu.ShowAsContext();
            };
        }

        private void SetupChildList()
        {
            float vSpace = 2;
            float hSpace = 3;
            var floatFieldWidth = EditorGUIUtility.singleLineHeight * 2.5f;
            var hBigSpace = EditorGUIUtility.singleLineHeight * 2 / 3;

            mChildList = new ReorderableList(serializedObject,
                serializedObject.FindProperty(() => Target.m_ChildCameras),
                true, true, true, true);

            mChildList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Virtual Camera Children");
                var priorityText = new GUIContent("Priority");
                var textDimensions = GUI.skin.label.CalcSize(priorityText);
                rect.x += rect.width - textDimensions.x;
                rect.width = textDimensions.x;
                EditorGUI.LabelField(rect, priorityText);
            };
            mChildList.drawElementCallback
                = (rect, index, isActive, isFocused) =>
                {
                    rect.y += vSpace;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.width -= floatFieldWidth + hBigSpace;
                    var element = mChildList.serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, element, GUIContent.none);

                    var oldWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = hBigSpace;
                    var obj = new SerializedObject(element.objectReferenceValue);
                    rect.x += rect.width + hSpace;
                    rect.width = floatFieldWidth + hBigSpace;
                    var priorityProp = obj.FindProperty(() => Target.m_Priority);
                    EditorGUI.PropertyField(rect, priorityProp, new GUIContent(" ", priorityProp.tooltip));
                    EditorGUIUtility.labelWidth = oldWidth;
                    obj.ApplyModifiedProperties();
                };
            mChildList.onChangedCallback = l =>
            {
                if (l.index < 0 || l.index >= l.serializedProperty.arraySize)
                    return;
                var o = l.serializedProperty.GetArrayElementAtIndex(
                    l.index).objectReferenceValue;
                var vcam = o != null
                    ? o as CinemachineVirtualCameraBase
                    : null;
                if (vcam != null)
                    vcam.transform.SetSiblingIndex(l.index);
            };
            mChildList.onAddCallback = l =>
            {
                var index = l.serializedProperty.arraySize;
                var vcam = CinemachineMenu.CreateDefaultVirtualCamera();
                Undo.SetTransformParent(vcam.transform, Target.transform, "");
                vcam.transform.SetSiblingIndex(index);
            };
            mChildList.onRemoveCallback = l =>
            {
                var o = l.serializedProperty.GetArrayElementAtIndex(
                    l.index).objectReferenceValue;
                var vcam = o != null
                    ? o as CinemachineVirtualCameraBase
                    : null;
                if (vcam != null)
                    Undo.DestroyObjectImmediate(vcam.gameObject);
            };
        }

        private class StateCollector
        {
            public Dictionary<int, int> mStateIndexLookup;
            public List<string> mStateNames;
            public Dictionary<int, int> mStateParentLookup;
            public List<int> mStates;

            public void CollectStates(AnimatorController ac, int layerIndex)
            {
                mStates = new List<int>();
                mStateNames = new List<string>();
                mStateIndexLookup = new Dictionary<int, int>();
                mStateParentLookup = new Dictionary<int, int>();

                mStateIndexLookup[0] = mStates.Count;
                mStateNames.Add("(default)");
                mStates.Add(0);

                if (ac != null && layerIndex >= 0 && layerIndex < ac.layers.Length)
                {
                    var fsm = ac.layers[layerIndex].stateMachine;
                    var name = fsm.name;
                    var hash = Animator.StringToHash(name);
                    CollectStatesFromFSM(fsm, name + ".", hash, string.Empty);
                }
            }

            private void CollectStatesFromFSM(
                AnimatorStateMachine fsm, string hashPrefix, int parentHash, string displayPrefix)
            {
                var states = fsm.states;
                for (var i = 0; i < states.Length; i++)
                {
                    var state = states[i].state;
                    var hash = AddState(hashPrefix + state.name, parentHash, displayPrefix + state.name);

                    // Also process clips as pseudo-states, if more than 1 is present.
                    // Since they don't have hashes, we can manufacture some.
                    var clips = CollectClipNames(state.motion);
                    if (clips.Count > 1)
                    {
                        var substatePrefix = displayPrefix + state.name + ".";
                        foreach (var name in clips)
                            AddState(
                                CinemachineStateDrivenCamera.CreateFakeHashName(hash, name),
                                hash, substatePrefix + name);
                    }
                }

                var fsmChildren = fsm.stateMachines;
                foreach (var child in fsmChildren)
                {
                    var name = hashPrefix + child.stateMachine.name;
                    var displayName = displayPrefix + child.stateMachine.name;
                    var hash = AddState(name, parentHash, displayName);
                    CollectStatesFromFSM(child.stateMachine, name + ".", hash, displayName + ".");
                }
            }

            private List<string> CollectClipNames(Motion motion)
            {
                var names = new List<string>();
                var clip = motion as AnimationClip;
                if (clip != null)
                    names.Add(clip.name);
                var tree = motion as BlendTree;
                if (tree != null)
                {
                    var children = tree.children;
                    foreach (var child in children)
                        names.AddRange(CollectClipNames(child.motion));
                }

                return names;
            }

            private int AddState(string hashName, int parentHash, string displayName)
            {
                var hash = Animator.StringToHash(hashName);
                if (parentHash != 0)
                    mStateParentLookup[hash] = parentHash;
                mStateIndexLookup[hash] = mStates.Count;
                mStateNames.Add(displayName);
                mStates.Add(hash);
                return hash;
            }
        }
    }
}