using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    private bool called = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || called) return;
        
        called = true; 
        EventsManager.Instance.SpawnTriggerEntered();
    }
}
