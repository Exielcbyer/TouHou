using System.Collections.Generic;
using UnityEngine.Events;

public interface IEventInfo { }//定义空接口用于泛型实现
public class EventInfo : IEventInfo//有泛型
{
    public UnityAction action;
}
public class EventInfo<T> : IEventInfo//有泛型
{
    public UnityAction<T> action;
}
public class EventInfo<T, K> : IEventInfo//有泛型（两个参数）
{
    public UnityAction<T, K> action;
}

public class EventHandler
{
    /// <summary>
    /// 记录事件列表
    /// HitEnemy-击中敌人的事件
    /// </summary>
    private static Dictionary<string, IEventInfo> eventDict = new Dictionary<string, IEventInfo>();

    /// <summary>
    /// 添加事件监听 
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="name">事件名</param>
    /// <param name="action">处理事件的委托</param>
    public static void AddEventListener(string name, UnityAction action)
    {
        if (eventDict.ContainsKey(name))
            (eventDict[name] as EventInfo).action += action;
        else
            eventDict.Add(name, new EventInfo() { action = action });
    }
    public static void AddEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDict.ContainsKey(name))
            (eventDict[name] as EventInfo<T>).action += action;
        else
            eventDict.Add(name, new EventInfo<T>() { action = action });
    }
    public static void AddEventListener<T, K>(string name, UnityAction<T, K> action)
    {
        if (eventDict.ContainsKey(name))
            (eventDict[name] as EventInfo<T, K>).action += action;
        else
            eventDict.Add(name, new EventInfo<T, K>() { action = action });
    }

    /// <summary>
    /// 移除对应的事件监听
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="name">事件名</param>
    /// <param name="action">对应的委托</param>
    public static void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDict.ContainsKey(name))
            (eventDict[name] as EventInfo).action -= action;
    }
    public static void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDict.ContainsKey(name))
            (eventDict[name] as EventInfo<T>).action -= action;
    }
    public static void RemoveEventListener<T, K>(string name, UnityAction<T, K> action)
    {
        if (eventDict.ContainsKey(name))
            (eventDict[name] as EventInfo<T, K>).action -= action;
    }
    /// <summary>
    /// 事件触发
    /// </summary>
    /// <param name="name">事件名</param>
    /// <typeparam name="T">参数类型</typeparam>
    public static void TriggerEvent(string name)
    {
        if (eventDict.ContainsKey(name))
            (eventDict[name] as EventInfo).action.Invoke();
    }
    public static void TriggerEvent<T>(string name, T par)
    {
        if (eventDict.ContainsKey(name))
            (eventDict[name] as EventInfo<T>).action.Invoke(par);
    }
    public static void TriggerEvent<T, K>(string name, T par1, K pra2)
    {
        if (eventDict.ContainsKey(name))
            (eventDict[name] as EventInfo<T, K>).action.Invoke(par1, pra2);
    }
    /// <summary>
    /// 清空事件中心
    /// 主要用在场景切换时
    /// </summary>
    public static void Clear()
    {
        eventDict.Clear();
    }

    /*public static event Action<PlayerRef> HitEnemy;//命中敌人的事件
    public static void CallHitEnemy(PlayerRef player)
    {
        HitEnemy?.Invoke(player);
    }*/
}
