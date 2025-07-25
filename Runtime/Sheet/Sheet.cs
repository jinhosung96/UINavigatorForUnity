#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && (R3_SUPPORT || UNIRX_SUPPORT)
using UnityEngine;

namespace JHS.Library.UINavigator.Runtime.Sheet
{
    public abstract class Sheet : UIView
    {
        [field: SerializeField] public bool IsRecycle { get; private set; } = true;
    }
}

#endif