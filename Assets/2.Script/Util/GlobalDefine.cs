public class GlobalDefine
{
    private static readonly int xRate = 16;
    private static readonly int yRate = 9;
    public static readonly float ConvRate = (float)yRate / xRate;
    public static readonly float InvConvRate = (float)xRate / yRate;
}

public class FilePath
{
    public static readonly string EnumTemplateFilePath = "Assets/Resources/Template/EnumTemplate.txt";

    public static readonly string GameDataScriptFolderPath = "Assets/2.Script/GameData/";
    public static readonly string EffectDataPath = "Assets/Resources/GameData/EffectData.asset";
    public static readonly string OBJECT_POOL_DATA_PATH = "Assets/Resources/GameData/ObjectPoolData.asset";
}

public class ResourcePath
{
    public static readonly string KEYBOARD_INPUT_SYSTEM = "Input/Keyboard Input System";
    public static readonly string SCREEN_INPUT_SYSTEM = "Input/Screen Input System";
}

public class BehaviourCodeList
{
    public static readonly int idleBehaviourCode = typeof(IdleBehaviour).GetHashCode();
    public static readonly int attackBehaviourCode = typeof(AttackBehaviour).GetHashCode();
}

public enum EKeyName { NONE = -1, BASEATTACK, JUMP, SKILL1, SKILL2, SKILL3, SKILL4, INVENTORY, SKILL, STATUS }
