using UnityEngine;

public static class EaseHelper
{
    public delegate float EaseFunction(float s, float e, float v);

    public enum EEaseType
    { 
        LINEAR,
        EASE_IN_SINE,
        EASE_IN_CUBIC,
        EASE_IN_QUART,
        EASE_OUT_QUART,
    }

    public static EaseFunction GetEaseFloatFunction(EEaseType easeType)
        => easeType switch
        {
            EEaseType.LINEAR => Linear,
            EEaseType.EASE_IN_SINE => EaseInSine,
            EEaseType.EASE_IN_CUBIC => EaseInCubic,
            EEaseType.EASE_IN_QUART => EaseInQuart,
            EEaseType.EASE_OUT_QUART => EaseOutQuart,
            _ => null
        };

    public static float Linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value);
    }

    public static float EaseInSine(float start, float end, float value)
    {
        end -= start;
        return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
    }

    public static float EaseInCubic(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    public static float EaseInQuart(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    public static float EaseOutQuart(float start, float end, float value)
    {
        value--;
        end -= start;
        return -end * (value * value * value * value - 1) + start;
    }
}
