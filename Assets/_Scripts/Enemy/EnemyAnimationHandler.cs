using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour
{

    [SerializeField] private Enemy enemy;

    public void HasGottenUp()
    {
        enemy.StandingUp = true;
    }
}
