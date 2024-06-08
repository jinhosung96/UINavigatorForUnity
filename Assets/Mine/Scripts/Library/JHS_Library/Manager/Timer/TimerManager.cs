#if UNIRX_SUPPORT && PUN_2_SUPPORT

using Library.Singleton;
using Library.Util;
using Photon.Pun;
using UniRx;
using UnityEngine;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

/// <summary>
/// PUN2���� ����ȭ �Ǵ� Ÿ�̸ӿ� ���� ��� ����
/// </summary>
public class TimerManager
{
    #region Field

    public const string KEY_StartTime = "StartTime";
    public const string KEY_IsTimerRunning = "IsTimerRunning";
    bool isTimerRunning = false;
    FloatReactiveProperty timer = new();
    int startTime;

    #endregion

    #region Property

    public IReadOnlyReactiveProperty<float> Timer => timer;

    public int MillisecondTimer
    {
        get
        {
            int millisecondTimer = (PhotonNetwork.ServerTimestamp - this.startTime);
            return millisecondTimer;
        }
    }

    public float SecondTimer
    {
        get
        {
            float secondTimer = MillisecondTimer / 1000f;
            return secondTimer;
        }
    }

    public bool IsTimerRunning => isTimerRunning;

    #endregion

    #region Unity Lifecycle

    protected override void Awake()
    {
        base.Awake();
        // RoomProperties�� ���ŵǸ� Ÿ�̸Ӱ� ���۵ȴ�.
        PhotonManager.In.OnRoomPropertiesUpdateAsObservable
            .Subscribe(x =>
            {
                Debug.Log("CountdownTimer.OnRoomPropertiesUpdate " + x.ToStringFull());
                Initialize();
            })
            .AddTo(gameObject);

        Observable.EveryUpdate()
            .Where(_ => isTimerRunning)
            .Select(_ => SecondTimer)
            .DistinctUntilChanged()
            .Subscribe(currentTime =>
            {
                timer.Value = currentTime;
            })
            .AddTo(gameObject);
    }

    #endregion

    #region �ܺ� �޼ҵ�

    /// <summary>
    /// Ÿ�̸� �ʱ�ȭ
    /// </summary>
    public void InitTimer()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Hashtable props = new Hashtable
        {
            {KEY_IsTimerRunning, true },
            {KEY_StartTime, PhotonNetwork.ServerTimestamp}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        Util.Log("Ÿ�̸� ����");
    }

    /// <summary>
    /// Ÿ�̸� ����
    /// </summary>
    public void ResetTimer()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Hashtable props = new Hashtable
        {
            {KEY_IsTimerRunning, false },
            {KEY_StartTime, PhotonNetwork.ServerTimestamp}
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        Util.Log("Ÿ�̸� ����");
    }

    #endregion

    #region ���� �޼ҵ�

    /// <summary>
    /// Ÿ�̸� ����
    /// </summary>
    void Initialize()
    {
        object propStartTime;
        object propIsTimerRunning;

        bool tryGetIsTimerRunning = PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(KEY_IsTimerRunning, out propIsTimerRunning);
        bool tryGetStartTime = PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(KEY_StartTime, out propStartTime);

        if (tryGetIsTimerRunning && tryGetStartTime)
        {
            startTime = (int)propStartTime;
            isTimerRunning = (bool)propIsTimerRunning;
        }
        else Util.Log("Ÿ�̸� �ʱ�ȭ ����", Util.LogLevel.Error);
    }

    #endregion
}


#endif