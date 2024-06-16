using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BehaviourTree.BehaviourTree))]
public class BehaviourTreeInspector : Editor
{
    #region Methods

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Behaviour Tree Tool"))
        {
            BehaviourTreeEditor.Init();
        }
    }

    #endregion Methods
}
