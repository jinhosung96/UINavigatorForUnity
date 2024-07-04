using System;
using System.Collections.Generic;
using System.Linq;
#if UNIRX_SUPPORT
using MoraeGames.Library.UniRxCustom;
using UniRx;
#endif
using UnityEngine;

namespace MoraeGames.Library.Status
{
#if UNIRX_SUPPORT
    public class IntHPProperty
    {
        #region Properties
        
        public IntReactivePropertyWithRange CurrentHP { get; }
        public IObservable<(int hp, float ratio, int delta)> OnChangedHP => CurrentHP.Pairwise((prev, curr) => (curr, (float)curr / CurrentHP.Max, (curr - prev))).Share();
        public ToggleAbilityProperty IsInvincible { get; } = new(false);

        #endregion

        #region Constructor

        public IntHPProperty(int maxHP)
        {
            CurrentHP = new IntReactivePropertyWithRange(maxHP, 0, maxHP);
        }

        #endregion

        #region Public Methods

        public int TakeDamage(int damage)
        {
            int applyDamage = IsInvincible.Value ? 0 : Math.Min(CurrentHP.Value, damage);
            if (!IsInvincible.Value) CurrentHP.Value -= damage;
            return applyDamage;
        }

        public int TakeHeal(int heal)
        {
            int applyHeal = Math.Min(CurrentHP.Max - CurrentHP.Value, heal);
            CurrentHP.Value += heal;
            return applyHeal;
        }

        #endregion
    }
    
    public class FloatHPProperty
    {
        #region Properties
        
        public FloatReactivePropertyWithRange CurrentHP { get; }
        public IObservable<(float hp, float ratio, float delta)> OnChangedHP => CurrentHP.Pairwise((prev, curr) => (curr, (float)curr / CurrentHP.Max, (curr - prev))).Share();
        public ToggleAbilityProperty IsInvincible { get; } = new(false);

        #endregion

        #region Constructor

        public FloatHPProperty(float maxHP)
        {
            CurrentHP = new FloatReactivePropertyWithRange(maxHP, 0, maxHP);
        }

        #endregion

        #region Public Methods

        public float TakeDamage(float damage)
        {
            float applyDamage = IsInvincible.Value ? 0 : Math.Min(CurrentHP.Value, damage);
            if (!IsInvincible.Value) CurrentHP.Value -= damage;
            return applyDamage;
        }

        public float TakeHeal(float heal)
        {
            float applyHeal = Math.Min(CurrentHP.Max - CurrentHP.Value, heal);
            CurrentHP.Value += heal;
            return applyHeal;
        }

        #endregion
    }
#endif
    
    [System.Serializable]

    public class IntAbilityProperty
    {
        [field: SerializeField] public int BaseValue { get; set; }
        public int Value => (int)((BaseValue + AdditionEffects.Select(buff => buff.Value).Sum()) * MultiplicationEffects.Select(buff => 1 + buff.Value).Aggregate(1f, (a, b) => a * b));
        public List<IntAbilityEffect> AdditionEffects { get; } = new();
        public List<IntAbilityEffect> MultiplicationEffects { get; } = new();

        public IntAbilityProperty(int baseValue) => BaseValue = baseValue;
    }

    [Serializable]
    public class FloatAbilityProperty
    {
        [field: SerializeField] public float BaseValue { get; set; }
        public float Value => (BaseValue + AdditionEffects.Select(buff => buff.Value).Sum()) * MultiplicationEffects.Select(buff => 1 + buff.Value).Aggregate(1f, (a, b) => a * b);

        public List<FloatAbilityEffect> AdditionEffects { get; } = new();
        public List<FloatAbilityEffect> MultiplicationEffects { get; } = new();

        public FloatAbilityProperty(float baseValue) => BaseValue = baseValue;
    }

    public class ToggleAbilityProperty
    {
        [field: SerializeField] public bool BaseValue { get; set; }
        public bool Value => Effects.Any() ? !BaseValue : BaseValue;
        public List<ToggleAbilityEffect> Effects { get; } = new();
        public ToggleAbilityProperty(bool baseValue) => BaseValue = baseValue;
    }
}