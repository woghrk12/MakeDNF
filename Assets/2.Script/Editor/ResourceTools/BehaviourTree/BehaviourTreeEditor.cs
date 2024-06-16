using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeEditor : EditorWindow
{
    #region Variables

    private BehaviourTree.BehaviourTreeView behaviourTreeView = null;
    private BehaviourTree.InspectorView inspectorView = null;

    private BehaviourTree.BehaviourTree targetBehaviourTree = null;

    #endregion Variables

    #region Unity Events

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/2.Script/Editor/ResourceTools/BehaviourTree/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement
        // The style will be applied to the VisualElement and all of its children
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/2.Script/Editor/ResourceTools/BehaviourTree/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        behaviourTreeView = root.Q<BehaviourTree.BehaviourTreeView>();
        inspectorView = root.Q<BehaviourTree.InspectorView>();
        
        behaviourTreeView.NodeViewSelected += OnNodeViewSelected;

        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        if (ReferenceEquals(Selection.activeGameObject, null)) return;

        if (!Selection.activeGameObject.TryGetComponent(out BehaviourTree.BehaviourTree behaviourTree)) return;

        targetBehaviourTree = behaviourTree;
        behaviourTreeView.PopulateView(targetBehaviourTree);
    }

    private void OnInspectorUpdate()
    {
        behaviourTreeView?.UpdateNodeViews();
    }

    #endregion Unity Events

    #region Methods

    [MenuItem("Tools/Data/Behaviour Tree Tool")]
    public static void Init()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    #region Events

    {
        {

        }

    }

    private void OnNodeViewSelected(BehaviourTree.NodeView nodeView)
    {
        inspectorView.UpdateInspector(nodeView);
    }

    #endregion Events

    #endregion Methods
}
