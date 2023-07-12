using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    /*
    This code when inserted in input code like "OnAttack", prevents it from activate when "Dash" is going
    Do NOT use in "OnMove" input

    if (isDushing == true)
    {
      return;
    }
    */

    //Movement parameters
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float collisionOffset = 0.05f;
    Vector2 movementInput;
    Vector2 movementDirection;
    public ContactFilter2D movementFilter;

    bool facingRight;
    bool facingUp;
    bool facingIdle;

    Animator myAnim;

    private float activeMoveSpeed;

    //Dash parameters
    [Header("Dash Settings")]
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashCooldown = 0f;
    [SerializeField] float dushCooldownSpeed = 10f;
    [SerializeField] float dashMaxCooldown = 1f;
    [SerializeField] float dashMinCooldown = -1f;
    [SerializeField] Slider dashDelaySlider;
    [SerializeField] GameObject Canvas;
    private float dashVelocityReset = 0f;
    float t = 0f;
    bool isDashing;
    bool canDash = true;

    //Attack parameters
    [Header("Attack Settings")]
    //[SerializeField] float damageValue = 10f;
    //[SerializeField] float damageDuration = 1f;
    [SerializeField] float damageCooldown = 1f;

    //RigidBody2D
    protected Rigidbody2D rb;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;

    [Header("Projectile Settings")]
    [Header("ProjectileShooter Prefabs")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectileRotation;
    //[SerializeField] float xSpeed = 1f;
    //[SerializeField] float ySpeed = 1f;
    //float xVelocity = 0f;
    //float yVelocity = 0f;

    public float animationIdleTime;

    //public GameObject projectilePrefab;

    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    // Start is called before the first frame update
    private void Start()
    {
        //facing
        rb = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        Canvas.SetActive(false);
        activeMoveSpeed = moveSpeed;

    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        //Prevents the Player from using
        if (isDashing == true)
        {
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask (gameObject.layer));
            contactFilter.useLayerMask = true;
            return;
        }

        dashDelaySlider.maxValue = dashMaxCooldown;
        dashDelaySlider.minValue = dashMinCooldown;
        dashDelaySlider.value = dashCooldown;
        if (canDash == false)
        {
            if (dashCooldown < dashMaxCooldown)
            {
                t += Time.deltaTime / dushCooldownSpeed;

                dashCooldown = Mathf.Lerp(dashMinCooldown, dashMaxCooldown, t);
            }
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        //Prevent the player from blocking on collisions when moving
        if (movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput);

            if (!success)
            {
                success = TryMove(new Vector2(movementInput.x, 0));

                if (!success)
                {
                    success = TryMove(new Vector2(0, movementInput.y));
                }
            }
        }

        movementDirection = new Vector2(moveX, moveY).normalized;

        Vector2 CurrentVelocity = movementInput;

        float xVector = CurrentVelocity.x;
        float yVector = CurrentVelocity.y;

        //float xIdleVector = velocity.x;
        //float yIdleVector = velocity.y;

        if (xVector > 0 && yVector == 0)//Right
        {
            //xVelocity = xSpeed;
            //yVelocity = 0f;

            myAnim.SetBool("Up", false);
            myAnim.SetBool("Right", true);
            myAnim.SetBool("Left", false);
            myAnim.SetBool("Down", false);
        }
        else if (xVector < 0 && yVector == 0)//Left
        {
            //xVelocity = -xSpeed;
            //yVelocity = 0f;

            myAnim.SetBool("Up", false);
            myAnim.SetBool("Right", false);
            myAnim.SetBool("Left", true);
            myAnim.SetBool("Down", false);
        }
        else if (xVector == 0 && yVector > 0)//Up
        {
            //xVelocity = 0f;
            //yVelocity = ySpeed;

            myAnim.SetBool("Up", true);
            myAnim.SetBool("Right", false);
            myAnim.SetBool("Left", false);
            myAnim.SetBool("Down", false);
        }
        else if (xVector == 0 && yVector < 0)//Down
        {
            //xVelocity = 0f;
            //yVelocity = -ySpeed;

            myAnim.SetBool("Up", false);
            myAnim.SetBool("Right", false);
            myAnim.SetBool("Left", false);
            myAnim.SetBool("Down", true);
        }
    }

    //The following order of "count"(RigidBody2D's parameters of movement) will be executed when "count" = 0
    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + collisionOffset
                );

            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //When Dash is pressed, the following order will be executed
    private IEnumerator Dash()
    {
        if (canDash == true)
        {
            canDash = false;
            Canvas.SetActive(true);
            isDashing = true;
            rb.velocity = new Vector2(movementDirection.x * dashSpeed, movementDirection.y * dashSpeed);
            yield return new WaitForSeconds(dashDuration);
            isDashing = false;
            rb.velocity = new Vector2(movementDirection.x * dashVelocityReset, movementDirection.y * dashVelocityReset);// <--- This line is necessary if you want to stop the player after initializing "Dash"
            t = 0.0f;
            dashCooldown -= dashCooldown;
            yield return new WaitUntil(() => dashCooldown >= dashMaxCooldown);
            Canvas.SetActive(false);
            canDash = true;
        }
    }

    public void SpawnPoint()
    {
        GameObject newProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation.transform.rotation);
    }

    //When one of WSAD was pressed
    public void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    //When Space was pressed
    public void OnDash()
    {
        StartCoroutine(Dash());
    }

    //When LeftMouseButton(LMB) was pressed
    public void OnAttack()
    {
        //GameObject Projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        //Projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity, yVelocity);

        SpawnPoint();

        if (isDashing == true)
        {
            return;
        }
    }

    //When RightMouseButton(LMB) was pressed
    public void OnFire()
    {
        if (isDashing == true)
        {
            return;
        }
        Debug.Log("Fire!");
    }

}
