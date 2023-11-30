public class GlobalDefine
{
    private static readonly int xRate = 16;
    private static readonly int yRate = 9;
    public static readonly float ConvRate = (float)yRate / xRate;
    public static readonly float InvConvRate = (float)xRate / yRate;
}
