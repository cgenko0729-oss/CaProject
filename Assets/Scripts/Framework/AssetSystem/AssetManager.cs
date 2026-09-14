using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Addressables でアセットを読み込み、キャッシュする。
/// </summary>
public static class AssetManager
{
    // キーはアドレス。同じアドレスを別の型で読まないこと
    private static readonly Dictionary<string, AsyncOperationHandle> _handle_dict = new();

    /// <summary>
    /// 同期で読み込む。読み込み済みならキャッシュを返す。失敗時は null。
    /// </summary>
    public static T Load<T>(string address) where T : UnityEngine.Object
    {
        if (_handle_dict.TryGetValue(address, out var cached_handle))
        {
            return cached_handle.Result as T;
        }

        var handle = Addressables.LoadAssetAsync<T>(address);
        handle.WaitForCompletion();

        return _Store(address, handle) ? handle.Result : null;
    }

    /// <summary>
    /// 非同期で読み込む。終わったら Load で取り出す。
    /// </summary>
    public static IEnumerator LoadAsync<T>(string address) where T : UnityEngine.Object
    {
        if (_handle_dict.ContainsKey(address))
        {
            yield break;
        }

        var handle = Addressables.LoadAssetAsync<T>(address);
        yield return handle;

        // 待っている間に他で読み込み済みなら、こちらは解放する
        if (_handle_dict.ContainsKey(address))
        {
            Addressables.Release(handle);
            yield break;
        }

        _Store(address, handle);
    }

    /// <summary>
    /// Prefab を読み込んで生成する。
    /// </summary>
    public static GameObject Instantiate(string address, Transform parent = null)
    {
        var prefab = Load<GameObject>(address);
        if (prefab == null)
        {
            return null;
        }

        return UnityEngine.Object.Instantiate(prefab, parent);
    }

    /// <summary>
    /// Prefab を非同期で読み込んで生成する。完了したら onComplete が呼ばれる（失敗時は null）。
    /// </summary>
    public static IEnumerator LoadAndInstantiateAsync(string address, Transform parent = null, Action<GameObject> onComplete = null)
    {
        yield return LoadAsync<GameObject>(address);

        var instance = Instantiate(address, parent);
        onComplete?.Invoke(instance);
    }

    /// <summary>
    /// 全アセットを解放する。使っているオブジェクトを先に破棄すること。
    /// </summary>
    public static void UnloadAll()
    {
        foreach (var handle in _handle_dict.Values)
        {
            Addressables.Release(handle);
        }

        _handle_dict.Clear();
    }

    // 成功ならキャッシュに入れる。失敗なら解放して false
    private static bool _Store<T>(string address, AsyncOperationHandle<T> handle)
    {
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[AssetManager] ロード失敗: {address} ({typeof(T).Name})");
            Addressables.Release(handle);
            return false;
        }

        _handle_dict.Add(address, handle);
        return true;
    }
}
