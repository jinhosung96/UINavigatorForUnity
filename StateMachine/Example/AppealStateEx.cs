#if UNIRX_SUPPORT

using Mine.Code.Framework.Extension;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace Library.StateMachine.Example
{
    [DisallowMultipleComponent]
    public class AppealStateEx : State
    {
        #region variable

        [Header("Cache")]

        [SerializeField] private Animator animator = null;

        [SerializeField] private Text tutorialText = null;

        #endregion

        #region event

        private void Reset()
        {
            animator = transform.GetComponent<Animator>();
        }

        private void Start()
        {
            // 어필 애니메이션 재생
            OnBeginStream.Subscribe(_ => animator.Play("Appeal"));

            // 튜토리얼 문구 변경
            OnBeginStream.Subscribe(_ => tutorialText.text =
                (int)IdleStateEx.TransitionToAppealDuration + "초가 경과했기 때문에 Apple State로 전이되었습니다.");

            // 어필 애니메이션 재생되면 초기 스테이트로 되돌린다.
            OnUpdateStream.Where(_ => animator.IsCompleted(Animator.StringToHash("Appeal")))
                .Subscribe(_ => Transition<IdleStateEx>());

            // 좌클릭시 지정 스테이트 재생
            OnUpdateStream.Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ => Transition<RunStateEx>());
        }

        #endregion
    }
} 

#endif