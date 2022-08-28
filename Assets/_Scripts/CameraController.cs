using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private CinemachineVirtualCamera camZoom;
     
    private void Start()
    {
        EventsManager.Instance.OnPlayerDead += DetachCamera;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.OnPlayerDead -= DetachCamera;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            ZoomCameraOut();
        }
    }

    private void DetachCamera()
    {
        cam.Follow = null;
    }

    private void ZoomCameraOut()
    {
        camZoom.Priority = 11;
        StartCoroutine(ReturnToNormalCam());
    }

    IEnumerator ReturnToNormalCam()
    {
        yield return new WaitForSeconds(1f);
        camZoom.Priority = 9;
    }
}
