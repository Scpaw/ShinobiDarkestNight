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
    [field: SerializeField] public CharacterState IdleAnim { get; private set; }
    [field: SerializeField] public CharacterState RunAnim { get; private set; }
    [field: SerializeField] public CharacterState DashAnim { get; private set; }
    [field: SerializeField] public CharacterState ThrowAnim { get; private set; }
    [field: SerializeField] public CharacterAnimationStateDictionary StateAnimations { get; private set; }
    [field: SerializeField] public float RunVelocityTreshchold { get; private set; } = 0.05f;
    
    public CharacterState CurrentState
    {
        get
        {
            return currentState;
        }
        private set
        {
            if (currentState != value)
            {
                currentState = value;
                ChangeClip();
                timeToEndAnimation = currentClip.length;
            }
        }
    }

    //Movement parameters
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float collisionOffset = 0.05f;
    Vector2 movementInput;
    Vector2 movementDirection;
    public ContactFilter2D movementFilter;

    private Animator myAnim;
    private CharacterState currentState;
    private AnimationClip currentClip;
    private Vector2 facingDirection;
    private float timeToEndAnimation = 0f;

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

    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    private void Start()
    {
        //facing
        rb = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        Canvas.SetActive(false);
        activeMoveSpeed = moveSpeed;
        CurrentState = IdleAnim;
    }

    private void Update()
    {
        timeToEndAnimation -= Mathf.Max(timeToEndAnimation - Time.deltaTime);

        if (currentState.CanExitWhilePlaying = true || timeToEndAnimation <= 0)
        {
            if (movementInput != Vector2.zero && rb.velocity.magnitude > RunVelocityTreshchold)
            {
                CurrentState = RunAnim;
            }
            else
            {
                CurrentState = IdleAnim;
            }

            ChangeClip();
        }
    }

    private void ChangeClip()
    {
        AnimationClip expectedClip = StateAnimations.GetFacingClipFromState(currentState, facingDirection);

        if (currentClip == null || currentClip != expectedClip)
        {
            myAnim.Play(expectedClip.name);
            currentClip = expectedClip;
        }
    }

    private void FixedUpdate()
    {
        if (currentState.CanMove)
        {
            Vector2 moveForce = movementInput * moveSpeed * Time.deltaTime;
            rb.AddForce(moveForce);
        }

        //Prevents the Player from using
        if (isDashing == true)
        {
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
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

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movementDirection = new Vector2(moveX, moveY).normalized;
    }

    //The following order of "count"(RigidBody2D's parameters of movement) will be executed when "count" = 0
    bool TryMove(Vector2 direction)
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

    private void SpawnPoint()
    {
        GameObject newProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation.transform.rotation);
    }

    //When one of WSAD was pressed
    public void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();

        if(movementInput != Vector2.zero) 
        {
            facingDirection = movementInput;
        }

        CurrentState = RunAnim;
    }

    //When Space was pressed
    public void OnDash()
    {
        StartCoroutine(Dash());
    }

    //When LeftMouseButton(LMB) was pressed
    public void OnAttack()
    {
        CurrentState = ThrowAnim;

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
