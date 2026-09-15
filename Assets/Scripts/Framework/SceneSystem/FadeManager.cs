using System.Collections;
using UnityEngine;

/// <summary>
/// 画面のフェードを行う常駐オブジェクト。最初のシーンに 1 つ置く。
/// 黒幕（CanvasGroup）はあらかじめ自分の子オブジェクトとしてシーンに置いておくこと。
/// </summary>
public sealed class FadeManager : Singleton<FadeManager>
{
    public const float DEFAULT_SECONDS = 0.3f;

    // alpha 0 = 透明、1 = 真っ黒
    private CanvasGroup _canvas_group;

    /// <summary>
    /// 画面を暗くする。
    /// </summary>
    public IEnumerator FadeOut(float seconds = DEFAULT_SECONDS)
    {
        return _Fade(1f, seconds);
    }

    /// <summary>
    /// 画面を明るくする。
    /// </summary>
    public IEnumerator FadeIn(float seconds = DEFAULT_SECONDS)
    {
        return _Fade(0f, seconds);
    }

    protected override void OnInit()
    {
        // 初期状態（alpha / blocksRaycasts）は上書きしない。シーン側の CanvasGroup の設定をそのまま使う
        _canvas_group = GetComponentInChildren<CanvasGroup>(includeInactive: true);
        if (_canvas_group == null)
        {
            Debug.LogError("[FadeManager] 子オブジェクトに CanvasGroup が見つかりません。黒幕をあらかじめ子として置いてください。");
        }
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
