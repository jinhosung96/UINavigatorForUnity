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
    public class HpProperty : FloatReactiveProperty
    {
        #region Properties
        
        public NumericAbilityProperty MaxHp { get; }
        public IObservable<(float hp, float ratio, float delta)> OnChangedHP => this.Pairwise((prev, curr) => (curr, curr / MaxHp.Value, curr - prev)).Share();
        public ToggleAbilityProperty IsInvincible { get; } = new(false);

        #endregion

        #region Constructor

        public HpProperty(float maxHP) : base(maxHP)
        {
            MaxHp = new NumericAbilityProperty(maxHP);
        }

        #endregion

        #region Public Methods

        public float TakeDamage(float damage)
        {
            float applyDamage = IsInvincible.Value ? 0 : Math.Min(Value, damage);
            if (!IsInvincible.Value) Value -= damage;
            return applyDamage;
        }

        public float TakeHeal(float heal)
        {
            float applyHeal = Math.Min(MaxHp.Value - Value, heal);
            Value += heal;
            return applyHeal;
        }

        #endregion
    }
#endif

    [Serializable]
    public class NumericAbilityProperty
    {
        [field: SerializeField] public float BaseValue { get; set; }
        public float Value => (BaseValue + AdditionEffects.Select(buff => buff.Value).Sum()) * (1 + SumEffects.Select(buff => buff.Value).Sum()) * MultiplicationEffects.Select(buff => 1 + buff.Value).Aggregate(1f, (a, b) => a * b);

        public List<NumericAbilityEffect> AdditionEffects { get; } = new();
        public List<NumericAbilityEffect> SumEffects { get; } = new();
        public List<NumericAbilityEffect> MultiplicationEffects { get; } = new();

        public NumericAbilityProperty(float baseValue) => BaseValue = baseValue;
    }

    public class ToggleAbilityProperty
    {
        [field: SerializeField] public bool BaseValue { get; set; }
        public bool Value => Effects.Any() ? !BaseValue : BaseValue;
        public List<ToggleAbilityEffect> Effects { get; } = new();
        public ToggleAbilityProperty(bool baseValue) => BaseValue = baseValue;
    }
}