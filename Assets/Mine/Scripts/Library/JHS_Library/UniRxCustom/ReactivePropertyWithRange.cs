#if UNIRX_SUPPORT
using System;
using UniRx;
using UnityEngine;

[Serializable]
public class IntReactivePropertyWithRange : IntReactiveProperty
{
    public int Min { get; }
    public int Max { get; }
    
    public IntReactivePropertyWithRange(int min, int max) : base()
    {
        Min = min;
        Max = max;
    }
    
    public IntReactivePropertyWithRange(int initialValue, int min, int max)
    {
        Min = min;
        Max = max;
        SetValue(initialValue);
    }

    protected sealed override void SetValue(int value)
    {
        base.SetValue(Mathf.Clamp(value, Min, Max));
    }
}

[Serializable]
public class FloatReactivePropertyWithRange : FloatReactiveProperty
{
    public float Min { get; }
    public float Max { get; }
    
    public FloatReactivePropertyWithRange(float min, float max) : base()
    {
        Min = min;
        Max = max;
    }
    
    public FloatReactivePropertyWithRange(float initialValue, float min, float max)
    {
        Min = min;
        Max = max;
        SetValue(initialValue);
    }

    protected sealed override void SetValue(float value)
    {
        base.SetValue(Mathf.Clamp(value, Min, Max));
    }
}

#endif