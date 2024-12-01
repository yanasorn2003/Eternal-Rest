using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpPower = 20f;
    [SerializeField] private GameObject playerBone;  // Reference to the sprite

    private CustomInput input;
    private Vector2 moveVector = Vector2.zero;
    private Rigidbody2D rb = null;
    private Animator animator = null;
    private float horizontal;
    public GameObject attackPoint;
    public float attackRadius;
    public LayerMask Monsters;
    public float damage;

    private Vector3 originalScale;  // To store the original scale of the sprite

    // Reference to the ground check
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.02f;
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        input = new CustomInput();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Store the original scale of the sprite
        if (playerBone != null)
        {
            originalScale = playerBone.transform.localScale;
        }
        else
        {
            Debug.LogWarning("PlayerBone is not assigned.");
        }
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementCancelled;
        input.Player.Jump.performed += OnJumpPerformed;  // Add the jump action
        input.Player.Jump.canceled += OnJumpCancelled;
        input.Player.Attack.performed += OnAttackPerformed;
        input.Player.Attack.canceled += OnAttackCancelled;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Movement.performed -= OnMovementPerformed;
        input.Player.Movement.canceled -= OnMovementCancelled;
        input.Player.Jump.performed -= OnJumpPerformed;  // Remove the jump action
        input.Player.Jump.canceled -= OnJumpCancelled;
        input.Player.Attack.performed -= OnAttackPerformed;
        input.Player.Attack.canceled -= OnAttackCancelled;
    }

    private void FixedUpdate()
    {
        // Apply movement
        rb.velocity = new Vector2(moveVector.x * moveSpeed, rb.velocity.y);

    }

    private bool IsGrounded()
    {

        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        moveVector = value.ReadValue<Vector2>();
        Debug.Log(moveVector);

        // Check movement direction and flip the sprite accordingly
        if (moveVector.x > 0)
        {
            // Move right: use the original scale (facing right)
            playerBone.transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (moveVector.x < 0)
        {
            // Move left: flip the sprite horizontally (facing left)
            playerBone.transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        // Set the "isRunning" animation state if the player is moving
        animator.SetBool("isRunning", moveVector.x != 0 || moveVector.y != 0);
    }

    private void OnMovementCancelled(InputAction.CallbackContext value)
    {
        moveVector = Vector2.zero;
        animator.SetBool("isRunning", false);
    }

    private void OnJumpPerformed(InputAction.CallbackContext value)
    {
        // Only allow jumping if the player is grounded
        if (IsGrounded())
        {
            rb.AddForce(new Vector3(0, jumpPower, 0), ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
        }
    }

    private void OnJumpCancelled(InputAction.CallbackContext value)
    {
        animator.SetBool("isJumping", false);
    }

    private void OnAttackPerformed(InputAction.CallbackContext value)
    {
        animator.SetBool("isAttack", true);
        Collider2D[] monster = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, Monsters);

        foreach(Collider2D monsterGameObject in monster)
        {
            monsterGameObject.GetComponent<EnemyHealth>().health -= damage;
            Debug.Log("Hit enemy");
        }
    }

    private void OnAttackCancelled(InputAction.CallbackContext value)
    {
        animator.SetBool("isAttack", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRadius);
    }
}
