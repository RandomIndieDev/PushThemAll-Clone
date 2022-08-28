using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{

    [Header("References")] 
    public Collider collider;

    public Transform finishPosition;

    public void Start()
    {
        EventsManager.Instance.OnZoneThreeCompleted += ActivateCollider;
    }

    public void OnDestroy()
    {
        EventsManager.Instance.OnZoneThreeCompleted -= ActivateCollider;
    }

    public void ActivateCollider()
    {
        collider.enabled = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        EventsManager.Instance.PlayerWon(finishPosition);
    }
}
