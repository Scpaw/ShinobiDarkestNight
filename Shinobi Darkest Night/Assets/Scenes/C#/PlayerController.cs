using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 1f;

    public float collisionOffset = 0.05f;

    public float dushSpeed = 5f;

    public float dushLenght = 0.5f;

    public float dushCooldown = 1f;

    public float dushCounter;

    public float dushCoolCounter;

    private float activeMoveSpeed;

    public ContactFilter2D movementFilter;

    Vector2 movementInput;

    float dushInput;

    Rigidbody2D rb;

    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();

        activeMoveSpeed = moveSpeed;

    }

    private void FixedUpdate()
    {
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

    void OnDush()
    {
        if (dushCoolCounter <= 0 && dushCounter <= 0)
        {
            activeMoveSpeed = dushSpeed;

            dushCounter = dushLenght;
        }

        if (dushCounter > 0)
        {
            dushCounter -= Time.deltaTime;

            if (dushCounter <= 0)
            {
                activeMoveSpeed = moveSpeed;

                dushCoolCounter = dushCooldown;
            }
        }

        if (dushCoolCounter <= 0)
        {
            dushCoolCounter -= Time.deltaTime;
        }


        Debug.Log("Dush!");
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
