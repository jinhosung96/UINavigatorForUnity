#if ADDRESSABLE_SUPPORT
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public static class AddressableUtil
{
    #region Fields

    static readonly Subject<IResourceLocator> onInitialize = new();
    static readonly Subject<List<string>> onCheckForCatalogUpdates = new();
    static readonly Subject<List<IResourceLocator>> onUpdateCatalogs = new();
    static readonly Subject<long> onGetDownloadSize = new();
    static readonly Subject<DownloadStatus> onDownloadDependencies = new();
    static readonly Subject<Unit> onEndDownload = new();

    #endregion

    #region Properties

    public static IObservable<IResourceLocator> OnInitialize => onInitialize;
    public static IObservable<List<string>> OnCheckForCatalogUpdates => onCheckForCatalogUpdates;
    public static IObservable<List<IResourceLocator>> OnUpdateCatalogs => onUpdateCatalogs;
    public static IObservable<long> OnGetDownloadSize => onGetDownloadSize;
    public static IObservable<DownloadStatus> OnDownloadDependencies => onDownloadDependencies;
    public static IObservable<Unit> OnEndDownload => onEndDownload;

    #endregion

    #region Public Methods

    public static async UniTask<IResourceLocator> InitializeAsync()
    {
        var resourceLocator = await Addressables.InitializeAsync().ToUniTask();
        onInitialize.OnNext(resourceLocator);
        return resourceLocator;
    }

    public static async UniTask<List<string>> CheckForCatalogUpdatesAsync(bool autoReleaseHandle = true)
    {
        var catalogList = await Addressables.CheckForCatalogUpdates(autoReleaseHandle).ToUniTask();
        onCheckForCatalogUpdates.OnNext(catalogList);
        return catalogList;
    }

    public static async UniTask<List<IResourceLocator>> UpdateCatalogsAsync(IEnumerable<string> catalogs = null, bool autoReleaseHandle = true)
    {
        var resourceLocators = await Addressables.UpdateCatalogs(catalogs, autoReleaseHandle);
        return resourceLocators;
    }

    public static async UniTask<List<IResourceLocator>> UpdateCatalogsAsync(bool autoCleanBundleCache, IEnumerable<string> catalogs = null, bool autoReleaseHandle = true)
    {
        var resourceLocators = await Addressables.UpdateCatalogs(autoCleanBundleCache, catalogs, autoReleaseHandle);
        return resourceLocators;
    }
    
    public static async UniTask<long> GetDownloadSizeAsync(object key)
    {
        var downloadSize = await Addressables.GetDownloadSizeAsync(key).ToUniTask();
        onGetDownloadSize.OnNext(downloadSize);
        return downloadSize;
    }

    public static async UniTask<long> GetDownloadSizeAsync(string key)
    {
        var downloadSize = await Addressables.GetDownloadSizeAsync(key).ToUniTask();
        onGetDownloadSize.OnNext(downloadSize);
        return downloadSize;
    }

    public static async UniTask<long> GetDownloadSizeAsync(IEnumerable keys)
    {
        var downloadSize = await Addressables.GetDownloadSizeAsync(keys).ToUniTask();
        onGetDownloadSize.OnNext(downloadSize);
        return downloadSize;
    }

    public static async UniTask DownloadDependenciesAsync(object key)
    {
        var handle = Addressables.DownloadDependenciesAsync(key);

        await DownloadDependenciesAsync(handle);
    }

    public static async UniTask DownloadDependenciesAsync(string key, bool autoReleaseHandle = false)
    {
        var handle = Addressables.DownloadDependenciesAsync(key, autoReleaseHandle);

        await DownloadDependenciesAsync(handle);
    }

    public static async UniTask DownloadDependenciesAsync(IList<IResourceLocation> locations, bool autoReleaseHandle = false)
    {
        var handle = Addressables.DownloadDependenciesAsync(locations, autoReleaseHandle);

        await DownloadDependenciesAsync(handle);
    }

    public static async UniTask DownloadDependenciesAsync(IEnumerable keys, Addressables.MergeMode mode, bool autoReleaseHandle = false)
    {
        var handle = Addressables.DownloadDependenciesAsync(keys, mode, autoReleaseHandle);

        await DownloadDependenciesAsync(handle);
    }

    #endregion

    #region Private Methods

    static async UniTask DownloadDependenciesAsync(AsyncOperationHandle handle)
    {
        while (!handle.IsDone)
        {
            DownloadStatus status = handle.GetDownloadStatus();
            onDownloadDependencies.OnNext(status);

            await UniTask.NextFrame();
        }

        onEndDownload.OnNext(Unit.Default);
    }

    #endregion
}
#endif