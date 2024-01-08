using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HitboxController))]
public class HitboxEditor : Editor
{
    public enum ECoordinateMode { XZ, XY }
    public enum EHitboxEditMode { NONE = -1, SIZE, OFFSET, PIVOT }

    #region Variables

    [Header("Hitbox Controller components for debug mode")]
    private List<HitboxController> hitboxControllerList = new();

    [Header("Hitbox Controller components for edit mode")]
    private HitboxController targetHitboxController = null;

    [Header("The variables for switching the edit mode")]
    private ECoordinateMode coordMode = ECoordinateMode.XZ;
    private EHitboxEditMode editMode = EHitboxEditMode.NONE;

    [Header("The variables for temporary info of the hitbox")]
    private int hitboxIndex = 0;
    private EHitboxType[] hitboxTypeArray = null;
    private Vector3[] sizeArray = null;
    private Vector3[] offsetArray = null;
    private Vector3[] pivotArray = null;
    private bool isDataDirty = false;

    #endregion Variables

    #region Unity Events

    private void OnEnable()
    {
        hitboxControllerList.Clear();

        hitboxIndex = 0;
        hitboxTypeArray = new EHitboxType[0];
        sizeArray = new Vector3[0];
        offsetArray = new Vector3[0];
        pivotArray = new Vector3[0];
        isDataDirty = false;

        HitboxController[] controllers = FindObjectsOfType<HitboxController>(true);
        foreach (HitboxController controller in controllers)
        {
            hitboxControllerList.Add(controller);
        }

        targetHitboxController = target as HitboxController;

        LoadData();
    }

    private void OnDisable()
    {
        if (isDataDirty && EditorUtility.DisplayDialog("Warning", "Would you like to save the changes?", "Yes", "No"))
        { 
            SaveData();
        }
    }

    private void OnSceneGUI()
    {
        // Display all hitboxes in the scene when in play mode
        if (Application.isPlaying)
        {
            foreach (HitboxController controller in hitboxControllerList)
            {
                if (!controller.gameObject.activeSelf) continue;
                if (controller.ActiveHitbox == null) continue;

                Hitbox activeHitbox = controller.ActiveHitbox;

                Transform posTransform = controller.transform;
                Transform yPosTransform = posTransform.childCount > 0 ? posTransform.GetChild(0) : null;
                Transform scaleTransform = yPosTransform != null && yPosTransform.childCount > 0 ? yPosTransform.GetChild(0) : null;

                Vector3 position = new Vector3(posTransform.position.x, yPosTransform != null ? yPosTransform.localPosition.y : 0f, posTransform.position.y * GlobalDefine.INV_CONV_RATE);
                Vector3 size = activeHitbox.Size;
                Vector3 offset = activeHitbox.Offset;
                Vector3 pivot = activeHitbox.Pivot;
                float localScale = scaleTransform != null ? scaleTransform.localScale.x : 1f;

                DrawHitbox(activeHitbox.HitboxType, position, offset, size, pivot, localScale);
            }
        }
        // Edit the selected hitbox when not in play mode
        else
        {
            DrawGUI();

            if (hitboxTypeArray.Length <= 0) return;

            Transform posTransform = targetHitboxController.transform;
            Transform yPosTransform = posTransform.childCount > 0 ? posTransform.GetChild(0) : null;
            Transform scaleTransform = yPosTransform != null && yPosTransform.childCount > 0 ? yPosTransform.GetChild(0) : null;

            Vector3 position = new Vector3(posTransform.position.x, yPosTransform != null ? yPosTransform.localPosition.y : 0f, posTransform.position.y * GlobalDefine.INV_CONV_RATE);
            float localScale = scaleTransform != null ? scaleTransform.localScale.x : 1f;

            DrawHitbox(hitboxTypeArray[hitboxIndex], position, offsetArray[hitboxIndex], sizeArray[hitboxIndex], pivotArray[hitboxIndex], localScale);

            switch (editMode)
            {
                case EHitboxEditMode.SIZE:
                    DrawSizeHandler(hitboxTypeArray[hitboxIndex], position, offsetArray[hitboxIndex], ref sizeArray[hitboxIndex], pivotArray[hitboxIndex], localScale);
                    break;

                case EHitboxEditMode.OFFSET:
                    DrawOffsetHandler(hitboxTypeArray[hitboxIndex], position, ref offsetArray[hitboxIndex], sizeArray[hitboxIndex], pivotArray[hitboxIndex], localScale);
                    break;

                case EHitboxEditMode.PIVOT:
                    DrawPivotHandler(hitboxTypeArray[hitboxIndex], position, offsetArray[hitboxIndex], sizeArray[hitboxIndex], ref pivotArray[hitboxIndex], localScale);
                    break;
            }
        }
    }

    #endregion Unity Events

    #region Methods

    /// <summary>
    /// Load hitbox data from the target object.
    /// </summary>
    private void LoadData()
    {
        int numHitbox = targetHitboxController.Hitboxes.Length;

        hitboxTypeArray = new EHitboxType[numHitbox];
        sizeArray = new Vector3[numHitbox];
        offsetArray = new Vector3[numHitbox];
        pivotArray = new Vector3[numHitbox];

        for (int index = 0; index < numHitbox; index++)
        {
            hitboxTypeArray[index] = targetHitboxController.Hitboxes[index].HitboxType;
            sizeArray[index] = targetHitboxController.Hitboxes[index].Size;
            offsetArray[index] = targetHitboxController.Hitboxes[index].Offset;
            pivotArray[index] = targetHitboxController.Hitboxes[index].Pivot;
        }
    }

    /// <summary>
    /// Save the modified hitbox data to the target object.
    /// </summary>
    private void SaveData()
    {
        int numHitbox = hitboxTypeArray.Length;

        targetHitboxController.Hitboxes = new Hitbox[numHitbox];

        for (int index = 0; index < numHitbox; index++)
        {
            Hitbox hitbox = new();

            hitbox.HitboxType = hitboxTypeArray[index];
            hitbox.Size = sizeArray[index];
            hitbox.Offset = offsetArray[index];
            hitbox.Pivot = pivotArray[index];

            targetHitboxController.Hitboxes[index] = hitbox;
        }

        isDataDirty = false;
    }

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
                            hitboxTypeArray = ArrayHelper.Add(EHitboxType.BOX, hitboxTypeArray);
                            sizeArray = ArrayHelper.Add(new Vector3(1f, 1f, 1f), sizeArray);
                            offsetArray = ArrayHelper.Add(Vector3.zero, offsetArray);
                            pivotArray = ArrayHelper.Add(new Vector3(0.5f, 0f, 0.5f), pivotArray);
                            
                            hitboxIndex = hitboxTypeArray.Length - 1;
                            isDataDirty = true;
                        }

                        GUI.enabled = hitboxTypeArray.Length > 0;

                        if (GUILayout.Button("Remove"))
                        {
                            hitboxTypeArray = ArrayHelper.Remove(hitboxIndex, hitboxTypeArray);
                            sizeArray = ArrayHelper.Remove(hitboxIndex, sizeArray);
                            offsetArray = ArrayHelper.Remove(hitboxIndex, offsetArray);
                            pivotArray = ArrayHelper.Remove(hitboxIndex, pivotArray);

                            hitboxIndex = hitboxIndex >= hitboxTypeArray.Length ? 0 : hitboxIndex;
                            isDataDirty = true;
                        }

                        GUI.enabled = true;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (hitboxTypeArray.Length > 0)
                    {
                        DrawHitboxInfoLayer();
                        DrawEditModeLayer();
                    }

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUI.backgroundColor = Color.yellow;

                        if (GUILayout.Button("Reload") && EditorUtility.DisplayDialog("Warning", "Are you sure to reload the hitbox data?", "Yes", "No"))
                        {
                            LoadData();
                            Debug.Log("Hitbox info is reloaded!");
                        }
                        if (GUILayout.Button("Save") && EditorUtility.DisplayDialog("Warning", "Are you sure to save the hitbox data?", "Yes", "No"))
                        {
                            SaveData();
                            Debug.Log("Hitbox info is saved!");
                        }
                    }
                    EditorGUILayout.EndHorizontal();
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
            EHitboxType hitboxType = (EHitboxType)EditorGUILayout.EnumPopup("Hitbox Type", hitboxTypeArray[hitboxIndex]);
            if (hitboxType != hitboxTypeArray[hitboxIndex])
            {
                hitboxTypeArray[hitboxIndex] = hitboxType;
                isDataDirty = true;
            }
            
            Vector3 size = EditorGUILayout.Vector3Field("Hitbox Size", sizeArray[hitboxIndex]);
            if ((size - sizeArray[hitboxIndex]).sqrMagnitude > GlobalDefine.EPSILON)
            {
                sizeArray[hitboxIndex] = size;
                isDataDirty = true;
            }

            Vector3 offset = EditorGUILayout.Vector3Field("Hitbox Offset", offsetArray[hitboxIndex]);
            if ((offset - offsetArray[hitboxIndex]).sqrMagnitude > GlobalDefine.EPSILON)
            {
                offsetArray[hitboxIndex] = offset;
                isDataDirty = true;
            }

            Vector3 pivot = EditorGUILayout.Vector3Field("Hitbox Pivot", pivotArray[hitboxIndex]);
            if ((pivot - pivotArray[hitboxIndex]).sqrMagnitude > GlobalDefine.EPSILON)
            {
                pivotArray[hitboxIndex] = pivot;
                isDataDirty = true;
            }

            // Select the hitbox to edit
            if (hitboxTypeArray.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUI.enabled = hitboxIndex - 1 >= 0;
                    if (GUILayout.Button("<<"))
                    {
                        hitboxIndex--;
                    }

                    GUI.enabled = hitboxIndex + 1 < hitboxTypeArray.Length;
                    if (GUILayout.Button(">>"))
                    {
                        hitboxIndex++;
                    }

                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();
            }
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
    private void DrawHitbox(EHitboxType hitboxType, Vector3 position, Vector3 offset, Vector3 size, Vector3 pivot, float localScale)
    {
        Vector3 minHitboxPos = position + offset - localScale * new Vector3(size.x * pivot.x, size.y * pivot.y, size.z * pivot.z);
        Vector3 maxHitboxPos = position + offset + localScale * new Vector3(size.x * (1f - pivot.x), size.y * (1f - pivot.y), size.z * (1f - pivot.z));
        Vector3 center = (minHitboxPos + maxHitboxPos) * 0.5f;
        Vector3 wireSize = maxHitboxPos - minHitboxPos;

        // Draw the gizmo of XZ Coordinate
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
    private void DrawSizeHandler(EHitboxType hitboxType, Vector3 position, Vector3 offset, ref Vector3 size, Vector3 pivot, float localScale)
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

        if (Mathf.Abs(size.x - x) > GlobalDefine.EPSILON) isDataDirty = true;

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

            if (Mathf.Abs(size.z - z) > GlobalDefine.EPSILON) isDataDirty = true;
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

            if (Mathf.Abs(size.y - y) > GlobalDefine.EPSILON) isDataDirty = true;
        }
    }

    /// <summary>
    /// Draw the controllers of the hitbox offset.
    /// Determine how far apart the position of the object is from the position of the hitbox.
    /// </summary>
    private void DrawOffsetHandler(EHitboxType hitboxType, Vector3 position, ref Vector3 offset, Vector3 size, Vector3 pivot, float localScale)
    {
        Vector3 changedPos = position + offset;

        // Draw the gizmo of X direction
        Handles.color = Color.red;
        float x = offset.x;

        changedPos = Handles.Slider(new Vector3(changedPos.x, changedPos.y + changedPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.right);
        offset.x = changedPos.x - position.x;

        if (Mathf.Abs(offset.x - x) > GlobalDefine.EPSILON) isDataDirty = true;

        if (coordMode == ECoordinateMode.XZ)
        {
            // Draw the gizmo of Z direction
            Handles.color = Color.blue;
            float z = offset.z;

            changedPos = Handles.Slider(new Vector3(changedPos.x, changedPos.y + changedPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.up);
            offset.z = (changedPos.y - position.y) * GlobalDefine.INV_CONV_RATE - position.z;

            if (Mathf.Abs(offset.z - z) > GlobalDefine.EPSILON) isDataDirty = true;
        }
        else
        {
            // Draw the gizmo of Y direction
            Handles.color = Color.green;
            float y = offset.y;

            changedPos = Handles.Slider(new Vector3(changedPos.x, changedPos.y + changedPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.up);
            offset.y = changedPos.y - position.z * GlobalDefine.CONV_RATE - position.y;

            if (Mathf.Abs(offset.y - y) > GlobalDefine.EPSILON) isDataDirty = true;
        }
    }

    /// <summary>
    /// Draw the controllers of the hitbox pivot.
    /// Scale the hitbox with the pivot as the reference.
    /// The value of pivot is between 0f and 1f.
    /// </summary>
    private void DrawPivotHandler(EHitboxType hitboxType, Vector3 position, Vector3 offset, Vector3 size, ref Vector3 pivot, float localScale)
    {
        Vector3 minHitboxPos = position + offset - localScale * new Vector3(size.x * pivot.x, size.y * pivot.y, size.z * pivot.z);
        Vector3 maxHitboxPos = position + offset + localScale * new Vector3(size.x * (1f - pivot.x), size.y * (1f - pivot.y), size.z * (1f - pivot.z));

        // Draw the gizmo of X direction
        Handles.color = Color.red;
        float x = pivot.x;

        Vector3 xHandlerPos = Handles.Slider(new Vector3(Mathf.Lerp(minHitboxPos.x, maxHitboxPos.x, pivot.x), minHitboxPos.y + minHitboxPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.right);
        pivot.x = (xHandlerPos.x - minHitboxPos.x) / (maxHitboxPos.x - minHitboxPos.x);

        if (pivot.x < 0f) pivot.x = 0f;
        if (pivot.x > 1f) pivot.x = 1f;

        if (Mathf.Abs(pivot.x - x) > GlobalDefine.EPSILON) isDataDirty = true;

        if (coordMode == ECoordinateMode.XZ)
        {
            // Draw the gizmo of Z direction
            Handles.color = Color.blue;
            float z = pivot.z;

            Vector3 zHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, Mathf.Lerp(minHitboxPos.z, maxHitboxPos.z, pivot.z) * GlobalDefine.CONV_RATE, 0f), Vector3.up);
            pivot.z = (zHandlerPos.y * GlobalDefine.INV_CONV_RATE - minHitboxPos.z) / (maxHitboxPos.z - minHitboxPos.z);

            if (pivot.z < 0f) pivot.z = 0f;
            if (pivot.z > 1f) pivot.z = 1f;

            if (Mathf.Abs(pivot.z - z) > GlobalDefine.EPSILON) isDataDirty = true;
        }
        else
        {
            // Draw the gizmo of Y direction
            Handles.color = Color.green;
            float y = pivot.y;

            Vector3 yHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, Mathf.Lerp(minHitboxPos.y, maxHitboxPos.y, pivot.y) + minHitboxPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.up);
            pivot.y = (yHandlerPos.y - minHitboxPos.z * GlobalDefine.CONV_RATE - minHitboxPos.y) / (maxHitboxPos.y - minHitboxPos.y);

            if (pivot.y < 0f) pivot.y = 0f;
            if (pivot.y > 1f) pivot.y = 1f;

            if (Mathf.Abs(pivot.y - y) > GlobalDefine.EPSILON) isDataDirty = true;
        }
    }

    #endregion Scene GUI

    #endregion Methods
}
