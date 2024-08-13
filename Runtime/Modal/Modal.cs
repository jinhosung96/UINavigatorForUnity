#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using JHS.Library.UINavigator.Runtime;
using UnityEngine;

namespace JHS.Library.UINavigator.Runtime.Modal
{
    public abstract class Modal : UIContext
    {
        #region Propertys

        public CanvasGroup BackDrop { get; internal set; }
        [field: SerializeField] public bool EnableBackDropButton { get; internal set; } = true;

        #endregion
    }
}
#endif