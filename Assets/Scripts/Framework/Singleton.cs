using UnityEngine;

/// <summary>
/// シーンをまたいで残るシングルトン。最初のシーンに 1 つ置く。
/// 派生クラスでは Awake を書かず、OnInit に初期化を書くこと。
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance { get; private set; }

    private void Awake()
    {
        // 2 つ目は消す
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = (T)this;
        DontDestroyOnLoad(gameObject);
        OnInit();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    protected virtual void OnInit()
    {
    }
}
