using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HitboxDebugTool : EditorWindow
{
    #region Variables

    private List<HitboxController> hitboxControllerList = new();
    private string[] nameList = null;

    private int selection = -1;

    private Vector2 listScrollPos = Vector2.zero;

    private bool isActiveTool = false;

    #endregion Variables

    #region Unity Events

    private void OnEnable()
    {
        selection = -1;
        hitboxControllerList.Clear();

        FindAllHitboxController();

        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        selection = -1;
        hitboxControllerList.Clear();
        nameList = null;

        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            DisplayTopLayer();

            if (Application.isPlaying && isActiveTool)
            {
                DisplayListLayer();
            }
        }
        EditorGUILayout.EndVertical();
    }

    #endregion Unity Events

    #region Methods

    [MenuItem("Tools/Debug/Hitbox Debug Tool")]
    private static void Init()
    {
        GetWindow<HitboxDebugTool>(false, "Hitbox Debug Tool").Show();
    }

    #region Layer

    private void DisplayTopLayer()
    {
        EditorGUILayout.BeginVertical();
        {
            if (isActiveTool)
            {
                if (GUILayout.Button("Deactivate Tool"))
                {
                    isActiveTool = false;
                }
            }
            else
            {
                if (GUILayout.Button("Activate Tool"))
                {
                    isActiveTool = true;
                }
            }

            if (Application.isPlaying && isActiveTool && GUILayout.Button("Refresh hitbox controller"))
            {
                FindAllHitboxController();
            }

            if (Application.isPlaying && isActiveTool && GUILayout.Button("Reset selection"))
            {
                selection = -1;
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void DisplayListLayer()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(EditorHelper.UI_WIDTH_MIDDLE));
        {
            EditorGUILayout.LabelField("Hitbox Controller List", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical("Box");
            {
                listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos);
                {
                    selection = GUILayout.SelectionGrid(selection, nameList, 1);
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }

    #endregion Layer

    #region Events

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!Application.isPlaying) return;
        if (!isActiveTool) return;

        if (selection < 0) // Draw all active hitboxes
        {
            for (int index = 0; index < hitboxControllerList.Count; index++)
            {
                if (!hitboxControllerList[index].IsHitboxActivated) continue;

                SerializedObject serializedObject = new SerializedObject(hitboxControllerList[index]);

                EHitboxType hitboxType = (EHitboxType)serializedObject.FindProperty("hitboxType").intValue;
                Vector3 minHitboxPos = serializedObject.FindProperty("minHitboxPos").vector3Value;
                Vector3 maxHitboxPos = serializedObject.FindProperty("maxHitboxPos").vector3Value;

                DrawHitbox(hitboxType, minHitboxPos, maxHitboxPos);
            }
        }
        else // Draw the selected active hitbox
        {
            if (!hitboxControllerList[selection].IsHitboxActivated) return;

            SerializedObject serializedObject = new SerializedObject(hitboxControllerList[selection]);

            EHitboxType hitboxType = (EHitboxType)serializedObject.FindProperty("hitboxType").intValue;
            Vector3 minHitboxPos = serializedObject.FindProperty("minHitboxPos").vector3Value;
            Vector3 maxHitboxPos = serializedObject.FindProperty("maxHitboxPos").vector3Value;

            DrawHitbox(hitboxType, minHitboxPos, maxHitboxPos);

            sceneView.LookAt(Utilities.ConvertDNFPosToWorldPos((minHitboxPos + maxHitboxPos) * 0.5f));
        }
    }

    #endregion Events

    #region Helper

    /// <summary>
    /// 
    /// </summary>
    private void FindAllHitboxController()
    {
        hitboxControllerList.Clear();
        nameList = new string[0];

        HitboxController[] controllers = FindObjectsOfType<HitboxController>(true);

        for (int index = 0; index < controllers.Length; index++)
        {
            HitboxController controller = controllers[index];

            nameList = ArrayHelper.Add(controller.gameObject.name, nameList);

            hitboxControllerList.Add(controller);
        }
    }

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

    #endregion Helper

    #endregion Methods
}
 