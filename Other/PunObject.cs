#if PUN_2_SUPPORT

using Photon.Pun;
using System.Collections.Generic;

/// <summary>
/// 상속 받는 객체는 View ID를 통해 객체를 구할 수 있게 된다.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PunObject<T> : MonoBehaviourPun, IPunInstantiateMagicCallback where T : PunObject<T>
{
    #region Field

    protected static Dictionary<int, T> punObjects = new Dictionary<int, T>();

    #endregion

    #region Unity Lifecycle

    void OnDestroy() => punObjects.Remove(photonView.ViewID);

    #endregion

    #region 재정의 메소드

    public virtual void OnPhotonInstantiate(PhotonMessageInfo info) => punObjects.Add(photonView.ViewID, (T)this);

    #endregion

    #region 외부 메소드

    public static T Get(int viewID)
    {
        punObjects.TryGetValue(viewID, out var punObject);
        return punObject;
    }

    #endregion
}

#endif