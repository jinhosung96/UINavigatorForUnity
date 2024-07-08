#if UNIRX_SUPPORT
using System;
using UniRx;
using UnityEngine;

namespace MoraeGames.Library.UniRxCustom
{
    [Serializable]
    public class IntReactivePropertyWithRange : IntReactiveProperty
    {
        [field: SerializeField] public int Min { get; set; }
        [field: SerializeField] public int Max { get; set; }

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
        [field: SerializeField] public float Min { get; set; }
        [field: SerializeField] public float Max { get; set; }

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
}

#endif