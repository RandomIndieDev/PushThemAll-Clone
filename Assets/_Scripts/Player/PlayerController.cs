using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private FloatingJoystick floatingJoystick;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private GameObject player;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider playerMainCollider;
    
    [SerializeField] private PlayerWeapon weapon;
    [SerializeField] private GameObject heldWeapon;
    [SerializeField] private GameObject weaponClone;

    [Header("Settings")] 
    [SerializeField] private float moveSpeed;
    [SerializeField] private float gotHitPower;
    [SerializeField] private float gotHitCooldown;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private Ease playerWeaponHitEase;
    


    private bool allowMove;
    private bool allowWeaponHit;
    private bool playerIsDead;
    private bool isGrounded;
    private bool isHit;

    private Vector3 originalPos;
    
    

    void Start()
    {
        EventsManager.Instance.OnPlayerWon += PlayerLevelCompleted;
        EventsManager.Instance.OnPlayerHit += PlayerGotHit;
        
        originalPos = weaponClone.transform.localPosition;
        allowWeaponHit = true;
        isGrounded = true;
        allowMove = true;
    }

    void OnDisable()
    {
        EventsManager.Instance.OnPlayerWon -= PlayerLevelCompleted;
        EventsManager.Instance.OnPlayerHit -= PlayerGotHit;
    }


    private void Update()
    {
        if (!allowMove) return;
        
        if (Input.GetMouseButtonUp(0) && allowWeaponHit)
        {
            allowWeaponHit = false;
            weapon.Activate(heldWeapon.transform);
            
            var seq = DOTween.Sequence();
            seq.Append(weaponClone.transform.DOLocalMove(weaponClone.transform.localPosition + new Vector3(0, 0, 4), .2f).SetEase(playerWeaponHitEase));
            seq.Append(weaponClone.transform.DOLocalMove(originalPos, .2f)).onComplete += AllowHit;
        }
    }

    private void AllowHit()
    {
        allowWeaponHit = true;
    }

    private void FixedUpdate()
    {
        if (!allowMove) return;
        
        FallFaster();
        
        if (isGrounded)
            MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 direction = Vector3.forward * floatingJoystick.Vertical + Vector3.right * floatingJoystick.Horizontal;

        rigidbody.velocity = direction * moveSpeed * Time.fixedDeltaTime;
        
        var characterSpeed = Mathf.Clamp(Mathf.Abs(floatingJoystick.Vertical) + Mathf.Abs(floatingJoystick.Horizontal), 0,
            1);
        
        animator.SetFloat("MoveSpeed", characterSpeed);

        if (direction != Vector3.zero)
        {
            player.transform.forward = direction;
        }
    }
    
    private void FallFaster()
    {
        if (rigidbody.velocity.y <= 0)
        {
            rigidbody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
    

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor") && isGrounded)
        {
            isGrounded = false;
            StartCoroutine(MakePlayerFall());
        }
    }

    private void PlayerLevelCompleted(Transform finishLocation)
    {
        allowMove = false;
        
        animator.SetTrigger("Dance");
        
        heldWeapon.SetActive(false);
        weaponClone.SetActive(false);
        player.transform.DOMove(finishLocation.position, .5f);
        player.transform.DORotate(finishLocation.rotation.eulerAngles, .5f);
        
        GameManager.Instance.RestartLevel();
    }

    private void PlayerGotHit(Transform hitPosition)
    {
        if (isHit) return;
        StartCoroutine(DoPlayerHit(hitPosition));
    }

    IEnumerator DoPlayerHit(Transform hitPosition)
    {
        allowMove = false;
        isHit = true;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        var direction = transform.position - hitPosition.position;
        
        animator.SetTrigger("Fall");
        
        rigidbody.AddForce(direction * gotHitPower, ForceMode.VelocityChange);
        
        yield return new WaitForSeconds(gotHitCooldown);

        allowMove = true;
        isHit = false;
    }

    IEnumerator MakePlayerFall()
    {
        yield return new WaitForSeconds(.1f);

        if (isGrounded) yield break;
        rigidbody.velocity = Vector3.zero;
        rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
        playerMainCollider.enabled = false;
        
        EventsManager.Instance.PlayerDead();
    }
        
        
    
    
}
