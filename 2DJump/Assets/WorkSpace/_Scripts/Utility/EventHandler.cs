using System.Collections.Generic;
using UnityEngine.Events;

public interface IEventInfo { }//����սӿ����ڷ���ʵ��
public class EventInfo : IEventInfo//�з���
{
    public UnityAction action;
}
public class EventInfo<T> : IEventInfo//�з���
{
    public UnityAction<T> action;
}
public class EventInfo<T, K> : IEventInfo//�з��ͣ�����������
{
    public UnityAction<T, K> action;
}

public class EventHandler
{
    /// <summary>
    /// ��¼�¼��б�
    /// HitEnemy-���е��˵��¼�
    /// </summary>
    private static Dictionary<string, IEventInfo> eventDict = new Dictionary<string, IEventInfo>();

    /// <summary>
    /// ����¼����� 
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="name">�¼���</param>
    /// <param name="action">�����¼���ί��</param>
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
    /// �Ƴ���Ӧ���¼�����
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="name">�¼���</param>
    /// <param name="action">��Ӧ��ί��</param>
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
    /// �¼�����
    /// </summary>
    /// <param name="name">�¼���</param>
    /// <typeparam name="T">��������</typeparam>
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
    /// ����¼�����
    /// ��Ҫ���ڳ����л�ʱ
    /// </summary>
    public static void Clear()
    {
        eventDict.Clear();
    }

    /*public static event Action<PlayerRef> HitEnemy;//���е��˵��¼�
    public static void CallHitEnemy(PlayerRef player)
    {
        HitEnemy?.Invoke(player);
    }*/
}
