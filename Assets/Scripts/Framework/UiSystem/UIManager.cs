using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// メニューを型で開閉する。
/// 閉じたメニューは非表示で残し、次に開くときに使い回す。
/// メニューは今のシーンに作られるので、シーン切り替えで一緒に消える（次に開くときに作り直す）。
/// </summary>
public static class UIManager
{
    // アドレスは「UI/クラス名」
    private const string _ADDRESS_PREFIX = "UI/";

    // 作成済みのメニュー。型ごとに 1 つだけ。
    private static readonly Dictionary<Type, Menu> _menu_dict = new();

    /// <summary>
    /// メニューを開いて返す。無ければ作る。すでに開いていればそのまま返す。
    /// 失敗時は null。
    /// </summary>
    public static T Open<T>() where T : Menu
    {
        var menu = Get<T>();
        if (menu == null)
        {
            menu = _Create<T>();
            if (menu == null)
            {
                return null;
            }
        }

        if (!menu.isOpen)
        {
            menu.Open();
        }

        return menu;
    }

    /// <summary>メニューを閉じる。開いていなければ何もしない。</summary>
    public static void Close<T>() where T : Menu
    {
        var menu = Get<T>();
        if (menu != null)
        {
            menu.Close();
        }
    }

    /// <summary>作成済みのメニューを返す（開きはしない）。無ければ null。</summary>
    public static T Get<T>() where T : Menu
    {
        if (!_menu_dict.TryGetValue(typeof(T), out var menu))
        {
            return null;
        }

        // シーン切り替えで破棄されていたら忘れる
        if (menu == null)
        {
            _menu_dict.Remove(typeof(T));
            return null;
        }

        return (T)menu;
    }

    /// <summary>メニューが表示中なら true。</summary>
    public static bool IsOpen<T>() where T : Menu
    {
        var menu = Get<T>();
        return menu != null && menu.isOpen;
    }

    // Prefab から生成し、初期化して閉じた状態で登録する。
    private static T _Create<T>() where T : Menu
    {
        var address = _ADDRESS_PREFIX + typeof(T).Name;
        var go = AssetManager.Instantiate(address);
        if (go == null)
        {
            return null;
        }

        var menu = go.GetComponent<T>();
        if (menu == null)
        {
            Debug.LogError($"[UIManager] {address} のルートに {typeof(T).Name} が付いていません。");
            Object.Destroy(go);
            return null;
        }

        _menu_dict[typeof(T)] = menu;
        menu.Initialize();
        return menu;
    }
}
