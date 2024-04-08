using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class SkillCreationTool : EditorWindow
{
    public class SkillInfo
    {
        #region Variables

        private EClassType classType = EClassType.NONE;
        private string skillName = string.Empty;
        private ESkillType skillType = ESkillType.NONE;
        private bool isAttackable = false;
        private List<string> stateNameList = new();

        #endregion Variables

        #region Properties

        /// <summary>
        /// Class type of the character to use the implemented skill.
        /// </summary>
        public EClassType ClassType 
        {
            set { classType = value; }
            get { return classType; }
        }

        /// <summary>
        /// The name of skill to be implemented.
        /// </summary>
        public string SkillName
        {
            set { skillName = value; }
            get { return skillName; }
        }

        /// <summary>
        /// The type of skill to be implemented.
        /// The only one option can be selected between ACTIVE and PASSIVE.
        /// The only one option can be selected among BASEATTACK, COMMON, and CLASSSPECIFIC.
        /// </summary>
        public ESkillType SkillType
        {
            set
            {
                if (value.HasFlag(ESkillType.ACTIVE) && skillType.HasFlag(ESkillType.PASSIVE))
                {
                    value &= ~ESkillType.PASSIVE;

                    IsAttackable = false;
                    StateNameList.Clear();
                }
                else if (value.HasFlag(ESkillType.PASSIVE) && skillType.HasFlag(ESkillType.ACTIVE))
                {
                    value &= ~ESkillType.ACTIVE;
                }

                if (value.HasFlag(ESkillType.BASEATTACK) && (skillType.HasFlag(ESkillType.COMMON) || skillType.HasFlag(ESkillType.CLASSSPECIFIC)))
                {
                    value &= ~ESkillType.COMMON;
                    value &= ~ESkillType.CLASSSPECIFIC;
                }
                else if (value.HasFlag(ESkillType.COMMON) && (skillType.HasFlag(ESkillType.BASEATTACK) || skillType.HasFlag(ESkillType.CLASSSPECIFIC)))
                {
                    value &= ~ESkillType.BASEATTACK;
                    value &= ~ESkillType.CLASSSPECIFIC;
                }
                else if (value.HasFlag(ESkillType.CLASSSPECIFIC) && (skillType.HasFlag(ESkillType.BASEATTACK) || skillType.HasFlag(ESkillType.COMMON)))
                {
                    value &= ~ESkillType.BASEATTACK;
                    value &= ~ESkillType.COMMON;
                }

                skillType = value;
            }
            get { return skillType; }
        }

        /// <summary>
        /// A flag variable indicating whether the skill allows for directly attacking enemies.
        /// This option can only be set if the skill includes the ACTIVE type.
        /// </summary>
        public bool IsAttackable 
        {
            set { isAttackable = value; }
            get { return isAttackable; }
        }

        /// <summary>
        /// A list containing the names of each state of the skill.
        /// This option can only be set if the skill includes the ACTIVE type.
        /// The skill must contain at least one state.
        /// </summary>
        public List<string> StateNameList 
        {
            set { stateNameList = value; }
            get { return stateNameList; } 
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Validates the skill information set by the user when creating a skill template using the tool.
        /// </summary>
        /// <returns>True if the skill information is valid; otherwise, false</returns>
        public bool ValidateSkillInfo()
        {
            if (ClassType == EClassType.NONE)
            {
                EditorUtility.DisplayDialog("Warning", $"The class type is not available!\nInput class type : {ClassType}", "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(SkillName))
            {
                EditorUtility.DisplayDialog("Warning", "The name of skill is empty!", "Ok");
                return false;
            }

            if (SkillName.Any(c => char.IsWhiteSpace(c)))
            {
                EditorUtility.DisplayDialog("Warning", "The white space cannot be included in the skill name!", "Ok");
                return false;
            }

            if (((SkillType & ESkillType.ACTIVE) | (SkillType & ESkillType.PASSIVE)) == ESkillType.NONE
                || ((SkillType & ESkillType.BASEATTACK) | (SkillType & ESkillType.COMMON) | (SkillType & ESkillType.CLASSSPECIFIC)) == ESkillType.NONE)
            {
                EditorUtility.DisplayDialog("Warning", $"The skill type is not available!\nInput skill type : {SkillType}", "Ok");
                return false;
            }

            if (SkillType.HasFlag(ESkillType.ACTIVE))
            {
                if (stateNameList.Count <= 0)
                {
                    EditorUtility.DisplayDialog("Warning", "There is no skill state for skill!", "Ok");
                    return false;
                }

                for (int index = 0; index < stateNameList.Count; index++)
                {
                    if (stateNameList[index].Any(c => char.IsWhiteSpace(c)))
                    {
                        EditorUtility.DisplayDialog("Warning", $"There is a white space in the state name!\nIndex : {index}\nState name : {stateNameList[index]}", "Ok");
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion Methods
    }

    #region Variables

    /// <summary>
    /// The information of the skill to be implemented.
    /// </summary>
    private static SkillInfo skillInfo = null;

    /// <summary>
    /// A list for settting the names of each state when the skill includes the ACTIVE type.
    /// </summary>
    private static ReorderableList stateNameEditorList = null;

    #endregion Variables

    #region Unity Events

    private void OnEnable()
    {
        skillInfo = new SkillInfo();
        stateNameEditorList = new ReorderableList(skillInfo.StateNameList, typeof(string), true, true, true, true);

        stateNameEditorList.drawHeaderCallback += (rect) =>
        {
            EditorGUI.LabelField(rect, "State List");
        };

        stateNameEditorList.drawElementCallback += (rect, index, isActive, isFocused) =>
        {
            rect.height -= 4;
            rect.y += 2;

            skillInfo.StateNameList[index] = EditorGUI.TextField(rect, skillInfo.StateNameList[index]);
        };

        stateNameEditorList.onAddCallback += (list) =>
        {
            skillInfo.StateNameList.Add("New State");
            
            list.index = skillInfo.StateNameList.Count - 1;
        };

        stateNameEditorList.onRemoveCallback += (list) =>
        {
            skillInfo.StateNameList.RemoveAt(list.index);

            list.index = -1;
        };
    }

    private void OnDisable()
    {
        skillInfo = null;
        stateNameEditorList = null;
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            DisplayInfoLayer();
            DisplayBottomLayer();
        }
        EditorGUILayout.EndVertical();
    }

    #endregion Unity Events

    #region Methods

    [MenuItem("Tools/Creation/Skill Creation Tool")]
    private static void Init()
    {
        GetWindow<SkillCreationTool>(false, "Skill Creation Tool").Show();
    }

    #region Layer

    /// <summary>
    /// Display the info layer of the SkillCreation tool editor.
    /// Users can edit the data for the skill to be implemented.
    /// </summary>
    private void DisplayInfoLayer()
    {
        EditorGUILayout.BeginVertical("Box", GUILayout.Width(300f), GUILayout.Height(500f));
        {
            skillInfo.ClassType = (EClassType)EditorGUILayout.EnumPopup("Class Type", skillInfo.ClassType);

            EditorGUILayout.Separator();

            skillInfo.SkillName = EditorGUILayout.TextField("Skill Name", skillInfo.SkillName);
            skillInfo.SkillType = (ESkillType)EditorGUILayout.EnumFlagsField("Skill Type", skillInfo.SkillType);

            if (skillInfo.SkillType.HasFlag(ESkillType.ACTIVE))
            {
                skillInfo.IsAttackable = EditorGUILayout.Toggle("Attackable", skillInfo.IsAttackable);

                stateNameEditorList.DoLayoutList();
                
                if (skillInfo.StateNameList.Count > 0 && GUILayout.Button("Clear"))
                {
                    skillInfo.StateNameList.Clear();
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Display the bottom layer of the SkillCreation tool editor.
    /// This layer includes the button for creating the skill template class according to the data.
    /// </summary>
    private void DisplayBottomLayer()
    {
        EditorGUILayout.BeginVertical();
        {
            if (GUILayout.Button("Create"))
            {
                CreateSkillScript();
            }
        }
        EditorGUILayout.EndVertical();
    }

    #endregion Layer

    /// <summary>
    /// Create skill template scripts by using the skill info the user set.
    /// If the skill includes the ACTIVE type, skill state template scripts are also created;
    /// </summary>
    private void CreateSkillScript()
    {
        if (!skillInfo.ValidateSkillInfo()) return;

        string folderPath = FilePath.SKILL_SCRIPT_FOLDER_PATH;
        string className = string.Empty;
        switch (skillInfo.ClassType)
        {
            case EClassType.FIREKNIGHT:
                className = GlobalDefine.FIRE_KNIGHT;
                break;

            case EClassType.GROUNDMONK:
                className = GlobalDefine.GROUND_MONK;
                break;
        }

        folderPath += className + "/";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        folderPath += (skillInfo.SkillType.HasFlag(ESkillType.ACTIVE) ? skillInfo.SkillName : "Passive") + "/";
        
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string skillFilePath = folderPath + skillInfo.SkillName + ".cs";

        if (File.Exists(skillFilePath))
        {
            EditorUtility.DisplayDialog("Warning", $"The skill file already exists!\nSkill name : {skillInfo.SkillName}", "Ok");
            return;
        }

        string skillTemplate = File.ReadAllText(skillInfo.SkillType.HasFlag(ESkillType.ACTIVE) ? FilePath.ACTIVE_SKILL_TEMPLATE_FILE_PATH : FilePath.PASSIVE_SKILL_TEMPLATE_FILE_PATH);

        skillTemplate = skillTemplate.Replace("$ClassName$", className);
        skillTemplate = skillTemplate.Replace("$SkillName$", skillInfo.SkillName);

        if (skillInfo.SkillType.HasFlag(ESkillType.ACTIVE))
        {
            skillTemplate = skillTemplate.Replace("$Inheritance$", "ActiveSkill" + (skillInfo.IsAttackable ? ", IAttackable" : string.Empty));

            string stateElements = string.Empty;

            for (int index = 0; index < skillInfo.StateNameList.Count; index++)
            {
                stateElements += skillInfo.StateNameList[index];

                if (index < skillInfo.StateNameList.Count - 1)
                {
                    stateElements += ", ";
                }
            }

            skillTemplate = skillTemplate.Replace("$StateElement$", stateElements);
        }
        else if (skillInfo.SkillType.HasFlag(ESkillType.PASSIVE))
        {
            skillTemplate = skillTemplate.Replace("$Inheritance$", "PassiveSkill");
        }

        string flagElements = string.Empty;
        string[] names = System.Enum.GetNames(typeof(ESkillType));
        int itemCount = 0;

        foreach (string name in names)
        {
            if (!System.Enum.TryParse(name, out ESkillType item)) continue;
            if (item == ESkillType.NONE) continue;
            if (!skillInfo.SkillType.HasFlag(item)) continue;

            if (itemCount > 0)
            {
                flagElements += " | ";
            }

            flagElements += "ESkillType." + name;
            itemCount++;
        }

        skillTemplate = skillTemplate.Replace("$SkillTypeList$", flagElements);

        if (skillInfo.SkillType.HasFlag(ESkillType.ACTIVE))
        {
            if (skillInfo.IsAttackable)
            {
                skillTemplate = skillTemplate.Replace("$IAttackable Implementation Start$", string.Empty);
                skillTemplate = skillTemplate.Replace("$IAttackable Implementation End$", string.Empty);

                skillTemplate = skillTemplate.Replace("$IAttackable Initialization Start$", string.Empty);
                skillTemplate = skillTemplate.Replace("$IAttackable Initialization End$", string.Empty);
            }
            else
            {
                RemoveTextBetweenMarkers(ref skillTemplate, "$IAttackable Implementation Start$", "$IAttackable Implementation End$");
                RemoveTextBetweenMarkers(ref skillTemplate, "$IAttackable Initialization Start$", "$IAttackable Initialization End$");
            }
        }

        if (skillInfo.StateNameList.Count > 0)
        {
            System.Tuple<int, string> result = GetTextBetweenMarker(ref skillTemplate, "$State Initialization Start$", "$State Initialization End$");
            
            for (int index = skillInfo.StateNameList.Count - 1; index >= 0; index--)
            {
                string stateName = skillInfo.StateNameList[index];
                string stateTemplate = File.ReadAllText(FilePath.SKILL_STATE_TEMPLATE_FILE_PATH);

                skillTemplate = skillTemplate.Insert(result.Item1, result.Item2.Replace("$StateName$", stateName));
                
                stateTemplate = stateTemplate.Replace("$ClassName$", className);
                stateTemplate = stateTemplate.Replace("$SkillName$", skillInfo.SkillName);
                stateTemplate = stateTemplate.Replace("$SkillStateName$", stateName);

                File.WriteAllText(folderPath + stateName + ".cs", stateTemplate);
            }
        }
      
        File.WriteAllText(skillFilePath, skillTemplate);

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    #region Helper

    /// <summary>
    /// Get the substring between the start marker and the end marker from the original string with the markers removed..
    /// The returned substring from the original string will be removed.
    /// </summary>
    /// <param name="original">The original string from which the substring is to be extracted</param>
    /// <param name="startMarker">The marker indicating the start of the substring</param>
    /// <param name="endMarker">The marker indicating the end of the substring</param>
    /// <returns>A pair consisting of the index of the original string where the substring was located and the extracted substring</returns>
    private System.Tuple<int, string> GetTextBetweenMarker(ref string original, string startMarker, string endMarker)
    {
        int startIndex = original.IndexOf(startMarker);
        int endIndex = original.IndexOf(endMarker) + endMarker.Length;

        string subString = original.Substring(startIndex, endIndex - startIndex + 1);

        original = original.Remove(startIndex, endIndex - startIndex + 1);

        subString = subString.Replace(startMarker, string.Empty);
        subString = subString.Replace(endMarker, string.Empty);

        return new System.Tuple<int, string>(startIndex, subString);
    }

    /// <summary>
    /// Remove the substring between the start marker and the end marker from the original string with the markers removed..
    /// </summary>
    /// <param name="original">The original string from which the substring is to be removed</param>
    /// <param name="startMarker">The marker indicating the start of the substring</param>
    /// <param name="endMarker">The marker indicating the end of the substring</param>
    private void RemoveTextBetweenMarkers(ref string original, string startMarker, string endMarker)
    {
        int startIndex = original.IndexOf(startMarker);
        int endIndex = original.IndexOf(endMarker) + endMarker.Length;
        
        original = original.Remove(startIndex, endIndex - startIndex + 1);
    }

    #endregion Helper

    #endregion Methods
}
