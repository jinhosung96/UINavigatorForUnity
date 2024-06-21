using System.Collections.Generic;
using UnityEngine;

namespace MoraeGames.Library.Manager.Coroutine
{
    public class CoroutineManager
    {
        #region Field

        Dictionary<float, WaitForSeconds> waitForSeconds = new();

        #endregion

        #region Public Methods

        public WaitForSeconds GetWaitForSeconds(float seconds)
        {
            if (waitForSeconds.TryGetValue(seconds, out var value)) return value;
            return waitForSeconds[seconds] = new WaitForSeconds(seconds);
        }

        #endregion
    } 
}
