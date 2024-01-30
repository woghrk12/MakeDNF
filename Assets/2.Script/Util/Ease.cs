public static class EaseHelper
{
    public delegate float EaseFunction(float s, float e, float v);

    public enum EEaseType
    { 
        EASE_OUT_QUART,
    }

    public static EaseFunction GetEaseFloatFunction(EEaseType easeType)
        => easeType switch
        {
            EEaseType.EASE_OUT_QUART => EaseOutQuart,
            _ => null
        };

    public static float EaseOutQuart(float start, float end, float value)
    {
        value--;
        end -= start;
        return -end * (value * value * value * value - 1) + start;
    }
}
