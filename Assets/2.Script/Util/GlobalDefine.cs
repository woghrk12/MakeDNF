public class GlobalDefine
{
    private static readonly int X_RATE = 16;
    private static readonly int Y_RATE = 9;
    public static readonly float CONV_RATE = (float)Y_RATE / X_RATE;
    public static readonly float INV_CONV_RATE = (float)X_RATE / Y_RATE;

    public static readonly float STIFFNESS_TIME = 0.1f;

    public static readonly float EPSILON = 0.0001f;

    public static readonly string FIRE_KNIGHT = "FireKnight";
    public static readonly string GROUND_MONK = "GroundMonk";
}

public class FilePath
{
    
    public static readonly string ENUM_TEMPLATE_FILE_PATH = "Assets/Resources/Template/EnumTemplate.txt";

    public static readonly string GAME_DATA_SCRIPT_FOLDER_PATH = "Assets/2.Script/GameData/";
    public static readonly string EFFECT_DATA_PATH = "Assets/Resources/GameData/EffectData.asset";
    public static readonly string OBJECT_POOL_DATA_PATH = "Assets/Resources/GameData/ObjectPoolData.asset";

    public static readonly string ACTIVE_SKILL_TEMPLATE_FILE_PATH = "Assets/Resources/Template/ActiveSkillTemplate.txt";
    public static readonly string PASSIVE_SKILL_TEMPLATE_FILE_PATH = "Assets/Resources/Template/PassiveSkillTemplate.txt";
    public static readonly string SKILL_STATE_TEMPLATE_FILE_PATH = "Assets/Resources/Template/SkillStateTemplate.txt";

    public static readonly string SKILL_SCRIPT_FOLDER_PATH = "Assets/2.Script/Skill/";
}

public class ResourcePath
{
    public static readonly string KEYBOARD_INPUT_SYSTEM = "Input/Keyboard Input System";
    public static readonly string SCREEN_INPUT_SYSTEM = "Input/Screen Input System";

    public static readonly string EFFECT_DATA = "GameData/EffectData";
    public static readonly string OBJECT_POOL_DATA = "GameData/ObjectPoolData";
}

public class BehaviourCodeList
{
    public static readonly int IDLE_BEHAVIOUR_CODE = typeof(IdleBehaviour).GetHashCode();
    public static readonly int MOVE_BEHAVIOUR_CODE = typeof(MoveBehaviour).GetHashCode();
    public static readonly int JUMP_BEHAVIOUR_CODE = typeof(JumpBehaviour).GetHashCode();
    public static readonly int ATTACK_BEHAVIOUR_CODE = typeof(AttackBehaviour).GetHashCode();
    public static readonly int HIT_BEHAVIOUR_CODE = typeof(HitBehaviour).GetHashCode();
}

public enum EKeyName { NONE = -1, BASEATTACK, JUMP, SKILL1, SKILL2, SKILL3, SKILL4, INVENTORY, SKILL, STATUS }
public enum EAttackType { NONE = -1, BASEATTACK, SKILL, ADDITIONAL }
public enum EHitType { NONE  = -1, DIRECT, INDIRECT }
public enum EClassType { NONE = -1, FIREKNIGHT, GROUNDMONK }