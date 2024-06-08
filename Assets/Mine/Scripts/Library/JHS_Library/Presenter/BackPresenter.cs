#if UNIRX_SUPPORT && UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
using JHS.Library.Manager.UINavigator.Runtime;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace JHS.Library.Presenter
{
    public class BackPresenter : IStartable
    {
        void IStartable.Start() => Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape)).Subscribe(_ => UIContainer.BackAsync().Forget());
    }
}

#endif