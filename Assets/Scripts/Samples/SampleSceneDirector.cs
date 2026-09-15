using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// SampleScene 用の SceneDirector。挂在场景里的一个物件上（跟 FadeManager/SceneLoader 不同，不用常驻）。
/// 按空格键切换到 TestTitleScene。
/// </summary>
public sealed class SampleSceneDirector : SceneDirector
{
    protected override IEnumerator Build()
    {
        yield break;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SceneLoader.Change("TestTitleScene");
        }
    }
}
