using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Hitbox))]
public class Hitbox_Editor : Editor
{
    public enum ECoordinateMode { XZ, XY }
    public enum EHitboxEditMode { NONE, SIZE, OFFSET, PIVOT }

    #region Variables

    [Header("The variable of target component")]
    private Hitbox hitbox = null;

    [Header("The variables for switching the edit mode")]
    private ECoordinateMode coordMode = ECoordinateMode.XZ;
    private EHitboxEditMode editMode = EHitboxEditMode.NONE;

    [Header("The variables for temporary info of the hitbox")]
    private Vector3 position = Vector3.zero;
    private Vector3 size = Vector3.zero;
    private Vector3 offset = Vector3.zero;
    private Vector3 pivot = Vector3.zero;
    private float localScale = 0f;

    #endregion Variables

    #region Unity Events

    private void OnEnable()
    {
        hitbox = target as Hitbox;

        size = hitbox.Size;
        offset = hitbox.Offset;
        pivot = hitbox.Pivot;

        Transform posTransform = hitbox.transform;
        Transform yPosTransform = posTransform.GetChild(0);
        Transform scaleTransform = yPosTransform.GetChild(0);

        position = new Vector3(posTransform.position.x, yPosTransform.localPosition.y, posTransform.position.y * GlobalDefine.InvConvRate);
        localScale = scaleTransform.localScale.x;

        if (hitbox.HitboxType == EHitboxType.NONE)
        {
            Debug.LogWarning($"The hitbox type is not set. Hitbox object : {hitbox.gameObject.name}");
        }
    }

    private void OnSceneGUI()
    {
        if (hitbox == null) return;

        if (hitbox.HitboxType == EHitboxType.NONE) return;

        Event curEvent = Event.current;
        if (!Application.isPlaying && curEvent.type == EventType.KeyDown)
        {
            if (curEvent.keyCode == KeyCode.F1) editMode = EHitboxEditMode.NONE;
            if (curEvent.keyCode == KeyCode.F2) editMode = EHitboxEditMode.SIZE;
            if (curEvent.keyCode == KeyCode.F3) editMode = EHitboxEditMode.OFFSET;
            if (curEvent.keyCode == KeyCode.F4) editMode = EHitboxEditMode.PIVOT;

            if (curEvent.keyCode == KeyCode.LeftControl)
                coordMode = coordMode == ECoordinateMode.XZ ? ECoordinateMode.XY : ECoordinateMode.XZ;
        }

        DrawHitbox();
        
        switch (editMode)
        {
            case EHitboxEditMode.SIZE:
                DrawSizeHandler();
                break;

            case EHitboxEditMode.OFFSET:
                DrawOffsetHandler();
                break;

            case EHitboxEditMode.PIVOT:
                DrawPivotHandler();
                break;
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
                hitbox.Size = EditorGUILayout.Vector3Field("Hitbox Size", hitbox.Size);
                hitbox.Offset = EditorGUILayout.Vector3Field("Hitbox Offset", hitbox.Offset);
                hitbox.Pivot = EditorGUILayout.Vector3Field("Hitbox Pivot", hitbox.Pivot);
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

                if (GUILayout.Button("Save"))
                {
                    hitbox.Size = size;
                    hitbox.Offset = offset;
                    hitbox.Pivot = pivot;

                    Debug.Log("Hitbox info is saved!");
                }
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
    private void DrawHitbox()
    {
        Vector3 minHitboxPos = position + offset - localScale * new Vector3(size.x * pivot.x, size.y * pivot.y, size.z * pivot.z);
        Vector3 maxHitboxPos = position + offset + localScale * new Vector3(size.x * (1f - pivot.x), size.y * (1f - pivot.y), size.z * (1f - pivot.z));
        Vector3 center = (minHitboxPos + maxHitboxPos) * 0.5f;
        Vector3 wireSize = maxHitboxPos - minHitboxPos;

        #region XZ Coordinates

        Handles.color = Color.red;
        if (hitbox.HitboxType == EHitboxType.BOX)
        {
            // Show the box-shaped hitbox by using x, z axis of DNF transform
            Handles.DrawWireCube(new Vector3(center.x, center.z * GlobalDefine.ConvRate, 0f), new Vector3(wireSize.x, wireSize.z * GlobalDefine.ConvRate, 0f));
        }
        else if (hitbox.HitboxType == EHitboxType.CIRCLE)
        {
            // Show the circle-shaped hitbox by using x, z axis of DNF transform
            float angle = Mathf.Acos(GlobalDefine.ConvRate) * Mathf.Rad2Deg;
            Handles.DrawWireArc(new Vector3(center.x, center.z * GlobalDefine.ConvRate, 0f),
                Quaternion.AngleAxis(angle, hitbox.transform.right) * hitbox.transform.forward,
                hitbox.transform.right,
                360f,
                wireSize.x * 0.5f);
        }

        #endregion XZ Coordinates

        #region XY Coordinates

        Handles.color = Color.green;
        // Show the box-shaped hitbox by using x, y axis of DNF transform
        Handles.DrawWireCube(new Vector3(center.x, center.z * GlobalDefine.ConvRate + center.y, 0f), new Vector3(wireSize.x, wireSize.y, 0f));

        #endregion XY Coordinates
    }

    /// <summary>
    /// Draw the controllers of the hitbox size.
    /// The direction of the arrow changes depending on the pivot. 
    /// The criterion for the arrow changing direction is 0.5f.
    /// The minimum value for the size is 0f.
    /// </summary>
    private void DrawSizeHandler()
    {
        Vector3 minHitboxPos = position + offset - localScale * new Vector3(size.x * pivot.x, size.y * pivot.y, size.z * pivot.z);
        Vector3 maxHitboxPos = position + offset + localScale * new Vector3(size.x * (1f - pivot.x), size.y * (1f - pivot.y), size.z * (1f - pivot.z));

        Handles.color = Color.red;
        if (pivot.x <= 0.5f)
        {
            Vector3 xHandlerPos = Handles.Slider(new Vector3(maxHitboxPos.x, minHitboxPos.z * GlobalDefine.ConvRate, 0f), Vector3.right);
            size.x = (xHandlerPos.x - position.x - offset.x) / ((1 - pivot.x) * localScale);

            if (size.x < 0f) size.x = 0f;
        }
        else
        {
            Vector3 xHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, minHitboxPos.z * GlobalDefine.ConvRate, 0f), -Vector3.right);
            size.x = (position.x + offset.x - xHandlerPos.x) / (pivot.x * localScale);

            if (size.x < 0f) size.x = 0f;
        }

        if (coordMode == ECoordinateMode.XZ)
        {
            Handles.color = Color.blue;
            if (pivot.z <= 0.5f)
            {
                Vector3 zHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, maxHitboxPos.z * GlobalDefine.ConvRate, 0f), Vector3.up);
                size.z = (zHandlerPos.y * GlobalDefine.InvConvRate - position.z - offset.z) / ((1 - pivot.z) * localScale);

                if (size.z < 0f) size.z = 0f;
            }
            else
            {
                Vector3 zHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, minHitboxPos.z * GlobalDefine.ConvRate, 0f), -Vector3.up);
                size.z = (position.z + offset.z - zHandlerPos.z * GlobalDefine.InvConvRate) / (pivot.z * localScale);

                if (size.z < 0f) size.z = 0f;
            }
        }
        else
        {
            Handles.color = Color.green;
            if (pivot.y <= 0.5f)
            {
                Vector3 yHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, maxHitboxPos.y + position.z * GlobalDefine.ConvRate, 0f), Vector3.up);
                size.y = ((yHandlerPos.y - position.z * GlobalDefine.ConvRate) - position.y - offset.y) / ((1 - pivot.y) * localScale);

                if (size.y < 0f) size.y = 0f;
            }
            else
            {
                Vector3 yHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, minHitboxPos.y + position.z * GlobalDefine.ConvRate, 0f), -Vector3.up);
                size.y = (position.y + offset.y - (yHandlerPos.y - position.z * GlobalDefine.ConvRate)) / (pivot.y * localScale);

                if (size.y < 0f) size.y = 0f;
            }
        }
    }

    /// <summary>
    /// Draw the controllers of the hitbox offset.
    /// Determine how far apart the position of the object is from the position of the hitbox.
    /// </summary>
    private void DrawOffsetHandler()
    {
        Vector3 changedPos = position + offset;

        Handles.color = Color.red;
        changedPos = Handles.Slider(new Vector3(changedPos.x, changedPos.y + changedPos.z * GlobalDefine.ConvRate, 0f), Vector3.right);
        offset.x = changedPos.x - position.x;

        if (coordMode == ECoordinateMode.XZ)
        {
            Handles.color = Color.blue;
            changedPos = Handles.Slider(new Vector3(changedPos.x, changedPos.y + changedPos.z * GlobalDefine.ConvRate, 0f), Vector3.up);
            offset.z = (changedPos.y - position.y) * GlobalDefine.InvConvRate - position.z;
        }
        else
        {
            Handles.color = Color.green;
            changedPos = Handles.Slider(new Vector3(changedPos.x, changedPos.y + changedPos.z * GlobalDefine.ConvRate, 0f), Vector3.up);
            offset.y = changedPos.y - position.z * GlobalDefine.ConvRate - position.y;
        }
    }

    /// <summary>
    /// Draw the controllers of the hitbox pivot.
    /// Scale the hitbox with the pivot as the reference.
    /// The value of pivot is between 0f and 1f.
    /// </summary>
    private void DrawPivotHandler()
    {
        Vector3 minHitboxPos = position + offset - localScale * new Vector3(size.x * pivot.x, size.y * pivot.y, size.z * pivot.z);
        Vector3 maxHitboxPos = position + offset + localScale * new Vector3(size.x * (1f - pivot.x), size.y * (1f - pivot.y), size.z * (1f - pivot.z));

        Handles.color = Color.red;
        Vector3 xHandlerPos = Handles.Slider(new Vector3(Mathf.Lerp(minHitboxPos.x, maxHitboxPos.x, pivot.x), minHitboxPos.y + minHitboxPos.z * GlobalDefine.ConvRate, 0f), Vector3.right);
        pivot.x = (xHandlerPos.x - minHitboxPos.x) / (maxHitboxPos.x - minHitboxPos.x);

        if (pivot.x < 0f) pivot.x = 0f;
        if (pivot.x > 1f) pivot.x = 1f;

        if (coordMode == ECoordinateMode.XZ)
        {
            Handles.color = Color.blue;
            Vector3 zHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, Mathf.Lerp(minHitboxPos.z, maxHitboxPos.z, pivot.z) * GlobalDefine.ConvRate, 0f), Vector3.up);
            pivot.z = (zHandlerPos.y * GlobalDefine.InvConvRate - minHitboxPos.z) / (maxHitboxPos.z - minHitboxPos.z);

            if (pivot.z < 0f) pivot.z = 0f;
            if (pivot.z > 1f) pivot.z = 1f;
        }
        else
        {
            Handles.color = Color.green;
            Vector3 yHandlerPos = Handles.Slider(new Vector3(minHitboxPos.x, Mathf.Lerp(minHitboxPos.y, maxHitboxPos.y, pivot.y) + minHitboxPos.z * GlobalDefine.ConvRate, 0f), Vector3.up);
            pivot.y = (yHandlerPos.y - minHitboxPos.z * GlobalDefine.ConvRate - minHitboxPos.y) / (maxHitboxPos.y - minHitboxPos.y);

            if (pivot.y < 0f) pivot.y = 0f;
            if (pivot.y > 1f) pivot.y = 1f;
        }
    }

    #endregion Methods
}
