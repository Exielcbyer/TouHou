using UnityEngine;
using Cinemachine;

public class SwichBounds : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.AddEventListener("AfterSceneLoadEvent", SwichConfinerShape);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener("AfterSceneLoadEvent", SwichConfinerShape);
    }

    private void SwichConfinerShape()
    {
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();
        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = confinerShape;

        confiner.InvalidatePathCache();//每次在runtime时切换Shape，清除缓存
    }
}
