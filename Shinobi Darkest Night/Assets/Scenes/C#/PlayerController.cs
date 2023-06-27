using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float collisionOffset = 0.05f;
    Vector2 movementInput;
    Vector2 movementDirection;
    public ContactFilter2D movementFilter;

    private float activeMoveSpeed;

    [Header("Dash Settings")]
    [SerializeField] float dushSpeed = 10f;
    [SerializeField] float dushDuration = 1f;
    [SerializeField] float dushCooldown = 1f;
    [SerializeField] float dushVelocityReset = 0f;
    float dushInput;
    bool isDushing;
    bool canDush = true;

    Rigidbody2D rb;

    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    // Start is called before the first frame update
    private void Start()
    {

        canDush = true;

        rb = GetComponent<Rigidbody2D>();

        activeMoveSpeed = moveSpeed;

    }

    private void Update()
    {
        if (isDushing)
        {
            return;
        }
    }

    private void FixedUpdate()
    {
        if (isDushing)
        {
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        //
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


     //Update is called once per frame
    private bool TryMove(Vector2 direction)
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

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    private IEnumerator Dush()
    {
        if (canDush == true)
        {
            canDush = false;
            isDushing = true;
            rb.velocity = new Vector2(movementDirection.x * dushSpeed, movementDirection.y * dushSpeed);
            yield return new WaitForSeconds(dushDuration);
            isDushing = false;
            rb.velocity = new Vector2(movementDirection.x * dushVelocityReset, movementDirection.y * dushVelocityReset);
            yield return new WaitForSeconds(dushCooldown);
            canDush = true;

            Debug.Log("Dush!");
            yield break;
        }
    }

    void OnDush()
    {
        StartCoroutine(Dush());
    }

    public void OnAttack()
    {
        Debug.Log("Attack");
    }

    public void OnFire()
    {
        Debug.Log("Fire!");
    }

}
