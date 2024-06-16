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
        behaviourTreeView.NodeViewSelected += OnNodeViewSelected;

        inspectorView = root.Q<BehaviourTree.InspectorView>();

        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        BehaviourTree.BehaviourTree behaviourTree = Selection.activeObject as BehaviourTree.BehaviourTree;

        if (ReferenceEquals(behaviourTree, null))
        {
            if (ReferenceEquals(Selection.activeGameObject, null)) return;

            if (Selection.activeGameObject.TryGetComponent(out Enemy enemy))
            {
                behaviourTree = enemy.BehaviourController;
            }
        }

        if (Application.isPlaying)
        {
            if (ReferenceEquals(behaviourTree, null)) return;

            behaviourTreeView.PopulateView(behaviourTree);
        }
        else
        {
            if (ReferenceEquals(behaviourTree, null)) return;
            if (!AssetDatabase.CanOpenAssetInEditor(behaviourTree.GetInstanceID())) return;

            behaviourTreeView.PopulateView(behaviourTree);
        }
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

    [OnOpenAsset]
    private static bool OnOpenAsset(int instanceID, int line)
    {
        if (Selection.activeObject is BehaviourTree.BehaviourTree)
        {
            Init();

            return true;
        }

        return false;
    }

    private void OnNodeViewSelected(BehaviourTree.NodeView nodeView)
    {
        inspectorView.UpdateInspector(nodeView);
    }

    #endregion Events

    #endregion Methods
}
