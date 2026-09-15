using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 示例菜单：演示 MenuBase / MenuManager 的用法。
/// Prefab 需要在 Addressables 中注册为 "UI/SampleMenu"，根节点挂这个脚本。
/// </summary>
public sealed class SampleMenu : MenuBase
{
    [SerializeField] private Button _closeButton;

    protected override void OnInitialize()
    {
        // 只在第一次生成时执行一次，适合做按钮注册
        _closeButton.onClick.AddListener(Close);
    }

    protected override void OnOpen()
    {
        // 每次打开都会执行，适合做刷新显示内容
        Debug.Log("[SampleMenu] 打开了");
    }

    protected override void OnClose()
    {
        Debug.Log("[SampleMenu] 关闭了");
    }
}
