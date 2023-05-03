using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10.0f;
    [SerializeField] float jumpSpeed = 5.0f;
    [SerializeField] Vector2 deathKick = new Vector2(10.0f, 10.0f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    float originalGravity;
    bool isAlive = true;
    CapsuleCollider2D bodyCollider;
    Vector2 moveInput;
    Rigidbody2D rb;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
        animator = GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isAlive) {
            Run();
            FlipSprite();
            ClimbLadder();
            Die();
        }
    }

    void OnMove(InputValue inputValue) {
        if (isAlive) {
            moveInput = inputValue.Get<Vector2>();
        }
    }

    void OnJump(InputValue inputValue) {
        if(isAlive) {
            if (!bodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
                return;
            }
            if(inputValue.isPressed) {
                rb.velocity += new Vector2(0.0f, jumpSpeed);
            }
        }
    }

    void OnFire(InputValue inputValue) {
        if(inputValue.isPressed && isAlive) {
            Instantiate(bullet, gun.position, transform.rotation);
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
        if(bodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
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
    private void Die() {
        if(bodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards"))) {
            isAlive = false;
            animator.SetTrigger("Dying");
            rb.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}
