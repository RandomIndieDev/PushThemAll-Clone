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
    [SerializeField] private float fallMultiplier;
    [SerializeField] private Ease playerWeaponHitEase;


    private bool allowWeaponHit;
    private bool playerIsDead;
    private bool isGrounded;

    private Vector3 originalPos;
    
    

    void Start()
    {
        originalPos = weaponClone.transform.localPosition;
        allowWeaponHit = true;
        isGrounded = true;
    }


    private void Update()
    {
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

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor") && isGrounded)
        {
            isGrounded = false;
            StartCoroutine(MakePlayerFall());
        }
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
