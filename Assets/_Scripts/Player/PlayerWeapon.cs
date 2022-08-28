using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public Rigidbody rigidbody;
    public void Activate(Transform copyTransform)
    {
        transform.SetPositionAndRotation(copyTransform.position, copyTransform.rotation);
        
        gameObject.SetActive(true);
        rigidbody.AddForce(transform.forward * 30, ForceMode.VelocityChange);

        StartCoroutine(Deactivate());

    }

    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(.3f);
        
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}
