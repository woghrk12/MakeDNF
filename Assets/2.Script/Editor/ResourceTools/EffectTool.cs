using UnityEditor;
using UnityEngine;

public class EffectTool : EditorWindow
{
    #region Variables 

    private static readonly string enumName = "EEffectList";

    [Header("Database variable")]
    private static EffectData effectData = null;

    [Header("Variables for selecting and modifying data")]
    private static int selection = -1;
    private static EffectClip selectedClip = null;
    private static bool isClipDirty = false;
    private static bool isDataDirty = false;

    [Header("Position variables for scrolling the area")]
    private Vector2 listScrollPos = Vector2.zero;
    private Vector2 settingScrollPos = Vector2.zero;

    #endregion Variables

    #region Unity Events

    private void OnGUI()
    {
        if (effectData == null) return;

        EditorGUILayout.BeginVertical();
        {
            DisplayTopLayer();

            EditorGUILayout.BeginHorizontal();
            {
                DisplayListLayer();

                if (selection >= 0)
                {
                    DisplayInfolayer();
                }
            }
            EditorGUILayout.EndHorizontal();

            DisplayBottomLayer();
        }
        EditorGUILayout.EndVertical();
    }

    #endregion Unity Events

    #region Methods

    [MenuItem("Tools/Data/Effect Tool")]
    private static void Init()
    {
        LoadData();

        GetWindow<EffectTool>(false, "Effect Tool").Show();
    }

    /// <summary>
    /// Load the effect database from the Assets folder.
    /// If the database does not exist, create a new database and save it.
    /// </summary>
    private static void LoadData()
    {
        if (effectData == null)
        {
            effectData = CreateInstance<EffectData>();
        }
        else
        {
            effectData.ClearData();
        }

        EffectData savedData = AssetDatabase.LoadAssetAtPath<EffectData>(FilePath.EFFECT_DATA_PATH);
        if (savedData == null)
        {
            savedData = CreateInstance<EffectData>();
            AssetDatabase.CreateAsset(savedData, FilePath.EFFECT_DATA_PATH);
            AssetDatabase.Refresh();
        }
        else
        {
            int dataCount = savedData.DataCount;

            for (int index = 0; index < dataCount; index++)
            {
                effectData.AddData(savedData.GetData(index));
            }
        }

        selection = -1;
        selectedClip = null;
        isClipDirty = false;
        isDataDirty = false;
    }

    /// <summary>
    /// Save the effect database at the specified path and write an Enum file.
    /// </summary>
    private static void SaveData()
    {
        EffectData savedData = AssetDatabase.LoadAssetAtPath<EffectData>(FilePath.EFFECT_DATA_PATH);
        savedData.ClearData();

        if (isClipDirty)
        {
            effectData.SetData(selection, selectedClip);
        }

        int dataCount = effectData.DataCount;
        for (int index = 0; index < dataCount; index++)
        {
            savedData.AddData(effectData.GetData(index));
        }

        isClipDirty = false;
        isDataDirty = false;
        
        EditorUtility.SetDirty(savedData);
        AssetDatabase.SaveAssetIfDirty(savedData);
        EditorHelper.CreateEnumStructure(enumName, effectData);

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    #region Layer

    /// <summary>
    /// Display the top layer of the Effect tool editor.
    /// This layer includes buttons for adding or removing data at the top of the editor.
    /// </summary>
    private void DisplayTopLayer()
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Add"))
            {
                if (isClipDirty)
                {
                    if (EditorUtility.DisplayDialog("Warning", "Would you like to save the clip changes?", "Yes", "No"))
                    {
                        effectData.SetData(selection, selectedClip);
                    }

                    isClipDirty = false;
                }

                selectedClip = effectData.AddData("New Effect");
                selection = effectData.DataCount - 1;

                isDataDirty = true;
            }
            if (selection >= 0 && GUILayout.Button("Copy"))
            {
                if (isClipDirty)
                {
                    if (EditorUtility.DisplayDialog("Warning", "Would you like to save the clip changes?", "Yes", "No"))
                    {
                        effectData.SetData(selection, selectedClip);
                    }

                    isClipDirty = false;
                }

                selectedClip = effectData.CopyData(selection);
                selection = effectData.DataCount - 1;

                isDataDirty = true;
            }
            if (effectData.DataCount > 0 && GUILayout.Button("Remove"))
            {
                effectData.RemoveData(selection);

                selectedClip = null;
                selection = -1;

                isDataDirty = true;
            }
            if (effectData.DataCount > 0 && GUILayout.Button("Clear"))
            {
                effectData.ClearData();

                selectedClip = null;
                selection = -1;

                isDataDirty = true;
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Display the list layer of the Effect tool editor.
    /// This layer includes the list of elements currently present in the database.
    /// Allow the user to click on an element to display a layer for editing the element.
    /// </summary>
    private void DisplayListLayer()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(EditorHelper.UI_WIDTH_MIDDLE));
        {
            EditorGUILayout.LabelField("Effect List", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical("Box");
            {
                listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos);
                {
                    int newSelection = GUILayout.SelectionGrid(selection, effectData.GetNameList(true), 1);
                    if (newSelection != selection)
                    {
                        if (isClipDirty)
                        {
                            if (EditorUtility.DisplayDialog("Warning", "Would you like to save the clip changes?", "Yes", "No"))
                            {
                                effectData.SetData(selection, selectedClip);
                                isDataDirty = true;
                            }

                            isClipDirty = false;
                        }

                        selection = newSelection;
                        selectedClip = effectData.GetData(selection);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Display the info layer of the Effect tool editor.
    /// When users select an element in the list layer, this layer displays information about the selected data.
    /// Users can also edit the data from this layer.
    /// </summary>
    private void DisplayInfolayer()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Effect Setting" + (isClipDirty ? " *" : string.Empty), EditorStyles.boldLabel);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            settingScrollPos = EditorGUILayout.BeginScrollView(settingScrollPos);
            {
                if (selectedClip != null)
                {
                    EditorGUILayout.LabelField("ID", selectedClip.ID.ToString(), GUILayout.Width(EditorHelper.UI_WIDTH_LARGE)); 
                    
                    string effectName = selectedClip.Name;
                    string newName = EditorGUILayout.TextField("Name", effectName, GUILayout.Width(EditorHelper.UI_WIDTH_LARGE)); 

                    if (!newName.Equals(effectName))
                    {
                        selectedClip.Name = newName;
                        isClipDirty = true;
                    }

                    GameObject effectPrefab = selectedClip.EffectPrefab;
                    GameObject newPrefab = EditorGUILayout.ObjectField("Effect Prefab", effectPrefab, typeof(GameObject), false, GUILayout.Width(EditorHelper.UI_WIDTH_LARGE)) as GameObject;

                    if (effectPrefab != newPrefab)
                    {
                        selectedClip.EffectPrefab = newPrefab;
                        selectedClip.ResourcesPath = EditorHelper.GetPath(newPrefab, false) + newPrefab.name;
                        selectedClip.FullPath = EditorHelper.GetPath(newPrefab, true);
                        isClipDirty = true;
                    }

                    EditorGUILayout.LabelField("Full Path", selectedClip.FullPath);
                }
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Display the bottom layer of the Effect tool editor.
    /// This layer incldues buttons for reloading or saving the database at the bottom of the editor.
    /// </summary>
    private void DisplayBottomLayer()
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Reload Data"))
            {
                if (EditorUtility.DisplayDialog("Warning", "Are you sure to reload the Effect data?", "Yes", "No"))
                {
                    LoadData();
                }
            }
            if ((isClipDirty || isDataDirty) && GUILayout.Button("Save Effects"))
            {
                if (EditorUtility.DisplayDialog("Warning", "Are you sure to save the Effect data?", "Yes", "No"))
                {
                    SaveData();
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    #endregion Layer

    #endregion Methods
}
