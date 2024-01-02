using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

using UnityObject = UnityEngine.Object;

public class EditorHelper
{
    #region Variables 

    public static readonly float UI_WIDTH_LARGE = 450f;
    public static readonly float UI_WIDTH_MIDDLE = 300f;
    public static readonly float UI_WIDTH_SMALL = 200f;

    #endregion Variables

    #region Methods

    public static string GetPath(UnityObject clip)
    {
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

        string enumTemplate = File.ReadAllText(FilePath.EnumTemplateFilePath);

        enumTemplate = enumTemplate.Replace("$ENUM$", enumName);
        enumTemplate = enumTemplate.Replace("$DATA$", builder.ToString());

        if (!Directory.Exists(FilePath.GameDataScriptFolderPath))
        {
            Directory.CreateDirectory(FilePath.GameDataScriptFolderPath);
        }

        string enumFileFullPath = FilePath.GameDataScriptFolderPath + enumName + ".cs";

        if (File.Exists(enumFileFullPath))
        {
            File.Delete(enumFileFullPath);
        }

        File.WriteAllText(enumFileFullPath, enumTemplate);
    }

    #endregion Methods
}
