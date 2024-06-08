#if UNIRX_SUPPORT

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace JHS.Library.StateMachine
{
    /// <summary>
    /// State Machine 애니메이션 동작에 있어 활용하면 좋을 것 같다.
    /// </summary>
    public class StateMachine : MonoBehaviour
    {
        #region variable

        //FSM : Finite State Machine
        [SerializeField, Tooltip("Start 함수에 의해 자동시작 됨")]
        public bool autoStart = true;

        //Current State
        [SerializeField, Tooltip("초기 스테이트 여기에 설정")]
        State initState;
        State currentState;

        #endregion

        #region property

        public Dictionary<Type, State> StateMap { get; } = new();
        public State CurrentState => currentState;
        public bool IsPlay { get; set; }

        #endregion

        #region unity events

        protected void Awake()
        {
            foreach (var state in GetComponents<State>())
            {
                Register(state);
            }
        }

        protected async void Start()
        {
            await UniTask.NextFrame();
            if (autoStart) StartFsm();
        }

        #endregion

        #region method

        /// <summary>
        /// 스테이트 머신 개시
        /// </summary>
        public void StartFsm()
        {
            if (initState == null)
                Debug.LogError("Please assign current state via Hierachy or call setfirststate");

            IsPlay = true;

            currentState = initState;
            currentState.StateBegin();
        }

        /// <summary>
        /// 스테이트 머신 종료
        /// </summary>
        public void FinishFsm()
        {
            currentState.StateEnd();
            currentState = null;
            IsPlay = false;
        }

        /// <summary>
        /// 초기 스테이트 등록
        /// </summary>
        /// <param name="initState">초기에 설정될 스테이트</param>
        public void SetFirstState(State initState)
        {
            this.initState = initState;
        }

        /// <summary>
        /// 스테이트 등록
        /// </summary>
        /// <param name="state"></param>
        public void Register(State state)
        {
            StateMap.Add(state.GetType(), state);
            state.Machine = this;
        }

        public void Transition<T>() where T : State
        { 
            //현재 스테이트 종료
            currentState.StateEnd();

            //다음 스테이트 개시
            currentState = StateMap[typeof(T)];
            currentState.StateBegin();
        }

        #endregion
    }
} 

#endif