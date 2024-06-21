#if UNIRX_SUPPORT
using System;
using UniRx;
using UnityEngine;

namespace MoraeGames.Library.Other
{
    public class AnimationEventReceiver : MonoBehaviour
    {
        Animator animator;
        Subject<int> animationEventSubject = new();

        void Awake() => animator = GetComponent<Animator>();

        public void OnEvent(int eventIndex = 0) => animationEventSubject.OnNext(eventIndex);

        public IObservable<int> GetEvent(string animationName, int eventIndex = 0) =>
            animationEventSubject
                .Where(_ => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
                .Where(x => x == eventIndex)
                .TakeUntilDestroy(gameObject)
                .Share();
    }
}

#endif