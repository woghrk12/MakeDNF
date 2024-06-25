using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public class BlackboardView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BlackboardView, UxmlTraits> { }

        public enum EVariableOption { NONE = -1, DELETE }

        #region Variables

        private Blackboard blackboard = null;

        private Type[] variableTypes = null;
        private string[] variableTypeNames = null;

        private string newVariableKey = string.Empty;
        private int selectedVariableType = -1;

        private bool isDisplayVariables = false;
        private SerializedProperty variableListProperty = null;

        private string[] variableOptions = null;
        private GUIStyle popupStyle = null;

        #endregion Variables

        #region Constructor

        public BlackboardView()
        {
            SetupVariableTypes();
            SetupVariableOptions();

            Add(new IMGUIContainer(() => UpdateInspector()));
        }

        #endregion Constructor

        #region Methods

        public void PopulateView(Blackboard blackboard)
        {
            this.blackboard = blackboard;
        }

        private void UpdateInspector()
        {
            if (ReferenceEquals(blackboard, null)) return;

            variableListProperty = new SerializedObject(blackboard).FindProperty("variableList");

            DisplayAddVariableLayer();

            EditorGUILayout.Space();

            DisplayVariableListLayer();
        }

        private void SetupVariableTypes()
        {
            var types = TypeCache.GetTypesDerivedFrom<BlackboardVariable>();
            var typeList = new List<Type>();
            var typeNameList = new List<string>();

            foreach (var type in types)
            {
                if (type.IsAbstract) continue;

                typeList.Add(type);
                typeNameList.Add(type.Name);
            }

            variableTypes = typeList.ToArray();
            variableTypeNames = typeNameList.ToArray();
        }

        private void SetupVariableOptions()
        {
            var elements = Enum.GetNames(typeof(EVariableOption));
            var elementNameList = new List<string>();

            for (int index = 0; index < elements.Length - 1; index++)
            {
                elementNameList.Add(elements[index].Substring(0, 1).ToUpper() + elements[index].Substring(1).ToLower());
            }

            variableOptions = elementNameList.ToArray();
        }

        private void DisplayAddVariableLayer()
        {
            EditorGUILayout.LabelField("Add variable", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Key", GUILayout.Width(80f));
                newVariableKey = EditorGUILayout.TextField(newVariableKey);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Type", GUILayout.Width(80f));
                selectedVariableType = EditorGUILayout.Popup(selectedVariableType, variableTypeNames);

                if (GUILayout.Button("Add", EditorStyles.miniButton))
                {
                    AddVariable();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DisplayVariableListLayer()
        {
            if (ReferenceEquals(popupStyle, null))
            {
                popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                popupStyle.imagePosition = ImagePosition.ImageOnly;
                popupStyle.margin.top += 3;
            }

            isDisplayVariables = EditorGUILayout.Foldout(isDisplayVariables, "Variables", true);

            if (isDisplayVariables)
            {
                for (int index = 0; index < variableListProperty.arraySize; index++)
                {
                    SerializedProperty serializedProperty = variableListProperty.GetArrayElementAtIndex(index);
                    SerializedObject serializedObject = new SerializedObject(serializedProperty.objectReferenceValue);
                    int popupOption = -1;

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PrefixLabel(serializedObject.FindProperty("Key").stringValue);
                        popupOption = EditorGUILayout.Popup(popupOption, variableOptions, popupStyle, GUILayout.MaxWidth(20f));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("value"), GUIContent.none);
                    }
                    EditorGUILayout.EndHorizontal();

                    serializedObject.ApplyModifiedProperties();

                    if (popupOption == (int)EVariableOption.DELETE)
                    {
                        RemoveVariable(serializedProperty.objectReferenceValue as BlackboardVariable);
                    }
                }
            }
        }

        private void AddVariable()
        {
            if (string.IsNullOrEmpty(newVariableKey) || newVariableKey.Equals("None"))
            {
                Debug.LogWarning($"Unavailable key name. Input : {newVariableKey}");
                return;
            }

            string newKey = new string(newVariableKey.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());

            // Check for key duplicates
            for (int index = 0; index < variableListProperty.arraySize; index++)
            {
                SerializedObject serializedObject = new SerializedObject(variableListProperty.GetArrayElementAtIndex(index).objectReferenceValue);
                string key = serializedObject.FindProperty("Key").stringValue;

                if (newVariableKey.Equals(key))
                {
                    Debug.LogWarning($"Variable {newKey} already exists.");
                    return;
                }
            }

            // Add variable
            Undo.RecordObject(blackboard, "Behaviour Tree (AddVariable)");

            BlackboardVariable variable = Undo.AddComponent(blackboard.gameObject, variableTypes[selectedVariableType]) as BlackboardVariable;

            variable.Key = newKey;
            variable.hideFlags = HideFlags.HideInInspector;

            blackboard.AddVariableForEditor(variable);

            // Reset field
            newVariableKey = string.Empty;
            selectedVariableType = -1;

            GUI.FocusControl("");
        }

        private void RemoveVariable(BlackboardVariable variable)
        {
            Undo.RecordObject(blackboard, "Behaviour Tree (RemoveVariable)");

            blackboard.RemoveVariableForEditor(variable);
            Undo.DestroyObjectImmediate(variable);
        }

        #endregion Methods
    }
}
