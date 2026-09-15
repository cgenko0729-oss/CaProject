using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン切り替えとローディング表示を行う常駐オブジェクト。最初のシーンに 1 つ置く。
/// ローディング UI は子オブジェクトとしてあらかじめ置き、Inspector で _loading にセットしておくこと。
/// </summary>
public sealed class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] private GameObject _loading;

    private bool _is_changing;

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
            Debug.LogError("見つかりません。最初のシーンに SceneLoader を置いてください。");
            return;
        }

        if (string.IsNullOrEmpty(scene_name))
        {
            Debug.LogError("シーン名が空です。");
            return;
        }

        if (instance._is_changing)
        {
            Debug.LogWarning($"切り替え中のため無視しました: {scene_name}");
            return;
        }

        // 古いシーンと一緒に止まらないよう、常駐の自分で動かす
        instance.StartCoroutine(instance._ChangeRoutine(scene_name));
    }

    protected override void OnInit()
    {
        if (_loading == null)
        {
            Debug.LogError("_loading が未設定です。");
            return;
        }

        _loading.SetActive(false);
    }

    private IEnumerator _ChangeRoutine(string scene_name)
    {
        _is_changing = true;

        yield return FadeManager.instance.FadeOut(1);
        _SetLoadingVisible(true);

        // 暗転中なので、ここで解放しても見た目は崩れない
        AssetManager.UnloadAll();

        yield return SceneManager.LoadSceneAsync(scene_name);

        // Director が無いシーンなら待たない
        var director = FindAnyObjectByType<SceneDirector>();
        while (director != null && !director.isReady)
        {
            yield return null;
        }

        _SetLoadingVisible(false);
        yield return FadeManager.instance.FadeIn(1);

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
