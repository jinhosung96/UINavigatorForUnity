#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && (R3_SUPPORT || UNIRX_SUPPORT)
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
#if R3_SUPPORT
using R3;
#elif UNIRX_SUPPORT
using UniRx;
#endif
using UnityEngine;
#if ADDRESSABLE_SUPPORT
using UnityEngine.AddressableAssets;
#endif
using Debug = JHS.Library.UINavigator.Runtime.Util.Debug;

namespace JHS.Library.UINavigator.Runtime.Page
{
    public sealed class PageContainer : UIContainer<PageContainer>, IHasHistory, ISerializationCallbackReceiver
    {
        #region Properties

        [field: SerializeField] public List<Page> RegisterPagesByPrefab { get; private set; } = new(); // 해당 Container에서 생성할 수 있는 Page들에 대한 목록
#if ADDRESSABLE_SUPPORT
        [field: SerializeField] public List<ComponentReference<Page>> RegisterPagesByAddressable {get; private set; } = new(); // 해당 Container에서 생성할 수 있는 Page들에 대한 목록 
#endif
        [field: SerializeField] public bool HasDefault { get; private set; } // 시작할 때 초기 시트 활성화 여부
        internal Page DefaultPage => History.FirstOrDefault();

        Dictionary<Type, Page> Pages { get; set; }

        /// <summary>
        /// Page UI View들의 History 목록이다. <br/>
        /// History는 각 Container에서 관리된다. <br/>
        /// </summary>
        Stack<Page> History { get; } = new();

        public Page CurrentView => History.TryPeek(out var currentView) ? currentView : null;
        bool IsRemainHistory => HasDefault ? History.Count > 1 : History.Count > 0;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

#if ADDRESSABLE_SUPPORT
            if(InstantiateType == InstantiateType.InstantiateByAddressable) RegisterPagesByPrefab = RegisterPagesByAddressable.Select(x => x.LoadAssetAsync<GameObject>().WaitForCompletion().GetComponent<Page>()).ToList();
#endif

            // pages에 등록된 모든 Page들을 Type을 키값으로 한 Dictionary 형태로 등록
            RegisterPagesByPrefab = RegisterPagesByPrefab.GroupBy(x => x.GetType()).Select(x => x.First()).Select(x => x.IsRecycle ? Instantiate(x, transform) : x).ToList();
            Pages = RegisterPagesByPrefab.ToDictionary(page => page.GetType(), page => page);

            foreach (var x in RegisterPagesByPrefab.Where(x => x.IsRecycle))
            {
                x.UIContainer = this;
                x.gameObject.SetActive(false);
            }

            if (HasDefault && RegisterPagesByPrefab.Any()) NextAsync(RegisterPagesByPrefab.First().GetType(), null, null, false).Forget();
        }

        void OnEnable() => ResetAsync(false).Forget();

        #endregion

        #region Public Methods

        /// <summary>
        /// 지정한 하위 Page를 활성화하고 History에 담는다. <br/>
        /// Page를 지정해주는 방법은 제네릭 타입으로 원하는 Page의 타입을 넘기는 것으로 이루어진다. <br/>
        /// <br/>
        /// 현재 활성화된 Page가 있다면, 이전 Page를 비활성화하고 새로운 Page를 활성화하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Page가 전환 중인 상태일 때는 실행되지 않는다. <br/>
        /// </summary>
        /// <typeparam name="T"> 활성화 시킬 Page의 Type </typeparam>
        /// <returns></returns>
        public async UniTask<T> NextAsync<T>(
            Action<T> onPreInitialize = null,
            Action<T> onPostInitialize = null,
            bool useAnimation = true) where T : Page
        {
            if (Pages.TryGetValue(typeof(T), out var page))
                return await NextAsync(page as T, onPreInitialize, onPostInitialize, useAnimation);

            Debug.LogError($"Page not found : {typeof(T)}");
            return null;
        }

        /// <summary>
        /// 지정한 하위 Page를 활성화하고 History에 담는다. <br/>
        /// Page를 지정해주는 방법은 제네릭 타입으로 원하는 Page의 타입을 넘기는 것으로 이루어진다. <br/>
        /// <br/>
        /// 현재 활성화된 Page가 있다면, 이전 Page를 비활성화하고 새로운 Page를 활성화하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Page가 전환 중인 상태일 때는 실행되지 않는다. <br/>
        /// </summary>
        /// <param name="nextPageName"> 활성화 시킬 Page의 클래스명 </param>
        /// <returns></returns>
        public async UniTask<Page> NextAsync(string nextPageName,
                                             Action<Page> onPreInitialize = null,
                                             Action<Page> onPostInitialize = null,
                                             bool useAnimation = true)
        {
            var page = Pages.Values.FirstOrDefault(x => x.GetType().Name == nextPageName);
            if (page != null) return await NextAsync(page, onPreInitialize, onPostInitialize, useAnimation);

            Debug.LogError($"Page not found : {nextPageName}");
            return null;
        }

        /// <summary>
        /// 특정 UI View를 종료하는 메소드이다. <br/>
        /// 해당 UI View가 종료되면 해당 View보다 Histroy상 뒤에 활성화된 View들도 모두 같이 종료된다. <br/>
        /// <br/>
        /// 해당 UI View의 History를 가지고 있는 부모 Sheet를 찾아 해당 History를 최상단부터 차근차근 비교하며 해당 View를 찾는다. <br/>
        /// 그리고 그 View들을 나중에 제거하기 위해 Queue에 담아둔다. <br/>
        /// 해당 View를 찾으면 해당 View를 제거하는데 이 때, 만약 해당 UI View가 Sheet라면, <br/>
        /// ResetOnPop 설정 여부에 따라 해당 Sheet의 부모 Container의 CurrentSheet를 null로 초기화하거나 InitialSheet를 CurrentSheet로 설정한다. <br/>
        /// <br/>
        /// 지정한 UI View가 종료되면 Queue에 담아둔 View들을 모두 종료하고 Modal은 추가로 Backdrop을 제거함과 동시에 Modal 또한 파괴한다. <br/>
        /// 이 때, PopRoutineAsync 메소드를 사용하지 않는 이유는 즉각적으로 제거해주기 위함과 더불어 Sheet의 경우 PopRoutineAsync가 정의되어있지 않기 때문이다. <br/>
        /// </summary>
        public async UniTask<Page> NextAsync(Type nextPageType,
                                             Action<Page> onPreInitialize = null,
                                             Action<Page> onPostInitialize = null,
                                             bool useAnimation = true)
        {
            if (Pages.TryGetValue(nextPageType, out var page))
                return await NextAsync(page, onPreInitialize, onPostInitialize, useAnimation);

            Debug.LogError($"Page not found : {nextPageType.Name}");
            return null;
        }

        public async UniTask PrevAsync(int count = 1, bool useAnimation = true)
        {
            count = Mathf.Clamp(count, 1, HasDefault ? History.Count - 1 : History.Count);
            if (!IsRemainHistory) return;

            if (CurrentView.VisibleState is VisibleState.Appearing or VisibleState.Disappearing) return;

            var prevView = CurrentView;
            var destroyTargetViews = new Queue<GameObject>();
            
            for (int i = 0; i < count; i++)
            {
                if (!CurrentView.IsRecycle) destroyTargetViews.Enqueue(CurrentView.gameObject);
                History.Pop();
            }
            
            prevView.HideAsync(useAnimation).ContinueWith(() =>
            {
                while (destroyTargetViews.Any()) Destroy(destroyTargetViews.Dequeue());
            }).Forget();

            if (!CurrentView) return;

            await CurrentView.ShowAsync(useAnimation);
        }

        public async UniTask ResetAsync(bool useAnimation = true) => await PrevAsync(History.Count, useAnimation);

        #endregion

        #region Private Methods

        async UniTask<T> NextAsync<T>(
            T nextView,
            Action<T> onPreInitialize,
            Action<T> onPostInitialize,
            bool useAnimation) where T : Page
        {
            if (CurrentView && CurrentView.VisibleState is VisibleState.Appearing or VisibleState.Disappearing) return null;
            if (CurrentView && CurrentView == nextView) return null;

            nextView.gameObject.SetActive(false);
            nextView = nextView.IsRecycle
                ? nextView
                :
#if VCONTAINER_SUPPORT
                VContainer.Unity.VContainerSettings.Instance.RootLifetimeScope.Container.Instantiate(nextView, transform);
#else
                Instantiate(nextView, transform);
#endif
            nextView.UIContainer = this;

            nextView.OnPreInitialize.Take(1).Subscribe(_ => onPreInitialize?.Invoke(nextView)).AddTo(nextView);
            nextView.OnPostInitialize.Take(1).Subscribe(_ => onPostInitialize?.Invoke(nextView)).AddTo(nextView);
            
            if (CurrentView)
            {
                var prevView = CurrentView;
                prevView.HideAsync().ContinueWith(() =>
                {
                    if (!prevView.IsRecycle) Destroy(prevView.gameObject);
                }).Forget();
            }

            History.Push(nextView);

            await CurrentView.ShowAsync(useAnimation);

            return CurrentView as T;
        }

        #endregion

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
#if ADDRESSABLE_SUPPORT
            if (InstantiateType == InstantiateType.InstantiateByAddressable)
                RegisterPagesByPrefab.Clear();
            else
                RegisterPagesByAddressable.Clear();
#endif
        }

        #endregion
    }
}
#endif