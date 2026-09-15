using System.Collections;
using UnityEngine;

/// <summary>
/// 综合示例：演示 AssetManager / MenuManager / SceneLoader / FadeManager 的基本用法。
/// 挂在场景中任意 GameObject 上即可。
/// 前提：场景里已经有常驻的 SceneLoader 与 FadeManager（一般放在第一个启动场景）。
/// </summary>
public sealed class SampleDemo : MonoBehaviour
{

    private const string _SPHERE = "Art/TestSphere";

    private IEnumerator Start()
    {
        //// 1. AssetManager：同步加载并生成一个 Prefab
        ////    "Prefabs/Coin" 需要在 Addressables 中注册
        //var coin = AssetManager.Instantiate("Prefabs/Coin", transform);
        //Debug.Log($"[SampleDemo] 生成了 Coin: {coin != null}");

        //// 2. AssetManager：异步加载 + 生成，带完成回调
        //yield return AssetManager.LoadAndInstantiateAsync(
        //    "Prefabs/Coin",
        //    transform,
        //    onComplete: instance => Debug.Log($"[SampleDemo] 异步生成完成: {instance != null}")
        //);

        //// 3. MenuManager：打开菜单（首次会自动加载 Prefab 并生成，之后直接复用）
        //MenuManager.Open<SampleMenu>();

        //yield return new WaitForSeconds(2f);

        //// 4. MenuManager：关闭菜单
        //MenuManager.Close<SampleMenu>();

        //// 5. FadeManager：手动淡出再淡入（一般不用手动调，SceneLoader 切场景时会自动做）
        //yield return FadeManager.FadeOut();
        //yield return FadeManager.FadeIn();

        //yield return new WaitForSeconds(1f);

        //// 6. SceneLoader：切换场景
        ////    内部流程：FadeOut → 显示 Loading → 卸载旧场景资源(AssetManager.UnloadAll)
        ////             → 异步加载新场景 → 等待新场景 SceneDirector 就绪 → 隐藏 Loading → FadeIn
        //SceneLoader.Change("NextScene");

        //yield return FadeManager.instance.FadeOut(2);

        GameObject Sphere = null;
        yield return AssetManager.LoadAndInstantiateAsync(_SPHERE, null, instance => Sphere = instance);
        
        yield return FadeManager.instance.FadeIn(3);
        yield return new WaitForSeconds(2f);
        Destroy(Sphere);

        

    }
}
