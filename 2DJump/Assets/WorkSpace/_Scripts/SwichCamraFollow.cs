using UnityEngine;
using Cinemachine;

public class SwichCamraFollow : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        EventHandler.AddEventListener<Transform>("SwichCamraFollowEvent", OnSwichCamraFollow);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener<Transform>("SwichCamraFollowEvent", OnSwichCamraFollow);
    }

    private void OnSwichCamraFollow(Transform followPoint)
    {
        virtualCamera.Follow = followPoint;
    }
}
