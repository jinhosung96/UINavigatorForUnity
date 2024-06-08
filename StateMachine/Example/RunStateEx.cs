#if UNIRX_SUPPORT

using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Library.StateMachine.Example
{
    public class RunStateEx : State
    {
        #region variable

        [Header("Cache")]

        [SerializeField]
        private Animator animator = null;

        [SerializeField]
        private Text tutorialText = null;

        #endregion

        #region event

        private void Reset()
        {
            animator = transform.GetComponent<Animator>();
        }

        private void Start()
        {
            // 러닝 애니메이션 재생
            OnBeginStream.Subscribe(_ => animator.Play("Run"));

            // 튜토리얼 문구 변경
            OnBeginStream.Subscribe(_ => tutorialText.text = "한 번 더 누르면 원래 스테이트로 돌아갑니다.");

            // 좌클릭시 이번엔 런이 아닌 지정 스테이트로 돌아간다.
            OnUpdateStream.Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ => Transition<IdleStateEx>());
        }

        #endregion
    }
} 

#endif