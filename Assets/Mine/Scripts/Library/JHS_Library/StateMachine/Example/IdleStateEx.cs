#if UNIRX_SUPPORT

using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace Library.StateMachine.Example
{
    [DisallowMultipleComponent]
    public class IdleStateEx : State
    {
        #region variable

        [Header("Parameter")]
        public const float TransitionToAppealDuration = 3f;

        [Header("Cache")]

        [SerializeField] private Animator animator = null;

        [SerializeField] private Text tutorialText = null;

        #endregion

        #region unity event

        private void Reset()
        {
            animator = transform.GetComponent<Animator>();
        }

        private void Start()
        {
            //등록된 애니메이션 재생
            OnBeginStream.Subscribe(_ => animator.Play("Idle"));

            //튜토리얼 텍스트 갱신
            OnBeginStream.Subscribe(_ => tutorialText.text = "튜토리얼 머시기머시기");

            float counter = 0f;

            //n초가 지난뒤 지정된 스테이트로 전이
            OnUpdateStream.Do(_ => counter += Time.deltaTime)
                .Where(count => counter > TransitionToAppealDuration)
                .Subscribe(_ => Transition<AppealStateEx>());

            //좌클릭하면 달리는 지정된 스테이트로 전이
            OnUpdateStream.Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ => Transition<RunStateEx>());

            //스테이트가 종료되면 데이터 리셋
            OnEndStream.Subscribe(_ => counter = 0f);
        }

        #endregion
    }
} 

#endif