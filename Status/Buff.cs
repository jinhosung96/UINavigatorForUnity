using System;
using UnityEngine;

namespace MoraeGames.Library.Status
{

    [Serializable]
    public class NumericAbilityEffect
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public float Value { get; set; }

        public NumericAbilityEffect(int id, float value)
        {
            Id = id;
            Value = value;
        }
    }

    [Serializable]
    public class ToggleAbilityEffect
    {
        [field: SerializeField] public int Id { get; private set; }

        public ToggleAbilityEffect(int id)
        {
            Id = id;
        }
    }
}