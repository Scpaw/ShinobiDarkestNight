using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
   
    //states
    PlayerState currentState;

    public PS_Idle ps_idle = new PS_Idle();
    public PS_Run ps_run = new PS_Run();
    public PS_Projectile ps_projectile = new PS_Projectile();
    public PS_Fan ps_fan = new PS_Fan();
    public PS_Katana ps_katana = new PS_Katana();

    //animations
    [field: SerializeField] public StateDictionary StateAnimations { get; private set; }
    private AnimationClip currentAnimation;
    private float animationTime;
    public Vector2 facingDirection;
    public Vector2 movingDirection;
    public Vector2 saveDirection;
    public Animator anim;

    //katana
    public List<MeeleAttack> katanaAttacks = new List<MeeleAttack>();
    public int attackIndex;
    public float katanaSpeed;
    public float pushForce;

    //fan
    public float shurikenDropDmg;
    public float fanDmg;
    [SerializeField] private float LMBPressTime;
    [SerializeField] private bool LMBPressed;

    //move
    public Rigidbody2D rb;
    public float moveSpeed;
    public ContactFilter2D movementFilter;
    public ContactFilter2D contactFilter;
    public List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public float collisionOffset = 0.05f;

    //projectile
    public Transform projectileSpawnPoint;


    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        currentState = ps_idle;
        currentState.Enter(this);
        facingDirection = Vector2.zero;
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        currentState.Update(this);

        if ((currentState.canExitAnim || Time.time >= animationTime && currentState.loops))
        {
            ChangeAnimation(facingDirection);
        }
        else if (Time.time >= animationTime && !currentState.loops)
        { 
            currentState.ChangeStateAfterAnim(this);
        }

        if (LMBPressed)
        {
            LMBPressTime += Time.deltaTime;
            if (LMBPressTime > 0.3f)
            {
                facingDirection = projectileSpawnPoint.position - transform.position;
                ChangeStates(ps_fan);
                LMBPressTime = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdate(this);
    }

    public void ChangeStates(PlayerState state)
    {
        currentState.Exit(this);
        currentState = state;
        currentState.Enter(this);
        if (movingDirection.magnitude > 0)
        {
            facingDirection = movingDirection;
            currentState.Move(this, facingDirection);
        }
        ChangeAnimation(facingDirection);
    }

    public void OnMove(InputValue movementValue)
    {
        movingDirection = movementValue.Get<Vector2>();
        if (currentState.canMove)
        {
            if (movementValue.Get<Vector2>() != Vector2.zero)
            {
                facingDirection = movementValue.Get<Vector2>();
            }
        }
        currentState.Move(this, movementValue.Get<Vector2>());
    }


    //When LeftMouseButton(LMB) was pressed
    public void OnFire(InputValue inputValue)
    {
        if (!currentState.canAttack)
        {
            return;
        }

        LMBPressed = inputValue.Get<float>() > 0;


        if ((currentState == ps_idle || currentState == ps_run) && !LMBPressed && LMBPressTime < 0.3f)
        {
            facingDirection = projectileSpawnPoint.position - transform.position;
            ChangeStates(ps_katana);
            LMBPressTime = 0;
        }

        currentState.LMB(this, inputValue.Get<float>());
    }

    //When RightMouseButton(RMB) was pressed
    public void OnAttack(InputValue inputValue)
    {
        if (!currentState.canAttack)
        {
            return;
        }

        facingDirection = projectileSpawnPoint.position - transform.position;
        ChangeStates(ps_projectile);

        currentState.RMB(this, inputValue.Get<float>());
    }

    public void ChangeAnimation(Vector2 direction)
    {
        if (currentState != ps_katana)
        {
            if (currentAnimation == StateAnimations.GetFacingClipFromState(currentState.directionalAnimationsIndex, direction))
            {
                return;
            }
        }




        if (Mathf.Abs(direction.x) == Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                direction = new Vector2(direction.x + 0.01f, direction.y);
            }
            else if (direction.x < 0)
            {
                direction = new Vector2(direction.x - 0.01f, direction.y);
            }
        }

        if (currentState == ps_katana)
        {
            currentAnimation = katanaAttacks[attackIndex].GetFacingAnimation(direction);
        }
        else
        {
            currentAnimation = StateAnimations.GetFacingClipFromState(currentState.directionalAnimationsIndex, direction);
        }

        anim.Play(currentAnimation.name);
        animationTime = Time.time + currentAnimation.length;
    }

    public void DoSomething()
    {
        Debug.Log("something");
    }

    public void ChangeToIdle()
    {
        if (movingDirection.magnitude > 0)
        {
            ChangeStates(ps_run);
        }
        else
        {
            ChangeStates(ps_idle);
        }

    }
}