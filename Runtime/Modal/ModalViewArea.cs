#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && (R3_SUPPORT || UNIRX_SUPPORT)
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JHS.Library.UINavigator.Runtime.Util;
#if R3_SUPPORT
using R3;
#elif UNIRX_SUPPORT
using UniRx;
#endif
using UnityEngine;
#if ADDRESSABLE_SUPPORT
using UnityEngine.AddressableAssets;
#endif
using UnityEngine.UI;
using VContainer.Unity;
using Debug = JHS.Library.UINavigator.Runtime.Util.Debug;

namespace JHS.Library.UINavigator.Runtime.Modal
{
    public sealed class ModalViewArea : UIViewArea<ModalViewArea>, IHasHistory, ISerializationCallbackReceiver
    {
        #region Fields

        [SerializeField] Backdrop modalBackdrop; // 생성된 모달의 뒤에 배치될 레이어

        #endregion

        #region Properties

        [field: SerializeField] public List<Modal> RegisterModalsByPrefab { get; private set; } = new(); // 해당 Container에서 생성할 수 있는 Modal들에 대한 목록
#if ADDRESSABLE_SUPPORT
        [field: SerializeField] public List<ComponentReference<Modal>> RegisterModalsByAddressable { get; private set; } = new(); // 해당 Container에서 생성할 수 있는 Modal들에 대한 목록 
#endif
        Dictionary<Type, Modal> Modals { get; set; }

        /// <summary>
        /// Page UI View들의 History 목록이다. <br/>
        /// History는 각 Container에서 관리된다. <br/>
        /// </summary>
        Stack<Modal> History { get; } = new();

        public Modal CurrentView => History.TryPeek(out var currentView) ? currentView : null;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

#if ADDRESSABLE_SUPPORT
            if(InstantiateType == InstantiateType.InstantiateByAddressable) RegisterModalsByPrefab = RegisterModalsByAddressable.Select(x => x.LoadAssetAsync<GameObject>().WaitForCompletion().GetComponent<Modal>()).ToList();
#endif

            // modals에 등록된 모든 Modal들을 Type을 키값으로 한 Dictionary 형태로 등록
            Modals = RegisterModalsByPrefab.GroupBy(x => x.GetType()).Select(x => x.FirstOrDefault()).ToDictionary(modal => modal.GetType(), modal => modal);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 지정한 하위 Modal를 생성하고 History에 담는다. <br/>
        /// Modal를 지정해주는 방법은 제네릭 타입으로 원하는 Modal의 타입을 넘기는 것으로 이루어진다. <br/>
        /// <br/>
        /// 기존에 생성된 Modal은 그대로 둔 채 새로운 Modal을 생성하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Modal이 생성 중인 상태일 때는 실행되지 않는다. <br/>
        /// </summary>
        /// <typeparam name="T"> 생성할 Modal의 Type </typeparam>
        /// <returns></returns>
        public async UniTask<T> NextAsync<T>(
            Action<T> onPreInitialize = null,
            Action<T> onPostInitialize = null,
            bool useAnimation = true) where T : Modal
        {
            if (Modals.TryGetValue(typeof(T), out var modal))
                return await NextAsync(modal as T, onPreInitialize, onPostInitialize);

            Debug.LogError($"Modal not found : {typeof(T)}");
            return null;
        }

        /// <summary>
        /// 지정한 하위 Modal를 생성하고 History에 담는다. <br/>
        /// Modal를 지정해주는 방법은 제네릭 타입으로 원하는 Modal의 타입을 넘기는 것으로 이루어진다. <br/>
        /// <br/>
        /// 기존에 생성된 Modal은 그대로 둔 채 새로운 Modal을 생성하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Modal이 생성 중인 상태일 때는 실행되지 않는다. <br/>
        /// </summary>
        /// <param name="nextModalName"> 생성할 Modal의 클래스명 </param>
        /// <returns></returns>
        public async UniTask<Modal> NextAsync(
            string nextModalName,
            Action<Modal> onPreInitialize = null,
            Action<Modal> onPostInitialize = null,
            bool useAnimation = true)
        {
            var modal = Modals.Values.FirstOrDefault(x => x.GetType().Name == nextModalName);
            if (modal != null) return await NextAsync(modal, onPreInitialize, onPostInitialize, useAnimation);

            Debug.LogError($"Modal not found : {nextModalName}");
            return null;
        }

        public async UniTask<Modal> NextAsync(
            Type nextModalType,
            Action<Modal> onPreInitialize = null,
            Action<Modal> onPostInitialize = null,
            bool useAnimation = true)
        {
            if (Modals.TryGetValue(nextModalType, out var modal))
                return await NextAsync(modal, onPreInitialize, onPostInitialize, useAnimation);

            Debug.LogError($"Modal not found : {nextModalType.Name}");
            return null;
        }

        public async UniTask PrevAsync(int count = 1, bool useAnimation = true)
        {
            count = Mathf.Clamp(count, 1, History.Count);

            if (!CurrentView) return;
            if (CurrentView.VisibleState is VisibleState.Appearing or VisibleState.Disappearing) return;

            await UniTask.WhenAll(Enumerable.Range(0, count).Select(_ => HideViewAsync(useAnimation)));
        }

        #endregion

        #region Private Methods

        async UniTask<T> NextAsync<T>(T nextView,
                                      Action<T> onPreInitialize,
                                      Action<T> onPostInitialize,
                                      bool useAnimation = true) where T : Modal
        {
            if (CurrentView != null && CurrentView.VisibleState is VisibleState.Appearing or VisibleState.Disappearing) return null;

            var backdrop = await ShowBackdrop();

            nextView.gameObject.SetActive(false);
            nextView =
#if VCONTAINER_SUPPORT
                VContainer.Unity.VContainerSettings.Instance.RootLifetimeScope.Container.Instantiate(nextView, transform);
#else
                Instantiate(nextView, transform);
#endif

            nextView.UIViewArea = this;

            nextView.OnPreInitialize.Take(1).Subscribe(_ => onPreInitialize?.Invoke(nextView)).AddTo(nextView);
            nextView.OnPostInitialize.Take(1).Subscribe(_ => onPostInitialize?.Invoke(nextView)).AddTo(nextView);

            if (backdrop)
            {
                nextView.BackDrop = backdrop;

                if (nextView.EnableBackdropButton)
                {
                    if (!nextView.BackDrop.TryGetComponent<Button>(out var button))
                        button = nextView.BackDrop.gameObject.AddComponent<Button>();

                    button.OnClickAsObservable().Subscribe(_ => PrevAsync().Forget());
                }
            }

            History.Push(nextView);

            if (nextView.BackDrop)
            {
                if (useAnimation)
                {
                    await UniTask.WhenAll
                    (
                        CurrentView.BackDrop.DOFade(1, 0.2f).SetUpdate(true).SetLink(CurrentView.BackDrop.gameObject).ToUniTask(),
                        CurrentView.ShowAsync()
                    );
                }
                else
                {
                    CurrentView.ShowAsync(false).Forget();
                    CurrentView.BackDrop.alpha = 1;
                }
            }
            else CurrentView.ShowAsync(useAnimation).Forget();

            return CurrentView as T;
        }

        async UniTask<CanvasGroup> ShowBackdrop()
        {
            if (!modalBackdrop) return null;

            var backdrop = Instantiate(modalBackdrop.gameObject, transform, true);
            if (!backdrop.TryGetComponent<CanvasGroup>(out var canvasGroup))
                canvasGroup = backdrop.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            var rectTransform = (RectTransform)backdrop.transform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            await UniTask.Yield();
            rectTransform.anchoredPosition = Vector2.zero;
            return canvasGroup;
        }

        async UniTask HideViewAsync(bool useAnimation = true)
        {
            var currentView = History.Pop();
            if (currentView.BackDrop)
            {
                if (useAnimation)
                {
                    await UniTask.WhenAll
                    (
                        currentView.BackDrop.DOFade(0, 0.2f).SetUpdate(true).SetLink(currentView.BackDrop.gameObject).ToUniTask(),
                        currentView.HideAsync()
                    );
                }
                else
                {
                    currentView.HideAsync(false).Forget();
                    currentView.BackDrop.alpha = 0;
                }
            }
            else await currentView.HideAsync(useAnimation);

            if (currentView.BackDrop) Destroy(currentView.BackDrop.gameObject);
            if (currentView) Destroy(currentView.gameObject);
        }

        #endregion

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
#if ADDRESSABLE_SUPPORT
            if (InstantiateType == InstantiateType.InstantiateByAddressable)
                RegisterModalsByPrefab.Clear();
            else
                RegisterModalsByAddressable.Clear();
#endif
        }

        #endregion
    }
}
#endif