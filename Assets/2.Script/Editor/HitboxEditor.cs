using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HitboxController))]
public class HitboxEditor : Editor
{
    public enum ECoordinateMode { XZ, XY }
    public enum EHitboxEditMode { NONE = -1, SIZE, OFFSET, PIVOT }

    #region Variables

    [Header("Hitbox Controller components for play mode")]
    private List<HitboxController> hitboxControllerList = new();

    [Header("Hitbox Controller components for edit mode")]
    private HitboxController targetHitboxController = null;
    private DNFTransform targetDNFTransform = null;

    [Header("Property variables of the target component")]
    private SerializedProperty hitboxesProperty = null;
    private SerializedProperty hitboxTypeProperty = null;
    private SerializedProperty sizeProperty = null;
    private SerializedProperty offsetProperty = null;
    private SerializedProperty pivotProperty = null;

    [Header("The variables for controlling the editor")]
    private int hitboxIndex = 0;
    private ECoordinateMode coordMode = ECoordinateMode.XZ;
    private EHitboxEditMode editMode = EHitboxEditMode.NONE;

    #endregion Variables

    #region Unity Events

    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            hitboxControllerList = new List<HitboxController>();
            
            HitboxController[] controllers = FindObjectsOfType<HitboxController>(true);
            
            foreach (HitboxController controller in controllers)
            {
                hitboxControllerList.Add(controller);
            }
        }
        else
        {
            targetHitboxController = target as HitboxController;
            targetDNFTransform = targetHitboxController.GetComponent<DNFTransform>();

            coordMode = ECoordinateMode.XZ;
            editMode = EHitboxEditMode.NONE;

            hitboxIndex = 0;

            hitboxesProperty = serializedObject.FindProperty("hitboxes");

            if (hitboxesProperty.arraySize <= 0) return;

            SerializedProperty hitboxProperty = hitboxesProperty.GetArrayElementAtIndex(hitboxIndex);

            hitboxTypeProperty = hitboxProperty.FindPropertyRelative("hitboxType");
            sizeProperty = hitboxProperty.FindPropertyRelative("size");
            offsetProperty = hitboxProperty.FindPropertyRelative("offset");
            pivotProperty = hitboxProperty.FindPropertyRelative("pivot");
        }
    }

    private void OnDisable()
    {
        hitboxControllerList = null;

        targetHitboxController = null;
        hitboxesProperty = null;
        hitboxTypeProperty = null;
        sizeProperty = null;
        offsetProperty = null;
        pivotProperty = null;
    }

    private void OnSceneGUI()
    {
        if (Application.isPlaying) // Display all hitboxes in the scene when in play mode
        {
            foreach (HitboxController hitboxController in hitboxControllerList)
            {
                if (!hitboxController.gameObject.activeSelf) continue;
                if (!hitboxController.IsHitboxActivated) continue;

                SerializedObject serializedObject = new SerializedObject(hitboxController);

                SerializedProperty activeHitboxProperty = serializedObject.FindProperty("activeHitbox");

                EHitboxType hitboxType = (EHitboxType)activeHitboxProperty.FindPropertyRelative("hitboxType").intValue;
                Vector3 minHitboxPos = activeHitboxProperty.FindPropertyRelative("minHitboxPos").vector3Value;
                Vector3 maxHitboxPos = activeHitboxProperty.FindPropertyRelative("maxHitboxPos").vector3Value;

                DrawHitbox(hitboxType, minHitboxPos, maxHitboxPos);
            }
        }
        else // Edit the selected hitbox when in edit mode
        {
            serializedObject.Update();

            DrawGUI();

            if (hitboxesProperty.arraySize <= 0) return;

            Vector3 position = targetDNFTransform.Position;
            float localScale = targetDNFTransform.LocalScale;

            EHitboxType hitboxType = (EHitboxType)hitboxTypeProperty.intValue;
            Vector3 size = sizeProperty.vector3Value;
            Vector3 offset = offsetProperty.vector3Value;
            Vector3 pivot = pivotProperty.vector3Value;

            Vector3 minHitboxPos = position + offset - localScale * new Vector3(size.x * pivot.x, size.y * pivot.y, size.z * pivot.z);
            Vector3 maxHitboxPos = position + offset + localScale * new Vector3(size.x * (1f - pivot.x), size.y * (1f - pivot.y), size.z * (1f - pivot.z));

            DrawHitbox(hitboxType, minHitboxPos, maxHitboxPos);

            switch (editMode)
            {
                case EHitboxEditMode.SIZE:
                    DrawSizeHandler(hitboxType, position, ref size, offset, pivot, localScale);
                    break;

                case EHitboxEditMode.OFFSET:
                    DrawOffsetHandler(hitboxType, position, size, ref offset, pivot, localScale);
                    break;

                case EHitboxEditMode.PIVOT:
                    DrawPivotHandler(hitboxType, position, size, offset, ref pivot, localScale);
                    break;
            }

            hitboxTypeProperty.intValue = (int)hitboxType;
            sizeProperty.vector3Value = size;
            offsetProperty.vector3Value = offset;
            pivotProperty.vector3Value = pivot;

            serializedObject.ApplyModifiedProperties();
        }
    }

    #endregion Unity Events

    #region Methods

    #region GUI

    /// <summary>
    /// Draw the scene GUI layout to control the hitbox controller component.
    /// </summary>
    private void DrawGUI()
    {
        Handles.BeginGUI();
        {
            GUILayout.BeginArea(new Rect(20f, 20f, 300f, 500f));
            {
                var rect = EditorGUILayout.BeginVertical();
                {
                    GUI.color = new Color(0f, 0f, 0f, 0.9f);
                    GUI.Box(rect, GUIContent.none);

                    GUI.color = Color.white;

                    // Set the title of editor
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Hitbox Editor");
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUI.backgroundColor = Color.white;

                        if (GUILayout.Button("Add"))
                        {
                            hitboxesProperty.InsertArrayElementAtIndex(hitboxesProperty.arraySize);
                        }

                        GUI.enabled = hitboxesProperty.arraySize > 0;

                        if (GUILayout.Button("Remove"))
                        {
                            hitboxesProperty.DeleteArrayElementAtIndex(hitboxIndex);
                            hitboxIndex = hitboxIndex >= hitboxesProperty.arraySize ? 0 : hitboxIndex;
                        }

                        GUI.enabled = true;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (hitboxesProperty.arraySize > 0)
                    {
                        DrawHitboxInfoLayer();
                        DrawEditModeLayer();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }
        Handles.EndGUI();
    }

    /// <summary>
    /// Draw GUI to display the hitbox value, such as hitbox type, size, offset, and pivot.
    /// Draw GUI to select the hitbox to be edited.
    /// </summary>
    private void DrawHitboxInfoLayer()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.LabelField($"Hitbox Index : {hitboxIndex}");

            // Display GUI to edit the hitbox value
            EHitboxType hitboxType = (EHitboxType)EditorGUILayout.EnumPopup("Hitbox Type", (EHitboxType)hitboxTypeProperty.intValue);
            hitboxTypeProperty.intValue = (int)hitboxType;
            
            sizeProperty.vector3Value = EditorGUILayout.Vector3Field("Hitbox Size", sizeProperty.vector3Value);
            offsetProperty.vector3Value = EditorGUILayout.Vector3Field("Hitbox Size", offsetProperty.vector3Value);
            pivotProperty.vector3Value = EditorGUILayout.Vector3Field("Hitbox Size", pivotProperty.vector3Value);

            // Select the hitbox to edit
            EditorGUILayout.BeginHorizontal();
            {
                GUI.enabled = hitboxIndex - 1 >= 0;
                if (GUILayout.Button("<<"))
                {
                    hitboxIndex--;
                }

                GUI.enabled = hitboxIndex + 1 < hitboxesProperty.arraySize;
                if (GUILayout.Button(">>"))
                {
                    hitboxIndex++;
                }

                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Draw GUI to select the edit mode and the coordinate mode of the hitbox editor.
    /// </summary>
    private void DrawEditModeLayer()
    {
        // Set edit mode of editor
        EditorGUILayout.BeginHorizontal();
        {
            GUI.backgroundColor = Color.red;

            if (GUILayout.Button("Size"))
            {
                editMode = EHitboxEditMode.SIZE;
            }
            if (GUILayout.Button("Offset"))
            {
                editMode = EHitboxEditMode.OFFSET;
            }
            if (GUILayout.Button("Pivot"))
            {
                editMode = EHitboxEditMode.PIVOT;
            }
        }
        EditorGUILayout.EndHorizontal();

        // Set coordinate mode of editor
        GUI.backgroundColor = coordMode == ECoordinateMode.XZ ? Color.blue : Color.green;
        string buttonText = coordMode == ECoordinateMode.XZ ? "XZ Coord" : "XY Coord";
        if (GUILayout.Button(buttonText))
        {
            coordMode = coordMode == ECoordinateMode.XZ ? ECoordinateMode.XY : ECoordinateMode.XZ;
        }
    }

    #endregion GUI

    #region Scene GUI

    /// <summary>
    /// Draw the hitbox colliders by using the hitbox info.
    /// The collider of XZ coordinates will appear red, and the collider of XY coordinates will appear green.
    /// The shape of the collider of XZ coordinates is a circle, it only depends on the X-coordinate.
    /// </summary>
    private void DrawHitbox(EHitboxType hitboxType, Vector3 minHitboxPos, Vector3 maxHitboxPos)
    {
        Vector3 center = (minHitboxPos + maxHitboxPos) * 0.5f;
        Vector3 wireSize = maxHitboxPos - minHitboxPos;

        //// Draw the gizmo of XZ Coordinate
        Handles.color = Color.red;
        if (hitboxType == EHitboxType.BOX)
        {
            // Show the box-shaped hitbox by using x, z axis of DNF transform
            Handles.DrawWireCube(new Vector3(center.x, center.z * GlobalDefine.CONV_RATE, 0f), new Vector3(wireSize.x, wireSize.z * GlobalDefine.CONV_RATE, 0f));
        }
        else if (hitboxType == EHitboxType.CIRCLE)
        {
            // Show the circle-shaped hitbox by using x, z axis of DNF transform
            float angle = Mathf.Acos(GlobalDefine.CONV_RATE) * Mathf.Rad2Deg;
            Handles.DrawWireArc(new Vector3(center.x, center.z * GlobalDefine.CONV_RATE, 0f),
                Quaternion.AngleAxis(angle, Vector3.right) * Vector3.forward,
                Vector3.right,
                360f,
                wireSize.x * 0.5f);
        }

        // Draw the gizmo of XY Coordinate
        Handles.color = Color.green;
        // Show the box-shaped hitbox by using x, y axis of DNF transform
        Handles.DrawWireCube(new Vector3(center.x, center.z * GlobalDefine.CONV_RATE + center.y, 0f), new Vector3(wireSize.x, wireSize.y, 0f));
    }

    /// <summary>
    /// Draw the controllers of the hitbox size.
    /// The direction of the arrow changes depending on the pivot. 
    /// The criterion for the arrow changing direction is 0.5f.
    /// The minimum value for the size is 0f.
    /// </summary>
    private void DrawSizeHandler(EHitboxType hitboxType, Vector3 position, ref Vector3 size, Vector3 offset, Vector3 pivot, float localScale)
    {
        Vector3 minHitboxPos = position + offset - localScale * new Vector3(size.x * pivot.x, size.y * pivot.y, size.z * pivot.z);
        Vector3 maxHitboxPos = position + offset + localScale * new Vector3(size.x * (1f - pivot.x), size.y * (1f - pivot.y), size.z * (1f - pivot.z));

        // Draw the gizmo of X direction
        Handles.color = Color.red;
        float x = size.x;

        if (pivot.x <= 0.5f)
        {
            Vector3 xHandlerPos = Handles.Slider(new Vector3(maxHitboxPos.x, minHitboxPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.right);
            size.x = (xHandlerPos.x - position.x - offset.x) / ((1 - pivot.x) * localScale);

            if (size.x < 0f) size.x = 0f;
        }
        else
        {
            Vector3 xHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, minHitboxPos.z * GlobalDefine.CONV_RATE, 0f), -Vector3.right);
            size.x = (position.x + offset.x - xHandlerPos.x) / (pivot.x * localScale);

            if (size.x < 0f) size.x = 0f;
        }

        if (coordMode == ECoordinateMode.XZ)
        {
            // Draw the gizmo of Z direction
            Handles.color = Color.blue;
            float z = size.z;

            if (pivot.z <= 0.5f)
            {
                Vector3 zHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, maxHitboxPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.up);
                size.z = (zHandlerPos.y * GlobalDefine.INV_CONV_RATE - position.z - offset.z) / ((1 - pivot.z) * localScale);

                if (size.z < 0f) size.z = 0f;
            }
            else
            {
                Vector3 zHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, minHitboxPos.z * GlobalDefine.CONV_RATE, 0f), -Vector3.up);
                size.z = (position.z + offset.z - zHandlerPos.z * GlobalDefine.INV_CONV_RATE) / (pivot.z * localScale);

                if (size.z < 0f) size.z = 0f;
            }
        }
        else
        {
            // Draw the gizmo of Y direction
            Handles.color = Color.green;
            float y = size.y;

            if (pivot.y <= 0.5f)
            {
                Vector3 yHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, maxHitboxPos.y + position.z * GlobalDefine.CONV_RATE, 0f), Vector3.up);
                size.y = ((yHandlerPos.y - position.z * GlobalDefine.CONV_RATE) - position.y - offset.y) / ((1 - pivot.y) * localScale);

                if (size.y < 0f) size.y = 0f;
            }
            else
            {
                Vector3 yHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, minHitboxPos.y + position.z * GlobalDefine.CONV_RATE, 0f), -Vector3.up);
                size.y = (position.y + offset.y - (yHandlerPos.y - position.z * GlobalDefine.CONV_RATE)) / (pivot.y * localScale);

                if (size.y < 0f) size.y = 0f;
            }
        }
    }

    /// <summary>
    /// Draw the controllers of the hitbox offset.
    /// Determine how far apart the position of the object is from the position of the hitbox.
    /// </summary>
    private void DrawOffsetHandler(EHitboxType hitboxType, Vector3 position, Vector3 size, ref Vector3 offset, Vector3 pivot, float localScale)
    {
        Vector3 changedPos = position + offset;

        // Draw the gizmo of X direction
        Handles.color = Color.red;
        float x = offset.x;

        changedPos = Handles.Slider(new Vector3(changedPos.x, changedPos.y + changedPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.right);
        offset.x = changedPos.x - position.x;

        if (coordMode == ECoordinateMode.XZ)
        {
            // Draw the gizmo of Z direction
            Handles.color = Color.blue;
            float z = offset.z;

            changedPos = Handles.Slider(new Vector3(changedPos.x, changedPos.y + changedPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.up);
            offset.z = (changedPos.y - position.y) * GlobalDefine.INV_CONV_RATE - position.z;
        }
        else
        {
            // Draw the gizmo of Y direction
            Handles.color = Color.green;
            float y = offset.y;

            changedPos = Handles.Slider(new Vector3(changedPos.x, changedPos.y + changedPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.up);
            offset.y = changedPos.y - position.z * GlobalDefine.CONV_RATE - position.y;
        }
    }

    /// <summary>
    /// Draw the controllers of the hitbox pivot.
    /// Scale the hitbox with the pivot as the reference.
    /// The value of pivot is between 0f and 1f.
    /// </summary>
    private void DrawPivotHandler(EHitboxType hitboxType, Vector3 position, Vector3 size, Vector3 offset, ref Vector3 pivot, float localScale)
    {
        Vector3 minHitboxPos = position + offset - localScale * new Vector3(size.x * pivot.x, size.y * pivot.y, size.z * pivot.z);
        Vector3 maxHitboxPos = position + offset + localScale * new Vector3(size.x * (1f - pivot.x), size.y * (1f - pivot.y), size.z * (1f - pivot.z));

        // Draw the gizmo of X direction
        Handles.color = Color.red;
        float x = pivot.x;

        Vector3 xHandlerPos = Handles.Slider(new Vector3(Mathf.Lerp(minHitboxPos.x, maxHitboxPos.x, pivot.x), minHitboxPos.y + minHitboxPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.right);
        pivot.x = (xHandlerPos.x - minHitboxPos.x) / (maxHitboxPos.x - minHitboxPos.x);

        pivot.x = Mathf.Clamp01(pivot.x);

        if (coordMode == ECoordinateMode.XZ)
        {
            // Draw the gizmo of Z direction
            Handles.color = Color.blue;
            float z = pivot.z;

            Vector3 zHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, Mathf.Lerp(minHitboxPos.z, maxHitboxPos.z, pivot.z) * GlobalDefine.CONV_RATE, 0f), Vector3.up);
            pivot.z = (zHandlerPos.y * GlobalDefine.INV_CONV_RATE - minHitboxPos.z) / (maxHitboxPos.z - minHitboxPos.z);

            pivot.y = Mathf.Clamp01(pivot.y);
        }
        else
        {
            // Draw the gizmo of Y direction
            Handles.color = Color.green;
            float y = pivot.y;

            Vector3 yHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, Mathf.Lerp(minHitboxPos.y, maxHitboxPos.y, pivot.y) + minHitboxPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.up);
            pivot.y = (yHandlerPos.y - minHitboxPos.z * GlobalDefine.CONV_RATE - minHitboxPos.y) / (maxHitboxPos.y - minHitboxPos.y);

            pivot.z = Mathf.Clamp01(pivot.z);
        }
    }

    #endregion Scene GUI

    #endregion Methods
}
