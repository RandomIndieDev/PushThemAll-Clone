using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisions : MonoBehaviour
{
    public Enemy enemy;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerWeapon"))
        {
            enemy.GotHit();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Zone"))
        {
            enemy.Disable();
        }
    }
}
