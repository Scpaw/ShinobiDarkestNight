using Cinemachine;
using Pathfinding.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
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
    [field: SerializeField] public float MoveSpeed { get; private set; } = 3f;

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

    [Header("Melee attack")]
    public float attackDamage;
    public float attackCooldown;
    private float startAttackCooldown;
    public float pushForce;
    public List<GameObject> enemiesToHit;

    [Header("Projectile")]
    public int projectileNumber;
    public Text projectileText;

    [Header("Stamina")]
    public Slider staminaSlider;
    float stamina = 100;
    float maxStamina;
    float staminaReg;
    public float staminaRegRate;
    bool canRegenStamina;
    private bool canAttack;

    [Header("Heal")]
    public bool isHealing;
    CinemachineVirtualCamera cam;
    float startLensSize;
    public float changeLensSize;
    private float timeToHeal;
    public bool canHeal;
    public AnimationClip startHeal;
    public AnimationClip loopHeal;

    [Header("Speeding Up")]
    private bool isSpeedingUp;
    private bool buttonUp;

    [Header("Shokyaku")]
    private bool shokyaku;
    private Collider2D[] hits;
    public LayerMask enemyLayer;
    private float shokyakuTimer;

    [Header("Desumiru")]
    private bool desumiru;
    private int desumiruState;
    [SerializeField] private float desumiuRadius;
    [SerializeField] AnimationClip[] desumiruAnimations;
    public float test;
    private Vector2 point2;
    private void Awake()
    {
        Instance = this;
    }
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
        maxStamina = stamina;
        staminaSlider.maxValue = maxStamina;
        cam = FindAnyObjectByType<CinemachineVirtualCamera>();
        startLensSize = cam.m_Lens.OrthographicSize;
        movementDirection = Vector2.zero;
        movementInput = Vector2.zero;
        canAttack = true;
        canHeal = false;
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
                SaveMovement();
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


        //attack cooldwon
        if (attackCooldown > 0)
        { 
            attackCooldown -= Time.deltaTime;
        }
        projectileText.text = projectileNumber.ToString();

        //stamina
        staminaSlider.value = stamina;
        if (canMove && !isDashing && canAttack)
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

        //health
        if (isHealing && canHeal && !isSpeedingUp)
        {
            timeToHeal -= Time.deltaTime;
            if (timeToHeal < 0)
            {
                StopHealing();
            }
        }

        if (isSpeedingUp)
        { 
            UseStamina(4* Time.deltaTime);
            GetComponent<PlayerHealth>().AddDamage(2* Time.deltaTime);
            if (stamina <= 0)
            {
                isSpeedingUp = false;
                canDash = true;
                canAttack = true;
                MoveSpeed = 3;
                buttonUp = true;
            }
        }

        //Shokyaku
        if (shokyaku)
        {
            ParticleManager.instance.UseParticle("Fire", projectileSpawnPoint.position, projectileSpawnPoint.rotation.eulerAngles);
            hits =Physics2D.OverlapAreaAll(new Vector2(projectileSpawnPoint.GetChild(0).position.x, projectileSpawnPoint.GetChild(0).position.y), new Vector2(projectileSpawnPoint.GetChild(1).position.x, projectileSpawnPoint.GetChild(1).position.y), enemyLayer);
            foreach(Collider2D enemy in hits)
            {
                if (enemy.GetComponent<EnemyHealth>())
                {
                    enemy.GetComponent<EnemyHealth>().enemyAddDamage(40 * Time.deltaTime, false, false);
                    if (!enemy.GetComponent<EnemyHealth>().isStuned)
                    {
                        StartCoroutine(enemy.GetComponent<EnemyHealth>().Stuned());
                    }
                }
            }
            UseStamina(10 * Time.deltaTime);
            GetComponent<PlayerHealth>().AddDamage(4 * Time.deltaTime);
            shokyakuTimer -= Time.deltaTime;
            if (shokyakuTimer <= 0)
            {
                shokyaku = false;
                movementInput = saveDirection;
                shokyaku = false;
                canDash = true;
                canMove = true;
                canAttack = true;
            }
        }


        //testing and time
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(SlowTime());
        }
    }

    private void ChangeClip()
    {
        if (!isHealing)
        {
            if (Mathf.Abs(facingDirection.x) == Mathf.Abs(facingDirection.y) && facingDirection != Vector2.zero)
            {
                if (facingDirection.x > 0)
                {
                    facingDirection = new Vector2(facingDirection.x + 0.01f, facingDirection.y);
                }
                else if(facingDirection.x < 0)
                {
                    facingDirection = new Vector2(facingDirection.x - 0.01f, facingDirection.y);
                }
            }
            AnimationClip expectedClip = StateAnimations.GetFacingClipFromState(currentState, facingDirection);
            if (currentClip == null || currentClip != expectedClip)
            {
                myAnim.Play(expectedClip.name);
                currentClip = expectedClip;
            }
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
            if (canMove && movementInput != movementDirection)
            {
                movementInput = movementDirection;
            }
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
            UseStamina(10);
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
        if (canAttack && !isHealing)
        {
            if (attackCooldown <= 0 && stamina >= 5)
            {
                if (isDashing == true)
                {
                    return;
                }
                Collider2D[] hit = Physics2D.OverlapCircleAll(projectileSpawnPoint.position, projectileSpawnPoint.GetComponent<CircleCollider2D>().radius);
                if (hit == null || hit.Length == 0)
                {
                    return;
                }
                foreach (Collider2D enemy in hit)
                {
                    if (enemy.gameObject.layer == 6 && enemy.gameObject != null)
                    {
                        if (enemy.gameObject.GetComponent<EnemyHealth>())
                        {
                            enemy.gameObject.GetComponent<EnemyHealth>().enemyAddDamage(attackDamage, true,true);
                        }
                        if (enemy.gameObject.GetComponent<Rigidbody2D>() != null && enemy.GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static)
                        {
                            enemy.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                            enemy.gameObject.GetComponent<Rigidbody2D>().AddForce(projectileSpawnPoint.right * pushForce, ForceMode2D.Impulse);
                            StartCoroutine(enemy.gameObject.GetComponent<EnemyHealth>().Stuned());
                        }
                    }
                }
                SaveMovement();
                facingDirection = projectileSpawnPoint.position - transform.position;
                attackCooldown = startAttackCooldown;
                CurrentState = AttackAnim;
                canMove = false;
                UseStamina(5);
            }
        }       
    }

    //When RightMouseButton(RMB) was pressed
    public void OnFire()
    {
        if (canAttack && !isHealing && !desumiru)
        {
            if (stamina >= 5)
            {
                if (projectileNumber > 0)
                {
                    if (isDashing == true)
                    {
                        return;
                    }
                    SaveMovement();
                    facingDirection = projectileSpawnPoint.position - transform.position;
                    canMove = false;
                    CurrentState = ThrowAnim;
                    UseStamina(5);
                }
            }
        }
        else if (desumiru && canAttack)
        {
            DesumiruState();
        }
    }

    public void OnHeal()
    {
        if (canAttack)
        {
            if (GetComponent<PlayerHealth>().playerCourrentHealth < GetComponent<PlayerHealth>().playerMaxHealth)
            {
                timeToHeal = 1;
                if (!isHealing)
                {
                    SaveMovement();
                    isHealing = true;
                    StartHealing();
                    canMove = false;
                }
                else
                {
                    if (canHeal)
                    {
                        if (currentClip != loopHeal)
                        {
                            myAnim.Play(loopHeal.name);
                            currentClip = loopHeal;
                        }
                        GetComponent<PlayerHealth>().AddHealth(3);
                    }
                }
            }
        }
    }

    public void OnSpeedUp()
    {
        if (stamina > 0.1f && !buttonUp)
        {
            isSpeedingUp = !isSpeedingUp;
            if (isSpeedingUp)
            {
                canDash = false;
                canAttack = false;
                MoveSpeed = 4.5f;
            }
            else
            {
                canDash = true;
                canAttack = true;
                MoveSpeed = 3;
            }
        }
        else if (buttonUp)
        {
            buttonUp = false;
        }
    }

    public void OnShokyaku(InputValue movementValue)
    {
        if (movementValue.Get<float>() == 1)
        {
            if (canAttack && !isHealing && canMove)
            {
                shokyakuTimer = 4;
                shokyaku = true;
                canDash = false;
                canMove = false;
                canAttack = false;
                SaveMovement();
            }
        }
        else if (movementValue.Get<float>() == 0)
        {
            movementInput = saveDirection;
            shokyaku = false;
            canDash = true;
            canMove = true;
            canAttack = true;
        }
    }

    public void OnDesumiru()
    {
        if (canAttack && !desumiru)
        {
            desumiruState = 0;
            desumiru = true;
            canDash = false;
            canMove = false;
            canAttack = false;
        }
    }


    void DesumiruState()
    {
        Debug.Log("desumiru");
        StartCoroutine(DesumiruAttackUse());
    }

    public void DesumiruAttack()
    { 
        
    }

    public void StartHealing()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeCamSizeUp());
    }

    public void StopHealing()
    {
        if (isHealing)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeCamSizeDown());
            movementInput = saveDirection;
            canMove = true;
        }
    }

    public void UseStamina(float staminaToUse)
    {
        stamina -= staminaToUse;
        staminaReg = 1f;
    }

    void SaveMovement()
    {
        if (movementInput != Vector2.zero && movementInput != saveDirection)
        {
            saveDirection = movementInput;
        }
        else if (movementInput == Vector2.zero)
        {
            saveDirection = Vector2.zero;
        }
        else
        {
            saveDirection = movementDirection;
        }
    }

    private IEnumerator ChangeCamSizeUp()
    {
        if (isHealing)
        {
            myAnim.Play(startHeal.name);
            currentClip = startHeal;
            while (cam.m_Lens.OrthographicSize >= changeLensSize)
            {
                cam.m_Lens.OrthographicSize -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private IEnumerator ChangeCamSizeDown()
    {
        if (isHealing)
        {
            isHealing = false;
            canHeal = false;
            while (cam.m_Lens.OrthographicSize <= startLensSize)
            {
                cam.m_Lens.OrthographicSize += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private IEnumerator SlowTime()
    {
        float waitTime = 1;
        if (Time.timeScale < 1)
        {
            while (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else if (Time.timeScale > 0.6f)
        {
            while (Time.timeScale > 0.6f)
            {
                Time.timeScale -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            Debug.LogError("to fast use");
        }

        while (waitTime > 0)
        {
            canAttack = true;
            waitTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (Time.timeScale < 1)
        {
            while (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else if (Time.timeScale > 0.6f)
        {
            while (Time.timeScale > 0.6f)
            {
                Time.timeScale -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            Debug.LogError("to fast use");
        }

    }

    private IEnumerator DesumiruAttackUse()
    {
        List<Vector2> vectors = new List<Vector2>();
        test = 1;
        vectors.Add(Vector2.zero);
        vectors.Add(Vector2.zero);
        vectors[1] = point2;
        while (test > 0)
        {
            if (!myAnim.GetComponent<EdgeCollider2D>().enabled)
            {
                myAnim.GetComponent<EdgeCollider2D>().enabled = true;
            }
            point2 = new Vector2( desumiuRadius * Mathf.Cos((test* 360 * Mathf.Deg2Rad)-(Mathf.Deg2Rad * 90)),  desumiuRadius * Mathf.Sin((test * 360 * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)));
            test -= Time.deltaTime/3;
            vectors[1] = point2;
            myAnim.GetComponent<EdgeCollider2D>().SetPoints(vectors);
            yield return new WaitForEndOfFrame();
        }
        myAnim.GetComponent<EdgeCollider2D>().enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, point2);
     
    }
    public GameObject GetPlayer()
    {
        return gameObject;
    }
}
