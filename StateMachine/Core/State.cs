#if UNIRX_SUPPORT

using System;
using JHS.Library.Extension;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace JHS.Library.StateMachine
{
    //각 State를 상속받아서 각종 State를 정의한다.
    //순서는 Begin - Update - LateUpdate - End 스트림
    //타 스테이트로 전이는 transition함수를 이용
    [RequireComponent(typeof(StateMachine))]
    public abstract class State : MonoBehaviour
    {
        #region variable
        //스테이트 머신
        public StateMachine Machine { get; set; }
        //스테이트 개시
        private Subject<Unit> _begin = new Subject<Unit>();
        //스테이트 종료
        private Subject<Unit> _end = new Subject<Unit>();

        [SerializeField] StateMachine[] subMachines;

        #endregion

        #region EventStream

        public IObservable<Unit> OnBeginStream => _begin.Share();

        public IObservable<Unit> OnEndStream => _end.Share();

        public IObservable<Unit> OnUpdateStream => StateStream(this.UpdateAsObservable());

        public IObservable<Unit> OnFixedUpdateStream => StateStream(this.FixedUpdateAsObservable());

        public IObservable<Unit> OnLateUpdateStream => StateStream(this.LateUpdateAsObservable());

        public IObservable<Unit> OnDrawGizmosStream => StateStream(this.OnDrawGizmosAsObservable());

        public IObservable<Unit> OnGUIStream => StateStream(this.OnGUIAsObservable());

        protected IObservable<T> StateStream<T>(IObservable<T> source)
        {
            return source
                //begin스트림에서 OnNext가 실행되면서
                .SkipUntil(OnBeginStream)
                //End스트림에서OnNext가 실행되기전까지
                .TakeUntil(OnEndStream)
                .RepeatUntilDestroy(gameObject)
                .Share(); //옵저버 추가시 자동으로 connect
        }

        #endregion

        #region Methods

        /// <summary>
        /// State의 시작을 알림 (State머신 외에는 호출하지 않음)
        /// </summary>
        public void StateBegin()
        {
            for (int i = 0; i < subMachines.Length; i++)
            {
                subMachines[i].StartFsm();
            }
            _begin.OnNext(default(Unit));
        }

        /// <summary>
        /// State의 종료 알림
        /// </summary>
        public void StateEnd()
        {
            for (int i = 0; i < subMachines.Length; i++)
            {
                subMachines[i].FinishFsm();
            }
            _end.OnNext(default(Unit));
        }

        /// <summary>
        /// 스테이트의 전이 예약
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected void Transition<T>() where T : State
        {
            Machine.Transition<T>();
        }
        #endregion
    }
} 

#endif