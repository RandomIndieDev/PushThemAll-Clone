using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private GameObject model;

    [Header("Settings")]
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private float playerHitRange;
    [SerializeField] private float playerHitCooldown;

    

    private bool isStandingUp;
    public bool StandingUp
    {
        get => isStandingUp;
        set => isStandingUp = value;
    }

    private bool isDisabled;
    private bool isAttacking;
    private bool canHitPlayer;

    private EnemyManager enemyManager;
    private Transform target;
    
    public void Initialize(Transform targetTrans, EnemyManager manager)
    {
        isStandingUp = true;
        isAttacking = false;
        canHitPlayer = true;
        
        target = targetTrans;
        enemyManager = manager;
        isDisabled = false;
    }

    private void Update()
    {
        if (isDisabled) return;
        CheckDistanceFromTarget();
        RotateTowardsPlayer();
    }

    private void CheckDistanceFromTarget()
    {

        var distanceFromPlayer = Vector3.Distance(transform.position, target.transform.position);

        if ((distanceFromPlayer <= attackRange) && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Walk");
        }else if (distanceFromPlayer <= playerHitRange && canHitPlayer)
        {
            canHitPlayer = false;
            
            EventsManager.Instance.PlayerHit(transform);
            
            StartCoroutine(CanHitCooldown());
        }
        

    }

    IEnumerator CanHitCooldown()
    {
        yield return new WaitForSeconds(playerHitCooldown);
        canHitPlayer = true;
    }

    private void FixedUpdate()
    {
        FallFaster();
        
        if (isDisabled) return;
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        if (!isStandingUp) return;
        if (!isAttacking) return;

        var moveDirection = target.transform.position - transform.position;
        moveDirection = moveDirection.normalized;

        rigidbody.velocity = moveDirection * moveSpeed * Time.fixedDeltaTime;
    }

    private void RotateTowardsPlayer()
    {
        if (!isStandingUp) return;
        if (!isAttacking) return;

        Vector3 lookVector = target.transform.position - transform.position;
        lookVector.y = model.transform.position.y;
        Quaternion rot = Quaternion.LookRotation(lookVector);
        model.transform.rotation = Quaternion.Slerp(model.transform.rotation, rot, 1);
        
        model.transform.eulerAngles = new Vector3(0,model.transform.eulerAngles.y, 0);
    }
    
    private void FallFaster()
    {
        if (rigidbody.velocity.y <= 0)
        {
            rigidbody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    public void Disable()
    {
        if (isDisabled) return;
        
        enemyManager.EnemyDisabled();
        isDisabled = true;
    }
    

    public void GotHit()
    {
        if (!isStandingUp) return;

        isStandingUp = false;
        
        animator.SetTrigger("Fall");
    }
    
}
