using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DNFTransform))]
public class DNFTransformEditor : Editor
{
    public enum ECoordinateMode { XZ, XY }

    #region Variables

    [Header("Components of the target")]
    private DNFTransform targetDNFTransform = null;
    private Transform targetTransform = null;

    [Header("Property variables of the target component")]
    private SerializedProperty positionProperty = null;
    private SerializedProperty scaleProperty = null;
    private SerializedProperty isLeftProperty = null;
    private SerializedProperty isBoundaryOverrideProperty = null;

    [Header("The variables for controllling the editor")]
    private bool isActiveSceneGUI = false;
    private ECoordinateMode coordMode = ECoordinateMode.XZ;

    #endregion Variables

    #region Unity Events

    private void OnEnable()
    {
        targetDNFTransform = target as DNFTransform;
        targetTransform = targetDNFTransform.GetComponent<Transform>();

        positionProperty = serializedObject.FindProperty("position");
        scaleProperty = serializedObject.FindProperty("localScale");
        isLeftProperty = serializedObject.FindProperty("isLeft");
        isBoundaryOverrideProperty = serializedObject.FindProperty("isBoundaryOverride");

        isActiveSceneGUI = false;
        coordMode = ECoordinateMode.XZ;
    }

    private void OnDisable()
    {
        targetDNFTransform = null;
        targetTransform = null;

        positionProperty = null;
        scaleProperty = null;
        isLeftProperty = null;
        isBoundaryOverrideProperty = null;
    }

    private void OnSceneGUI()
    {
        if (!isActiveSceneGUI) return;

        serializedObject.Update();

        DrawPositionHandler();

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Initialize values of properties
        Vector3 position = positionProperty.vector3Value;
        float scale = scaleProperty.floatValue;
        bool isLeft = isLeftProperty.boolValue;
        bool isBoundaryOverride = isBoundaryOverrideProperty.boolValue;

        // Receive user input values
        position = EditorGUILayout.Vector3Field("Position", position);
        scale = EditorGUILayout.FloatField("Scale", scale);
        isLeft = EditorGUILayout.Toggle("Is Left", isLeft);
        isBoundaryOverride = EditorGUILayout.Toggle("Is Boundary Override", isBoundaryOverride);

        // Validate the values
        if (position.y < 0f)
        {
            Debug.LogError("The y value of position cannot be less than 0.");
            position.y = 0f;
        }
        if (scale <= 0f)
        {
            Debug.LogError("The scale value should be greater than 0.");
            scale = 1f;
        }

        // Set the values to target component
        targetTransform.position = Utilities.ConvertDNFPosToWorldPos(position);
        targetTransform.localScale = new Vector3((isLeft ? -1f : 1f) * scale, scale, 1f);

        // Set the values to the properties
        positionProperty.vector3Value = position;
        scaleProperty.floatValue = scale;
        isLeftProperty.boolValue = isLeft;
        isBoundaryOverrideProperty.boolValue = isBoundaryOverride;

        if (isActiveSceneGUI)
        {
            if (GUILayout.Button("Deactivate Scene GUI"))
            {
                isActiveSceneGUI = false;
            }

            // Draw buttons for controlling the edit mode
            if (coordMode == ECoordinateMode.XZ)
            {
                GUI.backgroundColor = Color.green;

                if (GUILayout.Button("Convert to XY"))
                {
                    coordMode = ECoordinateMode.XY;
                }
            }
            else // Coord mode is set as XY
            {
                GUI.backgroundColor = Color.blue;

                if (GUILayout.Button("Convert to XZ"))
                {
                    coordMode = ECoordinateMode.XZ;
                }
            }
        }
        else
        {
            if (GUILayout.Button("Activate Scene GUI"))
            {
                isActiveSceneGUI = true;
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Draw the handlers to control the position of DNFTransform component.
    /// The arrow handler with red color will be used to control X value of the position.
    /// The arrow handler with green color will be used to control Y value of the position.
    /// The arrow handler with blue color will be used to control Z value of the position.
    /// The box handler with green color will be used to control X, Y value of the position.
    /// The box handler with blue color will be used to control X, Z value of the position.
    /// </summary>
    private void DrawPositionHandler()
    {
        Vector3 position = positionProperty.vector3Value;
        float handleSize = 0.3f;
        Vector3 handleOffset = new Vector3(handleSize, handleSize, 0f);

        if (coordMode == ECoordinateMode.XY)
        {
            // Draw the gizmo of X direction
            Handles.color = Color.red;
            Vector3 xHandlerPos = Handles.Slider(Utilities.ConvertDNFPosToWorldPos(position), Vector3.right);
            position.x = xHandlerPos.x;

            // Draw the gizmo of Y direction
            Handles.color = Color.green;
            Vector3 yHandlerPos = Handles.Slider(Utilities.ConvertDNFPosToWorldPos(position), Vector3.up);
            position.y = yHandlerPos.y - position.z * GlobalDefine.CONV_RATE;

            // Draw the gizmo of XY direction
            Vector3 xyHandlerPos = Handles.Slider2D(Utilities.ConvertDNFPosToWorldPos(position) + handleOffset, Vector3.forward, Vector3.right, Vector3.up, handleSize, Handles.RectangleHandleCap, 0.1f);
            xyHandlerPos -= handleOffset;
            position.x = xyHandlerPos.x;
            position.y = xyHandlerPos.y - position.z * GlobalDefine.CONV_RATE;

            if (position.y < 0f)
            {
                position.y = 0f;
            }
        }
        else if(coordMode == ECoordinateMode.XZ)
        {
            // Draw the gizmo of X direction
            Handles.color = Color.red;
            Vector3 xzPosition = new Vector3(position.x, 0f, position.z);
            Vector3 xHandlerPos = Handles.Slider(Utilities.ConvertDNFPosToWorldPos(xzPosition), Vector3.right);
            position.x = xHandlerPos.x;

            // Draw the gizmo of Z direction
            Handles.color = Color.blue;
            xzPosition = new Vector3(position.x, 0f, position.z);
            Vector3 zHandlerPos = Handles.Slider(Utilities.ConvertDNFPosToWorldPos(xzPosition), Vector3.up);
            position.z = zHandlerPos.y * GlobalDefine.INV_CONV_RATE;

            // Draw the gizmo of XZ direction
            xzPosition = new Vector3(position.x, 0f, position.z);
            Vector3 xzHandlerPos = Handles.Slider2D(Utilities.ConvertDNFPosToWorldPos(xzPosition) + handleOffset, Vector3.forward, Vector3.right, Vector3.up, handleSize, Handles.RectangleHandleCap, 0.1f);
            xzHandlerPos -= handleOffset;
            position.x = xzHandlerPos.x;
            position.z = xzHandlerPos.y * GlobalDefine.INV_CONV_RATE;
        }

        positionProperty.vector3Value = position;
        targetTransform.position = Utilities.ConvertDNFPosToWorldPos(position);
    }

    #endregion Methods
}
