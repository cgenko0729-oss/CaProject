using UnityEngine;

/// <summary>
/// メニューの基底クラス。Prefab のルートに付ける。
/// Prefab は自分で Canvas を持ち、Addressables に「UI/クラス名」で登録する。
/// </summary>
public abstract class MenuBase : MonoBehaviour
{
    /// <summary>表示中なら true。</summary>
    public bool isOpen => gameObject.activeSelf;

    /// <summary>自分を閉じる（非表示にする）。ボタンから直接呼んでよい。</summary>
    public void Close()
    {
        if (!isOpen)
        {
            return;
        }

        OnClose();
        gameObject.SetActive(false);
    }

    /// <summary>生成直後に 1 回だけ呼ばれる。ボタンの登録などはここ。</summary>
    protected virtual void OnInitialize() { }

    /// <summary>開くたびに呼ばれる。表示内容の更新はここ。</summary>
    protected virtual void OnOpen() { }

    /// <summary>閉じるたびに呼ばれる。</summary>
    protected virtual void OnClose() { }

    // MenuManager から呼ぶ。生成直後の初期化。閉じた状態で待機させる。
    internal void Initialize()
    {
        OnInitialize();
        gameObject.SetActive(false);
    }

    // MenuManager から呼ぶ。
    internal void Open()
    {
        gameObject.SetActive(true);
        OnOpen();
    }
}
