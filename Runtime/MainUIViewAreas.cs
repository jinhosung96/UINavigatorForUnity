#if  UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && (R3_SUPPORT || UNIRX_SUPPORT)
using System.Linq;
using JHS.Library.UINavigator.Runtime.Modal;
using JHS.Library.UINavigator.Runtime.Page;
using JHS.Library.UINavigator.Runtime.Sheet;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JHS.Library.UINavigator.Runtime
{
    public sealed class MainUIViewAreas : MonoBehaviour
    {
        #region Field

        static MainUIViewAreas instance;
        [SerializeField] SheetViewArea mainSheetViewArea;
        [SerializeField] PageViewArea mainPageViewArea;
        [SerializeField] ModalViewArea mainModalViewArea;

        #endregion

        #region Property

        public static MainUIViewAreas In => instance = instance ? instance : FindObjectOfType<MainUIViewAreas>() ?? new GameObject(nameof(MainUIViewAreas)).AddComponent<MainUIViewAreas>();

        #endregion

        #region Unity Lifecycle

        void Awake()
        {
            if (instance) Destroy(gameObject);

            if (!mainSheetViewArea && !mainPageViewArea && !mainModalViewArea)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (scene.isLoaded)
                    {
                        if(!mainSheetViewArea) mainSheetViewArea = scene.GetRootGameObjects().Select(root => root.GetComponentInChildren<SheetViewArea>()).FirstOrDefault(x => x);
                        if(!mainPageViewArea) mainPageViewArea = scene.GetRootGameObjects().Select(root => root.GetComponentInChildren<PageViewArea>()).FirstOrDefault(x => x);
                        if(!mainModalViewArea) mainModalViewArea = scene.GetRootGameObjects().Select(root => root.GetComponentInChildren<ModalViewArea>()).FirstOrDefault(x => x);
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public T GetMain<T>() where T : UIViewArea<T>
        {
            if (typeof(T) == typeof(SheetViewArea)) return mainSheetViewArea as T;
            if (typeof(T) == typeof(PageViewArea)) return mainPageViewArea as T;
            if (typeof(T) == typeof(ModalViewArea)) return mainModalViewArea as T;
            return null;
        }
    
        public void SetMain<T>(T container) where T : UIViewArea<T>
        {
            if (typeof(T) == typeof(SheetViewArea)) mainSheetViewArea = container as SheetViewArea;
            if (typeof(T) == typeof(PageViewArea)) mainPageViewArea = container as PageViewArea;
            if (typeof(T) == typeof(ModalViewArea)) mainModalViewArea = container as ModalViewArea;
        }

        #endregion
    }
}
#endif