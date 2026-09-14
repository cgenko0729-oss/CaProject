using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// struct をイベントとして送受信する。
/// OnEnable で Subscribe、OnDisable で Unsubscribe すること。
/// ラムダ式は解除できないので、メソッドを渡すこと。
/// </summary>
public static class EventManager
{
    // 値は List<Action<T>>
    private static readonly Dictionary<Type, object> _subscriber_dict = new();

    /// <summary>
    /// 登録する。二重登録は無視する。
    /// </summary>
    public static void Subscribe<T>(Action<T> handler) where T : struct
    {
        var list = _GetList<T>();
        if (list.Contains(handler))
        {
            Debug.LogWarning($"[EventManager] 二重登録を無視しました: {typeof(T).Name}");
            return;
        }

        list.Add(handler);
    }

    /// <summary>
    /// 登録を解除する。
    /// </summary>
    public static void Unsubscribe<T>(Action<T> handler) where T : struct
    {
        _GetList<T>().Remove(handler);
    }

    /// <summary>
    /// イベントを送る。登録順に呼ばれる。
    /// </summary>
    public static void Publish<T>(T event_data) where T : struct
    {
        // 途中で登録・解除されても大丈夫なようにコピーを回す
        foreach (var handler in _GetList<T>().ToArray())
        {
            handler(event_data);
        }
    }

    /// <summary>
    /// 全ての登録を消す。
    /// </summary>
    public static void ClearAll()
    {
        _subscriber_dict.Clear();
    }

    // 無ければ作る
    private static List<Action<T>> _GetList<T>() where T : struct
    {
        if (!_subscriber_dict.TryGetValue(typeof(T), out var list))
        {
            list = new List<Action<T>>();
            _subscriber_dict.Add(typeof(T), list);
        }

        return (List<Action<T>>)list;
    }
}
