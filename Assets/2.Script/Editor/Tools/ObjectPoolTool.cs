using UnityEditor;
using UnityEngine;

public class ObjectPoolTool : EditorWindow
{
    #region Variables

    private static readonly string enumName = "EObjectPoolList";

    [Header("Database variable")]
    private static ObjectPoolData objectPoolData = null;

    [Header("Variables for selecting and modifying data")]
    private static int selection = -1;
    private static ObjectPoolClip selectedClip = null;
    private static bool isClipDirty = false;
    private static bool isDataDirty = false;

    [Header("Position variables for scrolling the area")]
    private Vector2 listScrollPos = Vector2.zero;
    private Vector2 settingScrollPos = Vector2.zero;

    #endregion Variables

    #region Unity Events

    private void OnGUI()
    {
        if (objectPoolData == null) return;
     
        EditorGUILayout.BeginVertical();
        {
            DisplayTopLayer();

            EditorGUILayout.BeginHorizontal();
            {
                DisplayListLayer();

                if (selection >= 0)
                {
                    DisplayInfoLayer();
                }
            }
            EditorGUILayout.EndHorizontal();

            DisplayBottomLayer();
        }
        EditorGUILayout.EndVertical();
    }

    #endregion Unity Evnets

    #region Methods

    [MenuItem("Tools/Data/Object Pool Tool")]
    private static void Init()
    {
        LoadData();

        GetWindow<ObjectPoolTool>(false, "Object Pool Tool").Show();
    }

    /// <summary>
    /// Load the object pool database from the Assets folder.
    /// If the database does not exist, create a new database and save it.
    /// </summary>
    private static void LoadData()
    {
        if (objectPoolData == null)
        {
            objectPoolData = CreateInstance<ObjectPoolData>();
        }
        else
        {
            objectPoolData.ClearData();
        }

        ObjectPoolData savedData = AssetDatabase.LoadAssetAtPath<ObjectPoolData>(FilePath.OBJECT_POOL_DATA_PATH);
        if (savedData == null)
        {
            savedData = CreateInstance<ObjectPoolData>();
            AssetDatabase.CreateAsset(savedData, FilePath.OBJECT_POOL_DATA_PATH);
            AssetDatabase.Refresh();
        }
        else
        {
            int dataCount = savedData.DataCount;

            for (int index = 0; index < dataCount; index++)
            {
                objectPoolData.AddData(savedData.GetData(index));
            }
        }

        selection = -1;
        selectedClip = null;
        isClipDirty = false;
        isDataDirty = false;
    }

    /// <summary>
    /// Save the object pool database at the specified path and write an Enum file.
    /// </summary>
    private static void SaveData()
    {
        ObjectPoolData savedData = AssetDatabase.LoadAssetAtPath<ObjectPoolData>(FilePath.OBJECT_POOL_DATA_PATH);
        savedData.ClearData();

        if (isClipDirty)
        {
            objectPoolData.SetData(selection, selectedClip);
        }

        int dataCount = objectPoolData.DataCount;
        for (int index = 0; index < dataCount; index++)
        {
            savedData.AddData(objectPoolData.GetData(index));
        }

        isClipDirty = false;
        isDataDirty = false;

        EditorUtility.SetDirty(savedData);
        AssetDatabase.SaveAssetIfDirty(savedData);
        EditorHelper.CreateEnumStructure(enumName, objectPoolData);

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    #region Layer

    /// <summary>
    /// Display the top layer of the ObjectPool tool editor.
    /// This layer includes buttons for adding or removing data at the top of the editor.
    /// </summary>
    private void DisplayTopLayer()
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Add"))
            { 
                if(isClipDirty)
                {
                    if (EditorUtility.DisplayDialog("Warning", "Would you like to save the clip changes?", "Yes", "No"))
                    {
                        objectPoolData.SetData(selection, selectedClip);
                    }

                    isClipDirty = false;
                }

                selectedClip = objectPoolData.AddData("New Pool");
                selection = objectPoolData.DataCount - 1;

                isDataDirty = true;
            }
            if (selection >= 0 && GUILayout.Button("Copy"))
            {
                if (isClipDirty)
                {
                    if (EditorUtility.DisplayDialog("Warning", "Would you like to save the clip changes?", "Yes", "No"))
                    {
                        objectPoolData.SetData(selection, selectedClip);
                    }

                    isClipDirty = false;
                }

                selectedClip = objectPoolData.CopyData(selection);
                selection = objectPoolData.DataCount - 1;

                isDataDirty = true;
            }
            if (objectPoolData.DataCount > 0 && GUILayout.Button("Remove"))
            {
                objectPoolData.RemoveData(selection);

                selectedClip = null;
                selection = -1;

                isDataDirty = true;
            }
            if (objectPoolData.DataCount > 0 && GUILayout.Button("Clear"))
            {
                objectPoolData.ClearData();

                selectedClip = null;
                selection = -1;

                isDataDirty = true;
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Display the list layer of the ObjectPool tool editor.
    /// This layer includes the list of elements currently present in the database.
    /// Allow the user to click on an element to display a layer for editing the element.
    /// </summary>
    private void DisplayListLayer()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(EditorHelper.UI_WIDTH_MIDDLE));
        {
            EditorGUILayout.LabelField("Object Pool List", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical("Box");
            {
                listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos);
                {
                    int newSelection = GUILayout.SelectionGrid(selection, objectPoolData.GetNameList(true), 1);
                    if (newSelection != selection)
                    {
                        if (isClipDirty)
                        {
                            if (EditorUtility.DisplayDialog("Warning", "Would you like to save the clip changes?", "Yes", "No"))
                            {
                                objectPoolData.SetData(selection, selectedClip);
                                isDataDirty = true;
                            }

                            isClipDirty = false;
                        }

                        selection = newSelection;
                        selectedClip = objectPoolData.GetData(selection);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Display the info layer of the ObjectPool tool editor.
    /// When users select an element in the list layer, this layer displays information about the selected data.
    /// Users can also edit the data from this layer.
    /// </summary>
    private void DisplayInfoLayer()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Object Pool Setting" + (isClipDirty ? " *" : string.Empty), EditorStyles.boldLabel);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            settingScrollPos = EditorGUILayout.BeginScrollView(settingScrollPos);
            {
                if (selectedClip != null)
                {
                    EditorGUILayout.LabelField("ID", selectedClip.ID.ToString(), GUILayout.Width(EditorHelper.UI_WIDTH_LARGE));

                    string poolName = selectedClip.Name;
                    string newName = EditorGUILayout.TextField("Name", poolName, GUILayout.Width(EditorHelper.UI_WIDTH_LARGE));

                    if (!newName.Equals(poolName))
                    {
                        selectedClip.Name = newName;
                        isClipDirty = true;
                    }

                    GameObject objectPrefab = selectedClip.ObjectPrefab;
                    GameObject newPrefab = EditorGUILayout.ObjectField("Effect Prefab", objectPrefab, typeof(GameObject), false, GUILayout.Width(EditorHelper.UI_WIDTH_LARGE)) as GameObject;

                    if (objectPrefab != newPrefab)
                    {
                        selectedClip.ObjectPrefab = newPrefab;
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
    /// Display the bottom layer of the ObjectPool tool editor.
    /// This layer incldues buttons for reloading or saving the database at the bottom of the editor.
    /// </summary>
    private void DisplayBottomLayer()
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Reload Data"))
            {
                if (EditorUtility.DisplayDialog("Warning", "Are you sure to reload the Object Pool data?", "Yes", "No"))
                {
                    LoadData();
                }
            }
            if ((isClipDirty || isDataDirty) && GUILayout.Button("Save Object Pools"))
            {
                if (EditorUtility.DisplayDialog("Warning", "Are you sure to save the Object Pool data?", "Yes", "No"))
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
