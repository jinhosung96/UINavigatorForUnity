using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MoraeGames.Library.UniRxCustom
{
    [DisallowMultipleComponent]
    public class ObservableOnParticleSystemStopped : ObservableTriggerBase
    {
        #region variable

        Subject<Unit> onParticleSystemStopped;

        #endregion

        #region property

        public IObservable<Unit> OnParticleSystemStoppedAsObservable()
        {
            return onParticleSystemStopped ??= new Subject<Unit>();
        }

        #endregion

        #region unity event

        void OnParticleSystemStopped()
        {
            if (onParticleSystemStopped != null)
            {
                onParticleSystemStopped.OnNext(default);
            }
        }

        #endregion

        #region method

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onParticleSystemStopped != null)
            {
                onParticleSystemStopped.OnCompleted();
            }
        }

        #endregion
    }
}