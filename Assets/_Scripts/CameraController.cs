using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam;
    
    private void Start()
    {
        EventsManager.Instance.OnPlayerDead += DetachCamera;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.OnPlayerDead -= DetachCamera;
    }

    private void DetachCamera()
    {
        cam.Follow = null;
    }
}
