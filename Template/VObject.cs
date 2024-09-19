#if VCONTAINER_SUPPORT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MoraeGames.Library.Template
{
    public class VObject<TScope> where TScope : LifetimeScope
    {
        #region Properties

        [Inject] protected LifetimeScope context { private get; set; }

        protected TScope Context => context as TScope;

        #endregion
    }
}

#endif