#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && (R3_SUPPORT || UNIRX_SUPPORT)
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace JHS.Library.UINavigator.Runtime.Animation
{
    public abstract class FadeShowAnimation
    {
        public abstract UniTask AnimateAsync(CanvasGroup canvasGroup);
    }
    
    public abstract class FadeHideAnimation
    {
        public abstract UniTask AnimateAsync(CanvasGroup canvasGroup);
    }
}
#endif