using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoraeGames.Library.Status
{
    [System.Serializable]

    public class IntAbilityProperty
    {
        [field: SerializeField] public int BaseValue { get; private set; }
        public int Value => (int)((BaseValue + AdditionEffects.Select(buff => buff.Value).Sum()) * MultiplicationEffects.Select(buff => 1 + buff.Value).Aggregate(1f, (a, b) => a * b));
        public List<IntAbilityEffect> AdditionEffects { get; } = new();
        public List<IntAbilityEffect> MultiplicationEffects { get; } = new();

        public IntAbilityProperty(int baseValue) => BaseValue = baseValue;
    }

    [Serializable]
    public class FloatAbilityProperty
    {
        [field: SerializeField] public float BaseValue { get; private set; }
        public float Value => (BaseValue + AdditionEffects.Select(buff => buff.Value).Sum()) * MultiplicationEffects.Select(buff => 1 + buff.Value).Aggregate(1f, (a, b) => a * b);

        public List<FloatAbilityEffect> AdditionEffects { get; } = new();
        public List<FloatAbilityEffect> MultiplicationEffects { get; } = new();

        public FloatAbilityProperty(float baseValue) => BaseValue = baseValue;
    }

    public class ToggleAbilityProperty
    {
        [field: SerializeField] public bool BaseValue { get; private set; }
        public bool Value => Effects.Any() ? !BaseValue : BaseValue;
        public List<ToggleAbilityEffect> Effects { get; } = new();
        public ToggleAbilityProperty(bool baseValue) => BaseValue = baseValue;
    }
}