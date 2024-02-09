using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

using UnityObject = UnityEngine.Object;

public class EditorHelper
{
    #region Variables 

    [Header("Variables for indicating UI element's the width")]
    public static readonly float UI_WIDTH_LARGE = 450f;
    public static readonly float UI_WIDTH_MIDDLE = 300f;
    public static readonly float UI_WIDTH_SMALL = 200f;

    #endregion Variables

    #region Methods

    /// <summary>
    /// Retrieve the path of the given clip.
    /// If isFullPath is true, returns the path from the Assets folder, otherwise, from the Resources folder.
    /// </summary>
    /// <param name="clip">The unity object for which the path is to be retrieved </param>
    /// <param name="isFullPath">Indicate whether to return the path from the Assets folder (true) or from the Resources folder (false)</param>
    /// <returns>The path of the given clip</returns>
    public static string GetPath(UnityObject clip, bool isFullPath)
    {
        string path = AssetDatabase.GetAssetPath(clip);

        if (isFullPath)
        {
            return path;
        }

        string[] pathNode = AssetDatabase.GetAssetPath(clip).Split('/');
        string retString = string.Empty;
        bool isFindResourcesFolder = false;

        for (int index = 0; index < pathNode.Length - 1; index++)
        {
            if (isFindResourcesFolder)
            {
                retString += pathNode[index] + "/";
                continue;
            }

            if (pathNode[index].Equals("Resources"))
            {
                isFindResourcesFolder = true;
            }
        }

        return retString;
    }

    /// <summary>
    /// Write an enumeration based on the provided enum template file.
    /// The enumName specifies the name of the enum.
    /// The data contains elements from the database to be written as enum members.
    /// </summary>
    /// <typeparam name="T">The type of the BaseClip for which the enum is being written</typeparam>
    /// <param name="enumName">The name of the enum to be written</param>
    /// <param name="data">The database containing elements to be written as enum members</param>
    public static void CreateEnumStructure<T>(string enumName, BaseData<T> data) where T : BaseClip
    {
        StringBuilder builder = new();
        builder.AppendLine();

        int length = data.DataCount;
        string[] nameList = data.GetNameList(false);

        for (int index = 0; index < length; index++)
        {
            if (string.IsNullOrEmpty(nameList[index])) continue;

            string name = nameList[index];
            name = string.Concat(name.Where(character => !char.IsWhiteSpace(character)));
            builder.AppendLine("    " + name + " = " + index + ",");
        }

        string enumTemplate = File.ReadAllText(FilePath.ENUM_TEMPLATE_FILE_PATH);

        enumTemplate = enumTemplate.Replace("$ENUM$", enumName);
        enumTemplate = enumTemplate.Replace("$DATA$", builder.ToString());

        if (!Directory.Exists(FilePath.GAME_DATA_SCRIPT_FOLDER_PATH))
        {
            Directory.CreateDirectory(FilePath.GAME_DATA_SCRIPT_FOLDER_PATH);
        }

        string enumFileFullPath = FilePath.GAME_DATA_SCRIPT_FOLDER_PATH + enumName + ".cs";

        if (File.Exists(enumFileFullPath))
        {
            File.Delete(enumFileFullPath);
        }

        File.WriteAllText(enumFileFullPath, enumTemplate);
    }

    #endregion Methods
}
