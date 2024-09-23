#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && R3_SUPPORT
using UnityEngine;

namespace JHS.Library.UINavigator.Runtime.Sheet
{
    public abstract class Sheet : UIContext
    {
        [field: SerializeField] public bool IsRecycle { get; private set; } = true;
    }
}

#endif