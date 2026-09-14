using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン切り替えとローディング表示を行う常駐オブジェクト。最初のシーンに 1 つ置く。
/// </summary>
public sealed class SceneLoader : Singleton<SceneLoader>
{
    public const string LOADING_ADDRESS = "UI/Loading";

    // ローディングの最低表示時間。チラつき防止
    private const float _MIN_LOADING_SECONDS = 0.5f;

    private bool _is_changing;

    private GameObject _loading;

    /// <summary>
    /// シーン切り替え中なら true。
    /// </summary>
    public static bool isChanging => instance != null && instance._is_changing;

    /// <summary>
    /// シーンを切り替える。切り替え中は無視する。
    /// </summary>
    public static void Change(string scene_name)
    {
        if (instance == null)
        {
            Debug.LogError("[SceneLoader] 見つかりません。最初のシーンに SceneLoader を置いてください。");
            return;
        }

        if (string.IsNullOrEmpty(scene_name))
        {
            Debug.LogError("[SceneLoader] シーン名が空です。");
            return;
        }

        if (instance._is_changing)
        {
            Debug.LogWarning($"[SceneLoader] 切り替え中のため無視しました: {scene_name}");
            return;
        }

        // 古いシーンと一緒に止まらないよう、常駐の自分で動かす
        instance.StartCoroutine(instance._ChangeRoutine(scene_name));
    }

    protected override void OnInit()
    {
        // UnloadAll で消されないよう、AssetManager を使わず直接読む
        var handle = Addressables.LoadAssetAsync<GameObject>(LOADING_ADDRESS);
        handle.WaitForCompletion();
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[SceneLoader] ローディング Prefab を読めません: {LOADING_ADDRESS}。ローディング無しで動きます。");
            Addressables.Release(handle);
            return;
        }

        _loading = Instantiate(handle.Result, transform);
        _loading.SetActive(false);
    }

    private IEnumerator _ChangeRoutine(string scene_name)
    {
        _is_changing = true;

        yield return FadeManager.FadeOut();
        _SetLoadingVisible(true);
        var start_time = Time.unscaledTime;

        // 暗転中なので、ここで解放しても見た目は崩れない
        AssetManager.UnloadAll();

        yield return SceneManager.LoadSceneAsync(scene_name);

        // Director が無いシーンなら待たない
        var director = FindAnyObjectByType<SceneDirector>();
        while (director != null && !director.isReady)
        {
            yield return null;
        }

        while (Time.unscaledTime - start_time < _MIN_LOADING_SECONDS)
        {
            yield return null;
        }

        _SetLoadingVisible(false);
        yield return FadeManager.FadeIn();

        _is_changing = false;
    }

    private void _SetLoadingVisible(bool is_visible)
    {
        if (_loading != null)
        {
            _loading.SetActive(is_visible);
        }
    }
}
