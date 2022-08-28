using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private CinemachineVirtualCamera camZoom;
    [SerializeField] private CinemachineVirtualCamera camEnd;
    

    private bool isZoomedOut;
     
    private void Start()
    {
        EventsManager.Instance.OnPlayerDead += DetachCamera;
        EventsManager.Instance.OnPlayerWon += ActivateEndCamera;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.OnPlayerDead -= DetachCamera;
        EventsManager.Instance.OnPlayerWon -= ActivateEndCamera;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) && !isZoomedOut)
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
        isZoomedOut = true;
        camZoom.Priority = 11;
        StartCoroutine(ReturnToNormalCam());
    }

    private void ActivateEndCamera(Transform endPosition)
    {
        camEnd.Priority = 12;
        
        camZoom.Follow = null;
        cam.Follow = null;
    }
    IEnumerator ReturnToNormalCam()
    {
        yield return new WaitForSeconds(4f);
        camZoom.Priority = 9;
        isZoomedOut = false;
    }
    
    
}
