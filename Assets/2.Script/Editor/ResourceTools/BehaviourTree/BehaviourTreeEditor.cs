using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeEditor : EditorWindow
{
    #region Variables

    private BehaviourTree.BehaviourTreeView behaviourTreeView = null;
    private BehaviourTree.InspectorView inspectorView = null;
    private BehaviourTree.BlackboardView blackboardView = null;

    private BehaviourTree.BehaviourTree targetBehaviourTree = null;
    
    private Label workSpaceTextField = null;

    private static EditorWindow cachedEditor = null;

    #endregion Variables

    #region Unity Events

    public void CreateGUI()
    {
        if (!ReferenceEquals(cachedEditor, null))
        {
            DestroyImmediate(this);
            return;
        }

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
        blackboardView = root.Q<BehaviourTree.BlackboardView>();

        Toolbar toolbar = root.Q<Toolbar>();

        workSpaceTextField = new Label();
        toolbar.Add(workSpaceTextField);

        toolbar.Add(new ToolbarButton(() => behaviourTreeView.FocusRootNode()) { text = "Focus root node" });

        behaviourTreeView.NodeViewSelected += OnNodeViewSelected;

        cachedEditor = this;

        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        if (ReferenceEquals(Selection.activeGameObject, null)) return;

        if (!Selection.activeGameObject.TryGetComponent(out BehaviourTree.BehaviourTree behaviourTree)) return;

        targetBehaviourTree = behaviourTree;
        behaviourTreeView?.PopulateView(targetBehaviourTree);

        if (!Selection.activeGameObject.TryGetComponent(out BehaviourTree.Blackboard blackboard)) return;

        blackboardView?.PopulateView(blackboard);
    }

    private void OnInspectorUpdate()
    {
        if (ReferenceEquals(behaviourTreeView, null)) return;

        behaviourTreeView.UpdateNodeViews();

        Vector3 currentWorkPosition = behaviourTreeView.viewTransform.position;
        workSpaceTextField.text = $"Current work position : {new Vector2(currentWorkPosition.x, currentWorkPosition.y)}";
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
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
    
    private void OnNodeViewSelected(BehaviourTree.NodeView nodeView)
    {
        inspectorView.UpdateInspector(nodeView);
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;

            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
        }
    }

    #endregion Events

    #endregion Methods
}
