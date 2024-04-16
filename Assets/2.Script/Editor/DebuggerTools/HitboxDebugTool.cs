using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HitboxDebugTool : EditorWindow
{
    #region Variables

    private static List<HitboxController> hitboxControllerList = new();

    private static int selection = -1;

    private static Vector2 listScrollPos = Vector2.zero;

    private static bool isActiveTool = false;

    #endregion Variables

    #region Unity Events

    private void OnEnable()
    {
        selection = -1;
        hitboxControllerList.Clear();

        isActiveTool = false;

        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        selection = -1;
        hitboxControllerList.Clear();

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

    /// <summary>
    /// Init the Hitbox Debug Tool, opening the Hitbox Debug Tool.
    /// </summary>
    [MenuItem("Tools/Debug/Hitbox Debug Tool")]
    private static void Init()
    {
        GetWindow<HitboxDebugTool>(false, "Hitbox Debug Tool").Show();
    }

    /// <summary>
    /// Init the Hitbox Debug Tool when entering the play mode.
    /// Find all Hitbox Controller components existing in the scene.
    /// </summary>
    [InitializeOnEnterPlayMode]
    private static void InitOnEnterPlayMode()
    {
        selection = -1;
        hitboxControllerList.Clear();

        isActiveTool = false;
    }

    #region Layer

    /// <summary>
    /// Display the top layer of the Hitbox Debug Tool editor.
    /// This layer includes buttons for controling the editor.
    /// </summary>
    private void DisplayTopLayer()
    {
        EditorGUILayout.BeginVertical();
        {
            if (isActiveTool)
            {
                if (GUILayout.Button("Deactivate Tool"))
                {
                    isActiveTool = false;

                    selection = -1;
                    hitboxControllerList.Clear();
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

    /// <summary>
    /// Display the list layer of the Hitbox Debug Tool editor.
    /// This layer includes the list of all elements in the scene containing the Hitbox Controller component.
    /// Allow the user to click on an element to focus the camera on the selected element.
    /// If the activeSelf of the target is false or the active hitbox of the target is null, the button according to the target is deactivated.
    /// </summary>
    private void DisplayListLayer()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(EditorHelper.UI_WIDTH_LARGE));
        {
            EditorGUILayout.LabelField("Hitbox Controller List", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical("Box");
            {
                listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos);
                {
                    for (int index = 0; index < hitboxControllerList.Count; index++)
                    {
                        HitboxController controller = hitboxControllerList[index];

                        GUI.enabled = controller.gameObject.activeSelf && controller.IsHitboxActivated;

                        if (GUILayout.Button(controller.gameObject.name))
                        {
                            selection = index;
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }

    #endregion Layer

    #region Events

    /// <summary>
    /// The event method called every Scene GUI update.
    /// </summary>
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
    /// Find all Hitbox Controller components existing in the scene.
    /// </summary>
    private void FindAllHitboxController()
    {
        hitboxControllerList.Clear();

        HitboxController[] controllers = FindObjectsOfType<HitboxController>(true);

        foreach (HitboxController controller in controllers)
        {
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
 