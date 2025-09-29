using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Camera camerap;
    private new Rigidbody2D rigidBody;

    private float inputAxis;
    public float moveSpeed = 8f;

    [Header("Jump")]
    public float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;
    private bool jumpQueued;   // memorizza la pressione spazio fino al FixedUpdate

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        camerap = Camera.main;
    }

    private void Update()
    {
        // Input orizzontale
        inputAxis = Input.GetAxisRaw("Horizontal");

        // Richiesta di salto (la eseguo in FixedUpdate)
        if (Input.GetKeyDown(KeyCode.Space))
            jumpQueued = true;
    }

    private void FixedUpdate()
    {
        // 1) Ground check
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 2) Movimento orizzontale via velocità (niente MovePosition!)
        var v = rigidBody.linearVelocity;
        v.x = inputAxis * moveSpeed;
        rigidBody.linearVelocity = v;

        // 3) Salto
        if (jumpQueued && isGrounded)
        {
            v = rigidBody.linearVelocity;
            v.y = jumpForce;                 // spinta verso l’alto
            rigidBody.linearVelocity = v;
        }
        jumpQueued = false;

        // 4) Clamp ai bordi della camera (solo X) senza toccare la Y
        Vector2 leftEdge = camerap.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = camerap.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        float minX = leftEdge.x + 0.5f;
        float maxX = rightEdge.x - 0.5f;

        if (rigidBody.position.x < minX || rigidBody.position.x > maxX)
            rigidBody.position = new Vector2(Mathf.Clamp(rigidBody.position.x, minX, maxX), rigidBody.position.y);
    }
}
