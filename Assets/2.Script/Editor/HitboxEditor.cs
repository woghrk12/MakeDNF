using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Hitbox))]
public class HitboxEditor : Editor
{
    public enum ECoordinateMode { XZ, XY }
    public enum EHitboxEditMode { NONE = -1, SIZE, OFFSET, PIVOT }

    #region Variables

    [Header("Debug Mode")]
    private List<Hitbox> hitboxList = new();

    [Header("Edit Mode")]
    private Hitbox targetHitbox = null;

    [Header("The variables for switching the edit mode")]
    private ECoordinateMode coordMode = ECoordinateMode.XZ;
    private EHitboxEditMode editMode = EHitboxEditMode.NONE;

    [Header("The variables for temporary info of the hitbox")]
    private Vector3 size = Vector3.zero;
    private Vector3 offset = Vector3.zero;
    private Vector3 pivot = Vector3.zero;

    #endregion Variables

    #region Unity Events

    private void OnEnable()
    {
        Hitbox[] hitboxes = FindObjectsOfType<Hitbox>(true);

        foreach (Hitbox hitbox in hitboxes)
        {
            if (hitboxList.Contains(hitbox)) continue;

            hitboxList.Add(hitbox);
        }

        targetHitbox = target as Hitbox;

        size = targetHitbox.Size;
        offset = targetHitbox.Offset;
        pivot = targetHitbox.Pivot;

        if (targetHitbox.HitboxType == EHitboxType.NONE)
        {
            Debug.LogWarning($"The hitbox type is not set. Hitbox object : {targetHitbox.gameObject.name}");
        }
    }

    private void OnSceneGUI()
    {
        if (Application.isPlaying)
        {
            foreach (Hitbox hitbox in hitboxList)
            {
                if (!hitbox.gameObject.activeSelf) continue;

                Transform posTransform = hitbox.transform;
                Transform yPosTransform = posTransform.childCount > 0 ? posTransform.GetChild(0) : null;
                Transform scaleTransform = yPosTransform != null && yPosTransform.childCount > 0 ? yPosTransform.GetChild(0) : null;

                Vector3 position = new Vector3(posTransform.position.x, yPosTransform != null ? yPosTransform.localPosition.y : 0f, posTransform.position.y * GlobalDefine.INV_CONV_RATE);
                Vector3 size = hitbox.Size;
                Vector3 offset = hitbox.Offset;
                Vector3 pivot = hitbox.Pivot;
                float localScale = scaleTransform != null ? scaleTransform.localScale.x : 1f;

                DrawHitbox(hitbox.HitboxType, position, offset, size, pivot, localScale);
            }
        }
        else
        {
            Event curEvent = Event.current;
            if (curEvent.type == EventType.KeyDown)
            {
                if (curEvent.keyCode == KeyCode.F1) editMode = EHitboxEditMode.NONE;
                if (curEvent.keyCode == KeyCode.F2) editMode = EHitboxEditMode.SIZE;
                if (curEvent.keyCode == KeyCode.F3) editMode = EHitboxEditMode.OFFSET;
                if (curEvent.keyCode == KeyCode.F4) editMode = EHitboxEditMode.PIVOT;

                if (curEvent.keyCode == KeyCode.LeftControl)
                    coordMode = coordMode == ECoordinateMode.XZ ? ECoordinateMode.XY : ECoordinateMode.XZ;
            }

            Transform posTransform = targetHitbox.transform;
            Transform yPosTransform = posTransform.childCount > 0 ? posTransform.GetChild(0) : null;
            Transform scaleTransform = yPosTransform != null && yPosTransform.childCount > 0 ? yPosTransform.GetChild(0) : null;

            Vector3 position = new Vector3(posTransform.position.x, yPosTransform != null ? yPosTransform.localPosition.y : 0f, posTransform.position.y * GlobalDefine.INV_CONV_RATE);
            float localScale = scaleTransform != null ? scaleTransform.localScale.x : 1f;

            if (targetHitbox.HitboxType == EHitboxType.NONE) return;

            DrawHitbox(targetHitbox.HitboxType, position, offset, size, pivot, localScale);

            switch (editMode)
            {
                case EHitboxEditMode.SIZE:
                    DrawSizeHandler(targetHitbox.HitboxType, position, offset, ref size, pivot, localScale);
                    break;

                case EHitboxEditMode.OFFSET:
                    DrawOffsetHandler(targetHitbox.HitboxType, position, ref offset, size, pivot, localScale);
                    break;

                case EHitboxEditMode.PIVOT:
                    DrawPivotHandler(targetHitbox.HitboxType, position, offset, size, ref pivot, localScale);
                    break;
            }
        }
    }

    #endregion Unity Events

    #region Override Methods

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("Hitbox Setting");
            EditorGUILayout.BeginVertical("box");
            {
                targetHitbox.Size = EditorGUILayout.Vector3Field("Hitbox Size", targetHitbox.Size);
                targetHitbox.Offset = EditorGUILayout.Vector3Field("Hitbox Offset", targetHitbox.Offset);
                targetHitbox.Pivot = EditorGUILayout.Vector3Field("Hitbox Pivot", targetHitbox.Pivot);
            }
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.LabelField("Hitbox Setting");
            EditorGUILayout.BeginVertical("box");
            {
                size = EditorGUILayout.Vector3Field("Hitbox Size", size);
                offset = EditorGUILayout.Vector3Field("Hitbox Offset", offset);
                pivot = EditorGUILayout.Vector3Field("Hitbox Pivot", pivot);

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Reload"))
                    {
                        size = targetHitbox.Size;
                        offset = targetHitbox.Offset;
                        pivot = targetHitbox.Pivot;

                        Debug.Log("Hitbox info is reloaded!");
                    }
                    if (GUILayout.Button("Save"))
                    {
                        targetHitbox.Size = size;
                        targetHitbox.Offset = offset;
                        targetHitbox.Pivot = pivot;

                        Debug.Log("Hitbox info is saved!");
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
    }

    #endregion Override Methods

    #region Methods

    /// <summary>
    /// Draw the hitbox colliders by using the hitbox info.
    /// The collider of XZ coordinates will appear red, and the collider of XY coordinates will appear green.
    /// The shape of the collider of XZ coordinates is a circle, it only depends on the X-coordinate.
    /// </summary>
    private void DrawHitbox(EHitboxType hitboxType, Vector3 position, Vector3 offset, Vector3 size, Vector3 pivot, float localScale)
    {
        if (hitboxType == EHitboxType.NONE) return;

        Vector3 minHitboxPos = position + offset - localScale * new Vector3(size.x * pivot.x, size.y * pivot.y, size.z * pivot.z);
        Vector3 maxHitboxPos = position + offset + localScale * new Vector3(size.x * (1f - pivot.x), size.y * (1f - pivot.y), size.z * (1f - pivot.z));
        Vector3 center = (minHitboxPos + maxHitboxPos) * 0.5f;
        Vector3 wireSize = maxHitboxPos - minHitboxPos;

        Handles.color = Color.red;
        if (targetHitbox.HitboxType == EHitboxType.BOX)
        {
            // Show the box-shaped hitbox by using x, z axis of DNF transform
            Handles.DrawWireCube(new Vector3(center.x, center.z * GlobalDefine.CONV_RATE, 0f), new Vector3(wireSize.x, wireSize.z * GlobalDefine.CONV_RATE, 0f));
        }
        else if (targetHitbox.HitboxType == EHitboxType.CIRCLE)
        {
            // Show the circle-shaped hitbox by using x, z axis of DNF transform
            float angle = Mathf.Acos(GlobalDefine.CONV_RATE) * Mathf.Rad2Deg;
            Handles.DrawWireArc(new Vector3(center.x, center.z * GlobalDefine.CONV_RATE, 0f),
                Quaternion.AngleAxis(angle, Vector3.right) * Vector3.forward,
                Vector3.right,
                360f,
                wireSize.x * 0.5f);
        }

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

        Handles.color = Color.red;
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
            Handles.color = Color.blue;
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
            Handles.color = Color.green;
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
    private void DrawOffsetHandler(EHitboxType hitboxType, Vector3 position, ref Vector3 offset, Vector3 size, Vector3 pivot, float localScale)
    {
        Vector3 changedPos = position + offset;

        Handles.color = Color.red;
        changedPos = Handles.Slider(new Vector3(changedPos.x, changedPos.y + changedPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.right);
        offset.x = changedPos.x - position.x;

        if (coordMode == ECoordinateMode.XZ)
        {
            Handles.color = Color.blue;
            changedPos = Handles.Slider(new Vector3(changedPos.x, changedPos.y + changedPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.up);
            offset.z = (changedPos.y - position.y) * GlobalDefine.INV_CONV_RATE - position.z;
        }
        else
        {
            Handles.color = Color.green;
            changedPos = Handles.Slider(new Vector3(changedPos.x, changedPos.y + changedPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.up);
            offset.y = changedPos.y - position.z * GlobalDefine.CONV_RATE - position.y;
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

        Handles.color = Color.red;
        Vector3 xHandlerPos = Handles.Slider(new Vector3(Mathf.Lerp(minHitboxPos.x, maxHitboxPos.x, pivot.x), minHitboxPos.y + minHitboxPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.right);
        pivot.x = (xHandlerPos.x - minHitboxPos.x) / (maxHitboxPos.x - minHitboxPos.x);

        if (pivot.x < 0f) pivot.x = 0f;
        if (pivot.x > 1f) pivot.x = 1f;

        if (coordMode == ECoordinateMode.XZ)
        {
            Handles.color = Color.blue;
            Vector3 zHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, Mathf.Lerp(minHitboxPos.z, maxHitboxPos.z, pivot.z) * GlobalDefine.CONV_RATE, 0f), Vector3.up);
            pivot.z = (zHandlerPos.y * GlobalDefine.INV_CONV_RATE - minHitboxPos.z) / (maxHitboxPos.z - minHitboxPos.z);

            if (pivot.z < 0f) pivot.z = 0f;
            if (pivot.z > 1f) pivot.z = 1f;
        }
        else
        {
            Handles.color = Color.green;
            Vector3 yHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, Mathf.Lerp(minHitboxPos.y, maxHitboxPos.y, pivot.y) + minHitboxPos.z * GlobalDefine.CONV_RATE, 0f), Vector3.up);
            pivot.y = (yHandlerPos.y - minHitboxPos.z * GlobalDefine.CONV_RATE - minHitboxPos.y) / (maxHitboxPos.y - minHitboxPos.y);

            if (pivot.y < 0f) pivot.y = 0f;
            if (pivot.y > 1f) pivot.y = 1f;
        }
    }

    #endregion Methods
}
