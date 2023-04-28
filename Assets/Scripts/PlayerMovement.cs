using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10.0f;
    [SerializeField] float jumpSpeed = 5.0f;
    float originalGravity;
    CapsuleCollider2D playerCollider;
    Vector2 moveInput;
    Rigidbody2D rb;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        FlipSprite();
        ClimbLadder();
    }

    void OnMove(InputValue inputValue) {
        moveInput = inputValue.Get<Vector2>();
    }

    void OnJump(InputValue inputValue) {
        if (!playerCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
            return;
        }
        if(inputValue.isPressed) {
            rb.velocity += new Vector2(0.0f, jumpSpeed);
        }
    }

    void Run() {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        animator.SetBool("IsRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite() {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed) {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1.0f);
        }
    }

    void ClimbLadder() {
        if(playerCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
            Vector2 playerVelocity = new Vector2(rb.velocity.x, moveInput.y * runSpeed);
            rb.velocity = playerVelocity;
            rb.gravityScale = 0.0f;
            bool playerHasVerticalSpeed = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
            if (playerHasVerticalSpeed) {
                animator.SetBool("IsClimbing", true);
            }
            else {
                animator.SetBool("IsClimbing", false);
            }
        }
        else {
            rb.gravityScale = originalGravity;
            animator.SetBool("IsClimbing", false);
        }
    }
}
