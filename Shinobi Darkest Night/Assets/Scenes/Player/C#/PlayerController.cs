using Cinemachine;
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
    [field: SerializeField] public CharacterState AttackAnim { get; private set; }
    [field: SerializeField] public CharacterState HealAnim { get; private set; }
    [field: SerializeField] public CharacterAnimationStateDictionary StateAnimations { get; private set; }
    [field: SerializeField] public float RunVelocityTreshchold { get; private set; } = 0.1f;

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
    [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;

    [Header("Movement Settings")]
    [SerializeField] float collisionOffset = 0.05f;
    Vector2 movementInput = Vector2.zero;
    Vector2 movementDirection;
    public ContactFilter2D movementFilter;

    protected Rigidbody2D rb;
    protected ContactFilter2D contactFilter;

    bool canMove;
    Vector2 saveDirection;

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
    //[Header("Attack Settings")]
    //[SerializeField] float damageValue = 10f;
    //[SerializeField] float damageDuration = 1f;
    //[SerializeField] float damageCooldown = 1f;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectileRotation;

    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    private Animator myAnim;
    private CharacterState currentState;
    private AnimationClip currentClip;
    private Vector2 facingDirection;
    private float timeToEndAnimation = 0f;

    //melee attack
    public float attackDamage;
    public float attackCooldown;
    private float startAttackCooldown;
    public float pushForce;

    //projectile
    public int projectileNumber;
    private int maxProjectileNumber;
    public Text projectileText;

    //stamina
    public Slider staminaSlider;
    float stamina = 50;
    float maxStamina;
    float staminaReg;
    public float staminaRegRate;
    bool canRegenStamina;

    //heal
    bool isHealing;
    CinemachineVirtualCamera cam;
    float startLensSize;
    public float changeLensSize;

    private void Start()
    {
        //facing
        rb = GetComponent<Rigidbody2D>();
        myAnim = transform.Find("Grafika").GetComponent<Animator>();
        Canvas.SetActive(false);
        CurrentState = IdleAnim;
        canMove = true;
        startAttackCooldown = attackCooldown;
        attackCooldown = 0;
        maxProjectileNumber = projectileNumber;
        maxStamina = stamina;
        staminaSlider.maxValue = maxStamina;
        cam = FindAnyObjectByType<CinemachineVirtualCamera>();
        startLensSize = cam.m_Lens.OrthographicSize;
    }

    private void Update()
    {
        timeToEndAnimation = timeToEndAnimation - Time.deltaTime;

        if (currentState == ThrowAnim || currentState == AttackAnim)
        {
            if (timeToEndAnimation <= 0)
            {
                
                if (!isDashing)
                {
                    if (movementInput != Vector2.zero)
                    {
                        CurrentState = RunAnim;
                    }
                    else
                    {
                        CurrentState = IdleAnim;
                    }
                }
                ChangeClip();
                movementInput = saveDirection;
                canMove = true;
            }
        }
        else if (currentState.CanExitWhilePlaying = true || timeToEndAnimation <= 0)
        {
            if (!isDashing)
            {
                if (movementInput != Vector2.zero)
                {
                    CurrentState = RunAnim;
                }
                else
                {
                    CurrentState = IdleAnim;
                }
            }  
            ChangeClip();
        }
        if (!canMove)
        {
            movementInput = Vector2.zero;
        }

        //attack cooldwon
        if (attackCooldown > 0)
        { 
            attackCooldown -= Time.deltaTime;
        }
        projectileText.text = projectileNumber.ToString();

        //stamina
        staminaSlider.value = stamina;
        if (canMove && !isDashing)
        {
            if (stamina < maxStamina)
            {
                if (staminaReg <= 0)
                {
                    stamina += staminaRegRate * Time.deltaTime;
                }
                else
                {
                    staminaReg -= Time.deltaTime;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            StartHealing();
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
        if (canMove)
        {
            if (currentState.CanMove)
            {
                Vector2 moveForce = movementInput * MoveSpeed * Time.deltaTime;
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
        else
        {
            rb.AddForce(Vector2.zero);
        }
       
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
                MoveSpeed * Time.fixedDeltaTime + collisionOffset
                );

            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * MoveSpeed * Time.fixedDeltaTime);

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
        if (canDash && canMove)
        {
            canDash = false;            
            Canvas.SetActive(true);
            isDashing = true;
            CurrentState = DashAnim;
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
        projectileNumber -= 1;
    }

    //When one of WSAD was pressed
    public void OnMove(InputValue movementValue)
    {
        if (canMove)
        {
            movementInput = movementValue.Get<Vector2>();

            if (movementInput != Vector2.zero)
            {
                facingDirection = movementInput;
            }
        }
        else
        { 
            saveDirection = movementValue.Get<Vector2>();
        }
     
    }

    //When Space was pressed
    public void OnDash()
    {
        StartCoroutine(Dash());
    }

    //When LeftMouseButton(LMB) was pressed
    public void OnAttack()
    {
        if (attackCooldown <= 0 && stamina >= 5)
        {
            if (isDashing == true)
            {
                return;
            }

            GameObject[] hit = projectileSpawnPoint.GetComponent<AttackCollider>().enemiesThatCanHit.ToArray();
            if (hit == null || hit.Length == 0)
            {
                return;
            }
            foreach (GameObject enemy in hit)
            {
                if (enemy.layer == 6)
                {
                    enemy.GetComponent<Enemy>().enemyAddDamage(attackDamage, true);
                    if (enemy.GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static)
                    {
                        enemy.GetComponent<Rigidbody2D>().AddForce(projectileSpawnPoint.right * pushForce, ForceMode2D.Impulse);
                    }
                }
            }
            if (movementInput != Vector2.zero || movementInput != saveDirection)
            {
                saveDirection = movementInput;
            }
            else
            {
                saveDirection = Vector2.zero;
            }
            facingDirection = projectileSpawnPoint.position - transform.position;
            attackCooldown = startAttackCooldown;
            CurrentState = AttackAnim;           
            canMove = false;
            UseStamina(5);

        }
    }

    //When RightMouseButton(LMB) was pressed
    public void OnFire()
    {
        if (stamina >= 5)
        {
            if (projectileNumber > 0)
            {
                if (isDashing == true)
                {
                    return;
                }
                if (movementInput != Vector2.zero || movementInput != saveDirection)
                {
                    saveDirection = movementInput;
                }
                else
                {
                    saveDirection = Vector2.zero;
                }

                facingDirection = projectileSpawnPoint.position - transform.position;
                canMove = false;
                CurrentState = ThrowAnim;
                UseStamina(5);
            }
        }
       
    }

    public void OnHeal()
    {
        if (!isHealing)
        {
            isHealing = true;
        }
        //play animation
    }

    public void StartHealing()
    {
        StartCoroutine(ChangeCamSize());
    }

    public void UseStamina(float staminaToUse)
    {
        stamina -= staminaToUse;
        staminaReg = 2.5f;
    }

    private IEnumerator ChangeCamSize()
    {
        if (cam.m_Lens.OrthographicSize >= changeLensSize)
        {
            while (cam.m_Lens.OrthographicSize >= changeLensSize)
            {
                cam.m_Lens.OrthographicSize -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else 
        {
            while (cam.m_Lens.OrthographicSize >= changeLensSize)
            {
                cam.m_Lens.OrthographicSize -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}