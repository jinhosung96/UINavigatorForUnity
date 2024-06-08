#if UNIRX_SUPPORT && PUN_2_SUPPORT
using System;
using UniRx;

public class TimerModule
{
    #region Field

    float startTime;
    float timerSetting;
    bool isTimerRunning;
    Subject<Unit> completeSubject = new Subject<Unit>();
    FloatReactiveProperty remainTimer = new FloatReactiveProperty();

    #endregion

    #region Property

    public float TimerSetting => timerSetting;
    public IReadOnlyReactiveProperty<float> RemainTimer => remainTimer;
    public IObservable<Unit> OnComplete => completeSubject.Share();
    public bool IsTimerRunning => isTimerRunning;
    public float ProgressRatio => (TimerSetting - remainTimer.Value) / TimerSetting;

    #endregion

    #region Constructor

    public TimerModule()
    {
        Observable.EveryUpdate()
            .Where(_ => TimerManager.In.IsTimerRunning)
            .Where(_ => isTimerRunning)
            .Select(_ => TimerManager.In.Timer.Value)
            .Subscribe(timer =>
            {
                remainTimer.Value = timerSetting - (timer - startTime);
                if (remainTimer.Value <= 0)
                {
                    isTimerRunning = false;
                    completeSubject.OnNext(default);
                }
            }).AddTo(TimerManager.In);
    }

    #endregion

    #region Public Methord

    public void StartTimer(float timerSetting)
    {
        startTime = TimerManager.In.Timer.Value;
        this.timerSetting = timerSetting;
        isTimerRunning = true;
    }

    public void ResetTimer()
    {
        isTimerRunning = false;
    }

    #endregion
}
#endif