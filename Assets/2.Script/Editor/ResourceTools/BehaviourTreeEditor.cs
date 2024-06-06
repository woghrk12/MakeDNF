using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeEditor : EditorWindow
{
    [MenuItem("Tools/Data/Behaviour Tree Tool")]
    public static void Init()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/2.Script/Editor/ResourceTools/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement
        // The style will be applied to the VisualElement and all of its children
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/2.Script/Editor/ResourceTools/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);
    }
}
