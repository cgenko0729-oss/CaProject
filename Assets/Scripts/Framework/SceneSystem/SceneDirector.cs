using System.Collections;
using UnityEngine;

/// <summary>
/// シーンの初期化を行う基底クラス。シーンに 1 つ置く。
/// 派生クラスでは Start を書かず、Build に初期化を書くこと。
/// </summary>
public abstract class SceneDirector : MonoBehaviour
{
    /// <summary>
    /// Build が終わったら true。
    /// </summary>
    public bool isReady { get; private set; }

    private IEnumerator Start()
    {
        yield return Build();
        isReady = true;
    }

    /// <summary>
    /// シーンの初期化。終わるまでローディングは閉じない。
    /// </summary>
    protected abstract IEnumerator Build();
}
