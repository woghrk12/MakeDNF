public class GlobalDefine
{
    private static readonly int xRate = 16;
    private static readonly int yRate = 9;
    public static readonly float ConvRate = (float)yRate / xRate;
    public static readonly float InvConvRate = (float)xRate / yRate;
}
public enum EKeyName { NONE = -1, BASEATTACK, JUMP, SKILL1, SKILL2, SKILL3, SKILL4, INVENTORY, SKILL, STATUS }
