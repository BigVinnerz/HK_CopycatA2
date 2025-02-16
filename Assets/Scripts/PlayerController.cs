using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Walk Info
    [SerializeField] private float walkSpeed = 1;
    private float xAxis;

    // Jump Info
    [SerializeField] private float jumpForce = 45;
    [SerializeField] private Transform groundCheckpoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    //Wall Jump Info
    private bool isWallSliding;
    private float wallSlidingSpeed = 2;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask whatIsWall;

    //Dashing
    [SerializeField] private float dashingVelocity = 14f;
    [SerializeField] private float dashingTime = 0.5f;
    private Vector2 dashingDir;
    private bool isDashing;
    private bool canDash = true;

    //Components
    private TrailRenderer trailRenderer;
    private Rigidbody2D rb;

    // Double Jump
    private bool canDoubleJump;




    private bool isFacingRight = true;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    void Update()
    {


        float x = Input.GetAxis("Horizontal");

        GetInputs();
        Move();
        Jump();

        var dashInput = Input.GetButtonDown("Dash");

        if (dashInput && canDash)
        {
            isDashing = true;
            canDash = false;
            trailRenderer.emitting = true;
            dashingDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (dashingDir == Vector2.zero)
            {
                dashingDir = new Vector2(transform.localScale.x, 0);
            }

            StartCoroutine(StopDashing());
        }

        if (isDashing)
        {
            rb.linearVelocity = dashingDir.normalized * dashingVelocity;
            return;
        }

        if (Grounded())
        {
            canDash = true;
        }

        // Flip Object
        if ((x > 0 && !isFacingRight) || (x < 0 && isFacingRight))
        {
            FlipObject();
        }



        WallSlide();

    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false;
        isDashing = false;
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(walkSpeed * xAxis, rb.linearVelocity.y);
    }

    public bool Grounded()
    {
        return Physics2D.Raycast(groundCheckpoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckpoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckpoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (Grounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canDoubleJump = true;
            }
            else if (canDoubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canDoubleJump = false;
            }
        }
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, whatIsWall);
    }

    private void FlipObject()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void WallSlide()
    {
        if (IsWalled() && !Grounded() && xAxis != 0f)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }









}
