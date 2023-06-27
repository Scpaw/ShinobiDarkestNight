using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private float activeMoveSpeed;
    
    //Dash parameters
    [Header("Dash Settings")]
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashCooldown = 1f;
    private float dashVelocityReset = 0f;
    bool isDashing;
    bool canDash = true;

    //Attack parameters
    [Header("Attack Settings")]
    [SerializeField] float damageValue = 10f;
    [SerializeField] float damageDuration = 1f;
    [SerializeField] float damageCooldown = 1f;

    //RigidBody2D
    protected Rigidbody2D rb;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;


    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    // Start is called before the first frame update
    private void Start()
    {

        canDash = true;

        rb = GetComponent<Rigidbody2D>();

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
            isDashing = true;
            rb.velocity = new Vector2(movementDirection.x * dashSpeed, movementDirection.y * dashSpeed);
            yield return new WaitForSeconds(dashDuration);
            isDashing = false;
            rb.velocity = new Vector2(movementDirection.x * dashVelocityReset, movementDirection.y * dashVelocityReset);// <--- This line is necessary if you want to stop the player after initializing "Dush"
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;

            Debug.Log("Dush!");
            yield break;
        }
    }

    //When one of WSAD was pressed
    public void OnMove(InputValue movementValue)
    {

        movementInput = movementValue.Get<Vector2>();

        Debug.Log("Move!");
    }

    //When Space was pressed
    public void OnDash()
    {
        StartCoroutine(Dash());
    }

    //When LeftMouseButton(LMB) was pressed
    public void OnAttack()
    {
        if (isDashing == true)
        {
            return;
        }
        Debug.Log("Attack!");
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
