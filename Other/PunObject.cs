#if PUN_2_SUPPORT
using Photon.Pun;
using System.Collections.Generic;

namespace MoraeGames.Library.Other
{
    public class PunObject<T> : MonoBehaviourPun, IPunInstantiateMagicCallback where T : PunObject<T>
    {
        #region Field

        protected static Dictionary<int, T> punObjects = new Dictionary<int, T>();

        #endregion

        #region Unity Lifecycle

        void OnDestroy() => punObjects.Remove(photonView.ViewID);

        #endregion

        #region ������ �޼ҵ�

        public virtual void OnPhotonInstantiate(PhotonMessageInfo info) => punObjects.Add(photonView.ViewID, (T)this);

        #endregion

        #region �ܺ� �޼ҵ�

        public static T Get(int viewID)
        {
            punObjects.TryGetValue(viewID, out var punObject);
            return punObject;
        }

        #endregion
    }
}

#endif