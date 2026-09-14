using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 画面のフェードを行う常駐オブジェクト。最初のシーンに 1 つ置く。
/// </summary>
public sealed class FadeManager : Singleton<FadeManager>
{
    public const string FADE_ADDRESS = "UI/Fade";

    public const float DEFAULT_SECONDS = 0.3f;

    // alpha 0 = 透明、1 = 真っ黒
    private CanvasGroup _canvas_group;

    /// <summary>
    /// 画面を暗くする。
    /// </summary>
    public static IEnumerator FadeOut(float seconds = DEFAULT_SECONDS)
    {
        return _Fade(1f, seconds);
    }

    /// <summary>
    /// 画面を明るくする。
    /// </summary>
    public static IEnumerator FadeIn(float seconds = DEFAULT_SECONDS)
    {
        return _Fade(0f, seconds);
    }

    protected override void OnInit()
    {
        // UnloadAll で消されないよう、AssetManager を使わず直接読む
        var handle = Addressables.LoadAssetAsync<GameObject>(FADE_ADDRESS);
        handle.WaitForCompletion();
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[FadeManager] フェード Prefab を読めません: {FADE_ADDRESS}。フェード無しで動きます。");
            Addressables.Release(handle);
            return;
        }

        var fade = Instantiate(handle.Result, transform);
        _canvas_group = fade.GetComponent<CanvasGroup>();

        _canvas_group.alpha = 0f;
        _canvas_group.blocksRaycasts = false;
    }

    private static IEnumerator _Fade(float target_alpha, float seconds)
    {
        if (instance == null || instance._canvas_group == null)
        {
            Debug.LogWarning("[FadeManager] 準備できていないため、フェードせずに進みます。");
            yield break;
        }

        var group = instance._canvas_group;

        // フェード中はクリックを止める
        group.blocksRaycasts = true;

        var start_alpha = group.alpha;
        var elapsed = 0f;
        while (elapsed < seconds)
        {
            // ポーズ中でも進むように unscaled を使う
            elapsed += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start_alpha, target_alpha, elapsed / seconds);
            yield return null;
        }

        group.alpha = target_alpha;
        group.blocksRaycasts = target_alpha > 0f;
    }
}
