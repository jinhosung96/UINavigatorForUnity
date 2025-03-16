#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && (R3_SUPPORT || UNIRX_SUPPORT)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using JHS.Library.UINavigator.Runtime.Animation;
using JHS.Library.UINavigator.Runtime.Page;
#if R3_SUPPORT
using R3;
using R3.Triggers; 
#endif
#if UNIRX_SUPPORT
using UniRx;
using UniRx.Triggers; 
#endif
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable HeapView.ObjectAllocation

namespace JHS.Library.UINavigator.Runtime
{
    public enum AnimationSetting
    {
        Container,
        Custom
    }

    // UI View의 애니메이션 상태
    public enum VisibleState
    {
        Appearing, // 등장 중
        Appeared, // 등장 완료
        Disappearing, // 사라지는 중
        Disappeared // 사라짐
    }

    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
#if VCONTAINER_SUPPORT
    public abstract class UIContext : VContainer.Unity.LifetimeScope
#else
    public abstract class UIContext : MonoBehaviour
#endif
    {
        #region Fields

        [SerializeField] AnimationSetting animationSetting = AnimationSetting.Container;
        [SerializeField] ViewShowAnimation showAnimation = new();
        [SerializeField] ViewHideAnimation hideAnimation = new();

        CanvasGroup canvasGroup;

        readonly Subject<Unit> preInitializeEvent = new();
        readonly Subject<Unit> postInitializeEvent = new();
        readonly Subject<Unit> appearEvent = new();
        readonly Subject<Unit> appearedEvent = new();
        readonly Subject<Unit> disappearEvent = new();
        readonly Subject<Unit> disappearedEvent = new();

        #endregion

        #region Properties

        static List<UIContext> ActiveUIList { get; } = new();
        float LastShowTime { get; set; }

        public static UIContext FocusUI
        {
            get
            {
                var activeViews = ActiveUIList
                    .Where(view => view.gameObject.activeInHierarchy)
                    .Where(view => view is not Sheet.Sheet)
                    .Where(view =>
                    {
                        if (view is Page.Page { UIContainer: PageContainer { HasDefault: true } pageContainer } page) return pageContainer.DefaultPage != page;
                        return true;
                    });

                if (activeViews.Any()) return activeViews.Aggregate((prev, current) => prev.LastShowTime > current.LastShowTime ? prev : current);

                return null;
            }
        }

        public UIContainer UIContainer { get; set; }
        public CanvasGroup CanvasGroup => canvasGroup ? canvasGroup : canvasGroup = GetComponent<CanvasGroup>();
        public VisibleState VisibleState { get; private set; } = VisibleState.Disappeared;

        /// <summary>
        /// Awake보다 먼저 호출되는 이벤트
        /// </summary>
#if R3_SUPPORT
        public Observable<Unit> OnPreInitialize => preInitializeEvent.Share();
#elif UNIRX_SUPPORT
        public IObservable<Unit> OnPreInitialize => preInitializeEvent.Share();
#endif
        
        /// <summary>
        /// Awake보다 직후 호출되는 이벤트
        /// </summary>
#if R3_SUPPORT
        public Observable<Unit> OnPostInitialize => postInitializeEvent.Share();
#elif UNIRX_SUPPORT
        public IObservable<Unit> OnPostInitialize => postInitializeEvent.Share();
#endif
        
        /// <summary>
        /// UI View가 활성화를 시작할 때 발생하는 이벤트
        /// </summary>
#if R3_SUPPORT
        public Observable<Unit> OnAppear => appearEvent.Share();
#elif UNIRX_SUPPORT
        public IObservable<Unit> OnAppear => appearEvent.Share();
#endif

        /// <summary>
        /// UI View가 활성화 애니메이션이 진행 중일 때 매 프레임 발생하는 이벤트
        /// </summary>
#if R3_SUPPORT
        public Observable<Unit> OnAppearing => OnChangingVisibleState(OnAppear, OnAppeared);
#elif UNIRX_SUPPORT
        public IObservable<Unit> OnAppearing => OnChangingVisibleState(OnAppear, OnAppeared);
#endif

        /// <summary>
        /// UI View가 활성화가 완전히 끝났을 때 발생하는 이벤트
        /// </summary>
#if R3_SUPPORT
        public Observable<Unit> OnAppeared => appearedEvent.Share();
#elif UNIRX_SUPPORT
        public IObservable<Unit> OnAppeared => appearedEvent.Share();
#endif

        /// <summary>
        /// UI View가 활성화 되어 있는동안 매 프레임 발생하는 이벤트
        /// </summary>
#if R3_SUPPORT
        public Observable<Unit> OnUpdate => OnChangingVisibleState(OnAppeared, OnDisappear);
#elif UNIRX_SUPPORT
        public IObservable<Unit> OnUpdate => OnChangingVisibleState(OnAppeared, OnDisappear);
#endif

        /// <summary>
        /// UI View가 비활성화를 시작할 때 발생하는 이벤트
        /// </summary>
#if R3_SUPPORT
        public Observable<Unit> OnDisappear => disappearEvent.Share();
#elif UNIRX_SUPPORT
        public IObservable<Unit> OnDisappear => disappearEvent.Share();
#endif

        /// <summary>
        /// UI View가 비활성화 애니메이션이 진행 중일 때 매 프레임 발생하는 이벤트
        /// </summary>
#if R3_SUPPORT
        public Observable<Unit> OnDisappearing => OnChangingVisibleState(OnDisappear, OnDisappeared);
#elif UNIRX_SUPPORT
        public IObservable<Unit> OnDisappearing => OnChangingVisibleState(OnDisappear, OnDisappeared);
#endif

        /// <summary>
        /// UI View가 비활성화가 완전히 끝났을 때 발생하는 이벤트
        /// </summary>
#if R3_SUPPORT
        public Observable<Unit> OnDisappeared => disappearedEvent.Share();
#elif UNIRX_SUPPORT
        public IObservable<Unit> OnDisappeared => disappearedEvent.Share();
#endif

        #endregion

        #region Unity Lifecycle

#if VCONTAINER_SUPPORT
        protected override void OnDestroy()
#else
        protected void OnDestroy()
#endif
        {
            preInitializeEvent.Dispose();
            postInitializeEvent.Dispose();
            appearEvent.Dispose();
            appearedEvent.Dispose();
            disappearEvent.Dispose();
            disappearedEvent.Dispose();

            ActiveUIList.Remove(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// UI View를 활성화할 때 실행되는 로직이다. <br/>
        /// UI View를 활성화하기 전에 앵커와 피봇을 Stretch Stretch로 설정하고 알파를 1로 초기화한다. <br/>
        /// 이 과정에서 Unity UI 시스템의 한계 때문에 1 프레임이 소비된다. <br/>
        /// 그리고 UI View가 활성화 되기 전 후에 이벤트를 발생시키며 현재 VisibleState를 갱신한다. <br/>
        /// <br/>
        /// 나머지는 템플릿 메소드 패턴으로 구성되어 있으며 하위 객체에서 구체적인 로직을 정의하게 되어있다. <br/>
        /// <br/>
        /// 전체 로직의 흐름은 다음과 같다. <br/>
        /// RectTransform 및 CanvasGroup 초기화 후 게임 오브젝트 활성화 -> Appearing State로 변경 -> AppearEvent 송신 -> 전 처리 로직 대기 -> Show 애니메이션 진행 -> Appeared State로 변경 -> AppearedEvent 송신 <br/>
        /// <br/>
        /// 주의 사항 - 클라이언트는 사용할 필요가 없는 API, 추 후 외부에 노출시키지 않을 방법을 찾아볼 것
        /// </summary>
        /// <param name="useAnimation"> 애니메이션 사용 여부, 인스펙터 상에서 경정해주는 isUseAnimation이랑 둘 다 true일 경우에만 애니메이션을 실행한다. </param>
        internal async UniTask ShowAsync(bool useAnimation = true)
        {
            LastShowTime = Time.time;
            ActiveUIList.Add(this);

            var rectTransform = (RectTransform)transform;
            await InitializeRectTransformAsync(rectTransform);
            CanvasGroup.alpha = 1;
            
            preInitializeEvent.OnNext(Unit.Default);
            await WhenPreAppearAsync();
            gameObject.SetActive(true);
            postInitializeEvent.OnNext(Unit.Default);

            VisibleState = VisibleState.Appearing;
            appearEvent.OnNext(Unit.Default);

            if (useAnimation)
            {
                if (animationSetting == AnimationSetting.Custom) await showAnimation.AnimateAsync(rectTransform, CanvasGroup);
                else await UIContainer.ShowAnimation.AnimateAsync(transform, CanvasGroup);
            }
            await WhenPostAppearAsync();

            VisibleState = VisibleState.Appeared;
            appearedEvent.OnNext(Unit.Default);
        }

        /// <summary>
        /// UI View를 비활성화할 때 실행되는 로직이다. <br/>
        /// 그리고 UI View가 비활성화 되기 전 후에 이벤트를 발생시키며 현재 VisibleState를 갱신한다. <br/>
        /// <br/>
        /// 나머지는 템플릿 메소드 패턴으로 구성되어 있으며 하위 객체에서 구체적인 로직을 정의하게 되어있다. <br/>
        /// <br/>
        /// 전체 로직의 흐름은 다음과 같다. <br/>
        /// Disappearing State로 변경 -> DisappearEvent 송신 -> Hide 애니메이션 진행 -> 후 처리 로직 대기 -> Disappeared State로 변경 -> DisappearedEvent 송신 <br/>
        /// <br/>
        /// 주의 사항 - 클라이언트는 사용할 필요가 없는 API, 추 후 외부에 노출시키지 않을 방법을 찾아볼 것
        /// </summary>
        /// <param name="useAnimation"> 애니메이션 사용 여부, 인스펙터 상에서 경정해주는 isUseAnimation이랑 둘 다 true일 경우에만 애니메이션을 실행한다. </param>
        internal async UniTask HideAsync(bool useAnimation = true)
        {
            ActiveUIList.Remove(this);

            VisibleState = VisibleState.Disappearing;
            disappearEvent.OnNext(Unit.Default);

            await UniTask.Yield(cancellationToken: this.GetCancellationTokenOnDestroy());
            
            await WhenPreDisappearAsync();

            if (useAnimation)
            {
                if (animationSetting == AnimationSetting.Custom) await hideAnimation.AnimateAsync(transform, CanvasGroup);
                else await UIContainer.HideAnimation.AnimateAsync(transform, CanvasGroup);
            }

            gameObject.SetActive(false);
            await WhenPostDisappearAsync();
            
            VisibleState = VisibleState.Disappeared;
            disappearedEvent.OnNext(Unit.Default);
        }

        #endregion

        #region Virtual Methods

        protected virtual UniTask WhenPreAppearAsync() => UniTask.CompletedTask;
        protected virtual UniTask WhenPostAppearAsync() => UniTask.CompletedTask;

        protected virtual UniTask WhenPreDisappearAsync() => UniTask.CompletedTask;
        protected virtual UniTask WhenPostDisappearAsync() => UniTask.CompletedTask;

        #endregion

        #region Private Methods

#if R3_SUPPORT
        Observable<Unit> OnChangingVisibleState(Observable<Unit> begin, Observable<Unit> end) => Stream(this.UpdateAsObservable(), begin, end, gameObject.GetCancellationTokenOnDestroy()).Share();
#elif UNIRX_SUPPORT
        IObservable<Unit> OnChangingVisibleState(IObservable<Unit> begin, IObservable<Unit> end) => this.UpdateAsObservable().SkipUntil(begin).TakeUntil(end).RepeatUntilDestroy(gameObject).Share();
#endif

        async UniTask InitializeRectTransformAsync(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            await UniTask.Yield();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
        }

#if R3_SUPPORT
        static Observable<T> Stream<T, T1, T2>(Observable<T> source, Observable<T1> beginStream, Observable<T2> endStream, CancellationToken ct)
        {
            Subject<T> subject = new Subject<T>();
            Stream(source, beginStream, endStream, subject, ct).Forget();
            return subject.TakeUntil(ct).Share();
        }

        static async UniTask Stream<T, T1, T2>(Observable<T> source, Observable<T1> beginStream, Observable<T2> endStream, Subject<T> subject, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested) 
                await source.SkipUntil(beginStream).TakeUntil(endStream).ForEachAsync(subject.OnNext, ct);
        }
#endif

        #endregion
    }
}
#endif