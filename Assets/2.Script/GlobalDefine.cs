public class GlobalDefine
{
    private static readonly int xRate = 16;
    private static readonly int yRate = 9;
    public static readonly float ConvRate = (float)yRate / xRate;
    public static readonly float InvConvRate = (float)xRate / yRate;
}

public class ResourcePath
{
    public static readonly string PREFAB = "Prefab";
    public static readonly string PREFAB_INPUT = "Prefab/Input";
    public static readonly string KEYBOARD_INPUT_SYSTEM = "Prefab/Input/Keyboard Input System";
    public static readonly string SCREEN_INPUT_SYSTEM = "Prefab/Input/Screen Input System";
}

public enum EKeyName { NONE = -1, BASEATTACK, JUMP, SKILL1, SKILL2, SKILL3, SKILL4, INVENTORY, SKILL, STATUS }
