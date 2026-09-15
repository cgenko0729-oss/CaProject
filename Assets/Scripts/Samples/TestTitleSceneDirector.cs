using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// TestTitleScene 用の SceneDirector。挂在场景里的一个物件上。
/// 按空格键切换回 SampleScene。
/// </summary>
public sealed class TestTitleSceneDirector : SceneDirector
{
    protected override IEnumerator Build()
    {
        yield break;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SceneLoader.Change("SampleScene");
        }
    }
}
