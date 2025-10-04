using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 2f;
    [SerializeField] private float dashCoolDown = 3f;



    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private int extraJumps = 1; // numero di salti extra (1 = doppio salto)

    [Header("Wall Slide / Wall Jump")]
    [SerializeField] private float wallSlideSpeed = -2f;
    [SerializeField] private float wallJumpForce = 12f;
    [SerializeField] private float wallJumpPush = 8f;
    [SerializeField] private float wallJumpDuration = 0.2f;
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Camera cam;

    private float moveInput;
    private bool jumpPressed;
    private bool jumpReleased;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallJumping;

    private float originalGravityScale;

    private bool canDash = true;
    private bool isDashing = false;

    private float coyoteCounter;
    private float jumpBufferCounter;
    private float wallJumpTimer;

    private int jumpsRemaining;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        originalGravityScale = rb.gravityScale;
        cam = Camera.main;
    }

    private void Start()
    {
        jumpsRemaining = extraJumps + 1; // salto base + extra
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        CheckGround();
        CheckWalls();

        if (!isWallJumping && !isDashing) ApplyMovement();

        ApplyJump();
        ApplyWallSlide();
        ApplyWallJump();
        ClampToCamera();
    }

    // ------------------------------
    // FUNZIONI SEPARATE
    // ------------------------------

    private void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump")) jumpPressed = true;
        if (Input.GetButtonUp("Jump")) jumpReleased = true;

        if (Input.GetKeyDown(KeyCode.C) && canDash) {

            StartCoroutine(Dash());

        } 
    }

    IEnumerator Dash()
    {

        canDash = false;
        isDashing = true;

        Vector2 v = rb.linearVelocity;

        v.x = dashSpeed;

        v.y = 0f;

        rb.gravityScale = 0;

        rb.linearVelocity = v;

        yield return new WaitForSeconds(dashTime);

        isDashing = false;

        rb.linearVelocity = Vector2.zero;

        rb.gravityScale = originalGravityScale;

        yield return new WaitForSeconds(dashCoolDown);

        canDash = true;


    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            jumpsRemaining = extraJumps; // reset salti quando tocchi terra
        }
        else
        {
            coyoteCounter -= Time.fixedDeltaTime;
        }

        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferTime;
            jumpPressed = false;
        }
        else
        {
            jumpBufferCounter -= Time.fixedDeltaTime;
        }
    }

    private void CheckWalls()
    {
        bool leftWall = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, groundLayer);
        bool rightWall = Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, groundLayer);
        isTouchingWall = (leftWall || rightWall);
    }

    private void ApplyMovement()
    {
        Vector2 v = rb.linearVelocity;
        v.x = moveInput * moveSpeed;
        rb.linearVelocity = v;
    }

    private void ApplyJump()
    {
        // Salto da terra (con coyote time e jump buffer)
        if (jumpBufferCounter > 0 && (coyoteCounter > 0 || jumpsRemaining > 0))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0;

            // se non sei a terra, consumi un salto
            if (!isGrounded) jumpsRemaining--;
        }

        // Jump cut
        if (jumpReleased && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
        jumpReleased = false;
    }

    private void ApplyWallSlide()
    {
        if (!isGrounded && isTouchingWall && rb.linearVelocity.y < wallSlideSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallSlideSpeed);
        }
    }

    private void ApplyWallJump()
    {
        if (jumpBufferCounter > 0 && isTouchingWall && !isGrounded)
        {
            float direction = wallCheckLeft && Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, groundLayer) ? 1 : -1;

            rb.linearVelocity = new Vector2(direction * wallJumpPush, wallJumpForce);

            isWallJumping = true;
            wallJumpTimer = wallJumpDuration;
            jumpBufferCounter = 0;
        }

        if (isWallJumping)
        {
            wallJumpTimer -= Time.fixedDeltaTime;
            if (wallJumpTimer <= 0) isWallJumping = false;
        }
    }

    private void ClampToCamera()
    {
        Vector2 leftEdge = cam.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        float minX = leftEdge.x + 0.5f;
        float maxX = rightEdge.x - 0.5f;

        if (rb.position.x < minX || rb.position.x > maxX)
        {
            rb.position = new Vector2(Mathf.Clamp(rb.position.x, minX, maxX), rb.position.y);
        }
    }
}
