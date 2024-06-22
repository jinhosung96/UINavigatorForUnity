using System;
using UnityEngine;

namespace MoraeGames.Library.Status
{
    [System.Serializable]

    public class IntAbilityEffect
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public int Value { get; private set; }

        public IntAbilityEffect(int id, int value)
        {
            Id = id;
            Value = value;
        }
    }

    [Serializable]
    public class FloatAbilityEffect
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public float Value { get; private set; }

        public FloatAbilityEffect(int id, float value)
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