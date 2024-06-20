#if UNITY_NATIVE_TOASTS_SUPPORT
using System;
using UnityEngine;
using UnityNative.Toasts;

namespace JHS.Library.Manager.Toast
{
    /// <summary> 안드로이드 토스트 메시지 표시 싱글톤 </summary>
    public class ToastManager : MonoBehaviour
    {
        #region Singleton

        /// <summary> 싱글톤 인스턴스 Getter </summary>
        public static ToastManager In
        {
            get
            {
                if (instance == null) // 체크 1 : 인스턴스가 없는 경우
                    CheckExsistence();

                return instance;
            }
        }

        // 싱글톤 인스턴스
        static ToastManager instance;

        // 싱글톤 인스턴스 존재 여부 확인 (체크 2)
        static void CheckExsistence()
        {
            // 싱글톤 검색
            instance = FindObjectOfType<ToastManager>();

            // 인스턴스 가진 오브젝트가 존재하지 않을 경우, 빈 오브젝트를 임의로 생성하여 인스턴스 할당
            if (instance == null)
            {
                // 빈 게임 오브젝트 생성
                GameObject container = new GameObject("AndroidToast Singleton Container");

                // 게임 오브젝트에 클래스 컴포넌트 추가 후 인스턴스 할당
                instance = container.AddComponent<ToastManager>();
            }
        }

        void CheckInstance()
        {
            // 싱글톤 인스턴스가 존재하지 않았을 경우, 본인으로 초기화
            if (instance == null)
                instance = this;

            // 싱글톤 인스턴스가 존재하는데, 본인이 아닐 경우, 스스로(컴포넌트)를 파괴
            if (instance != null && instance != this)
            {
                Debug.Log("이미 AndroidToast 싱글톤이 존재하므로 오브젝트를 파괴합니다.");
                Destroy(this);

                // 만약 게임 오브젝트에 컴포넌트가 자신만 있었다면, 게임 오브젝트도 파괴
                var components = gameObject.GetComponents<Component>();

                if (components.Length <= 2)
                    Destroy(gameObject);
            }
        }

        void Awake()
        {
            CheckInstance();
        }

        #endregion // ==================================================================

        public enum ToastLength
        {
            /// <summary> 약 2.5초 </summary>
            Short,

            /// <summary> 약 4초 </summary>
            Long
        };

#if UNITY_EDITOR
        float __editorGuiTime = 0f;
        string __editorGuiMessage;
#endif
        IUnityNativeToasts nativeToasts;

        public void ShowToastMessage(string message, ToastLength length = ToastLength.Short)
        {
#if UNITY_EDITOR
            __editorGuiTime = length == ToastLength.Short ? 2.5f : 4f;
            __editorGuiMessage = message;
#else
        if(nativeToasts == null) nativeToasts = UnityNativeToasts.Create();
        switch (length)
        {
            case ToastLength.Short:
                nativeToasts.ShowShortToast(message);
                break;
            case ToastLength.Long:
                nativeToasts.ShowLongToast(message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(length), length, null);
        }
#endif
        }

#if UNITY_EDITOR
        GUIStyle toastStyle;

        void OnGUI()
        {
            if (__editorGuiTime <= 0f) return;

            float width = Screen.width * 0.5f;
            float height = Screen.height * 0.08f;
            Rect rect = new Rect((Screen.width - width) * 0.5f, Screen.height * 0.8f, width, height);

            if (toastStyle == null)
            {
                toastStyle = new GUIStyle(GUI.skin.box);
                toastStyle.fontSize = 36;
                toastStyle.fontStyle = FontStyle.Bold;
                toastStyle.alignment = TextAnchor.MiddleCenter;
                toastStyle.normal.textColor = Color.white;
            }

            GUI.Box(rect, __editorGuiMessage, toastStyle);
        }

        void Update()
        {
            if (__editorGuiTime > 0f)
                __editorGuiTime -= Time.unscaledDeltaTime;
        }
#endif
    }
}
#endif