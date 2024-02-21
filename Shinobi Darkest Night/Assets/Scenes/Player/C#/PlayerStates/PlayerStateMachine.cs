using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    public static PlayerStateMachine Instance;
   
    //states
    public PlayerState currentState;
    public PlayerState lastState;

    public PS_Idle ps_idle = new PS_Idle();
    public PS_Run ps_run = new PS_Run();
    public PS_Projectile ps_projectile = new PS_Projectile();
    public PS_Fan ps_fan = new PS_Fan();
    public PS_Katana ps_katana = new PS_Katana();
    public PS_Dash ps_dash = new PS_Dash();

    public PS_Start_Itaiken ps_start_itaiken = new PS_Start_Itaiken();


    public List<PlayerState> ability = new List<PlayerState>();

    //animations
    [field: SerializeField] public StateDictionary StateAnimations { get; private set; }
    public AnimationClip currentAnimation;
    public float animationTime;
    public Vector2 facingDirection;
    public Vector2 movingDirection;
    public Vector2 saveDirection;
    public Animator anim;

    //katana
    public List<MeeleAttack> katanaAttacks = new List<MeeleAttack>();
    public int attackIndex;
    public float katanaSpeed;
    public float pushForce;
    public Coroutine lastAttack;

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

    //dash
    public float dashSpeed = 10f;
    public float dashDuration = 1f;
    public float dashVelocityReset = 0f;
    public float dashCooldown;
    public bool canDash = true;

    //projectile
    public int projectileNumber;
    public Text projectileText;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public GameObject projectileRotation;

    //itaiken
    public GameObject itaikenToSpawn;

    //shokyaku
    public LayerMask enemyLayer;

    //health
    public PlayerHealth hp;

    //test
    public bool godMode;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        currentState = ps_idle;
        currentState.Enter(this);
        facingDirection = Vector2.zero;
        rb = GetComponent<Rigidbody2D>();
        canDash = true;
        projectileText.text = projectileNumber.ToString();
        hp = GetComponent<PlayerHealth>();

    }


    void Update()
    {
        //logic and anim update
        currentState.Update(this);

        if ((currentState.canExitAnim || Time.time >= animationTime && currentState.loops))
        {
            ChangeAnimation(facingDirection);
        }
        else if (Time.time >= animationTime && !currentState.loops)
        { 
            currentState.ChangeStateAfterAnim(this);
        }


        //activate attack
        if (LMBPressed)
        {
            LMBPressTime += Time.deltaTime;
            if (LMBPressTime > 0.4f)
            {
                facingDirection = projectileSpawnPoint.position - transform.position;
                ChangeStates(ps_fan);
                LMBPressTime = 0;
            }
        }

        //hp
        if (lastAttack == null)
        { 
            hp.DoHealth();
        }

        //test
        if (Input.GetKeyDown(KeyCode.P))
        {
            godMode = !godMode;
        }
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdate(this);
    }

    public void ChangeStates(PlayerState state)
    {
        if (state == null)
        {
            return;
        }

        currentState.Exit(this);
        currentState = state;
        currentState.Enter(this);
        float convert = 0;
        if (LMBPressed)
        {
            convert = 1;
        }
        currentState.LMB(this,convert);
        if (movingDirection.magnitude > 0)
        {
            if (currentState.canMove)
            {
                facingDirection = movingDirection;
            }
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
        currentState.LMB(this, inputValue.Get<float>());

        if (!currentState.canAttack)
        {
            if (LMBPressed)
            { 
                LMBPressed = false;
                LMBPressTime = 0;
            }
            return;
        }

        LMBPressed = inputValue.Get<float>() > 0;
        LMBPressTime = 0;

        if ((currentState == ps_idle || currentState == ps_run) && !LMBPressed && LMBPressTime < 0.3f)
        {
            facingDirection = projectileSpawnPoint.position - transform.position;
            ChangeStates(ps_katana);
            LMBPressTime = 0;
        }
    }

    //When RightMouseButton(RMB) was pressed
    public void OnAttack(InputValue inputValue)
    {
        currentState.RMB(this, inputValue.Get<float>());

        if (!currentState.canAttack)
        {
            return;
        }

        if (projectileNumber > 0)
        {
            facingDirection = projectileSpawnPoint.position - transform.position;
            ChangeStates(ps_projectile);
        }
    }

    public void OnDash(InputValue dashValue)
    {
        if (dashValue.Get<float>() == 1 && currentState.canMove && movingDirection.magnitude > 0.01f && canDash)
        {
            lastState = currentState;
            ChangeStates(ps_dash);
        }
    }

    public void OnKey1()
    {
        if (currentState.canAttack && !currentState.ability && ability.Count > 0)
        {
            ChangeStates(ability[0]);
        }
        else if(currentState.canExitAnim && currentState.ability)
        { 
            ChangeToIdle();
        }
    }

    public void OnKey2() 
    {
        if (currentState.canAttack && !currentState.ability && ability.Count > 1)
        {
            ChangeStates(ability[1]);
        }
        else if (currentState.canExitAnim && currentState.ability)
        {
            ChangeToIdle();
        }
    }

    public void OnKey3()
    {
        if (currentState.canAttack && !currentState.ability && ability.Count > 2)
        {
            ChangeStates(ability[2]);
        }
        else if (currentState.canExitAnim && currentState.ability)
        {
            ChangeToIdle();
        }
    }

    public void OnKey4()
    {
        if (currentState.canAttack && !currentState.ability && ability.Count > 3)
        {
            ChangeStates(ability[3]);
        }
        else if (currentState.canExitAnim && currentState.ability)
        {
            ChangeToIdle();
        }
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

    public void SetLastAttack(bool onlyStop)
    {
        if (lastAttack != null)
        {
            StopCoroutine(lastAttack);
        }

        if (!onlyStop)
        {
            lastAttack = StartCoroutine(TimeSinceLastAttack(0.2f));
        }
    }

    public void Shoot()
    {
        GameObject newProjectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.Euler(projectileSpawnPoint.transform.rotation.eulerAngles));
        projectileNumber -= 1;
        projectileText.text = projectileNumber.ToString();
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

    public void ResetDash()
    {
        StopCoroutine(DashReseting());
        StartCoroutine(DashReseting());
    }

    private IEnumerator DashReseting()
    { 
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private IEnumerator TimeSinceLastAttack(float time)
    { 
        yield return new WaitForSeconds(time);
        lastAttack = null;
    }



    public GameObject GetHead()
    {
        return anim.transform.GetChild(0).gameObject;
    }

}