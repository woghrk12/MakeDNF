using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        #region Variables

        private Editor editor = null;

        #endregion Variables

        #region Constructor

        public InspectorView() { }

        #endregion Constructor

        #region Methods

        public void UpdateInspector(NodeView nodeView)
        {
            Clear();

            if (!ReferenceEquals(editor, null))
            { 
                Object.DestroyImmediate(editor);
            }

            editor = Editor.CreateEditor(nodeView.Node);

            Add(new IMGUIContainer(() => editor.OnInspectorGUI()));
        }

        #endregion Methods
    }
}
