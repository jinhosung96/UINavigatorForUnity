#if VCONTAINER_SUPPORT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MoraeGames.Library.Template
{
    public class VObject<T> where T : LifetimeScope
    {
        #region Properties

        [Inject] protected LifetimeScope context { private get; set; }

        protected T Context => context as T;

        #endregion
    }
}

#endif