#if  UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && R3_SUPPORT
using Cysharp.Threading.Tasks;

namespace JHS.Library.UINavigator.Runtime
{
    public interface IHasHistory
    {
        UniTask PrevAsync(int count = 1, bool useAnimation = true);
    }
}

#endif