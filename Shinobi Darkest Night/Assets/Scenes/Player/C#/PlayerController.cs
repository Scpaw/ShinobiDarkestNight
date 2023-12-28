using Cinemachine;
using Pathfinding.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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
    [field: SerializeField] public CharacterState ShokyakuAnim { get; private set; }
    [field: SerializeField] public CharacterState MezameAnim { get; private set; }
    [field: SerializeField] public CharacterState ItaikenRunAnim { get; private set; }
    [field: SerializeField] public CharacterState ItaikenIdleAnim { get; private set; }
    [field: SerializeField] public CharacterState ItaikenAttackAnim { get; private set; }
    [field: SerializeField] public CharacterAnimationStateDictionary StateAnimations { get; private set; }
    [field: SerializeField] public float RunVelocityTreshchold { get; private set; } = 0.1f;

    public CharacterState CurrentState
    {
        get
        {
            return currentState;
        }
         set
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

    public bool canMove;
    [SerializeField] bool dialogue;
    
    public bool Dialogue
    {
        get
        {
            return dialogue;
        }
        set
        {
            if (dialogue != value)
            {
                dialogue = value;
                if (value)
                {
                    canMove = false;
                    canAttack = false;
                    desumiru = false;
                    isHealing = false;
                    CurrentState = RunAnim;
                    ChangeClip();
                }
                else
                { 
                    canAttack = true;
                    canMove = true;
                }
            }
        }
    }

    Vector2 saveDirection;

    //Dash parameters
    [Header("Dash Settings")]
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashCooldown = 0f;
    [SerializeField] float dushCooldownSpeed = 10f;
    [SerializeField] float dashMaxCooldown = 1f;
    [SerializeField] float dashMinCooldown = -1f;
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
    [SerializeField] public Transform projectileSpawnPoint;
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
    private PlayerHealth hp;

    [Header("Projectile")]
    public int projectileNumber;
    public Text projectileText;

    [Header("Stamina")]
    public Image staminaSlider;
    private Text staminaText;
    float stamina = 100;
    float maxStamina;
    public float staminaRegRate;
    public bool canAttack;

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
    private float startSpeed;

    [Header("Shokyaku")]
    public bool shokyaku;
    public bool shokyakuAttack;
    private Collider2D[] hits;
    public LayerMask enemyLayer;
    private float shokyakuTimer;
    public AnimationClip startShokyaku;
    private bool stopShokyaku;

    [Header("Desumiru")]
    private bool desumiru;
    [SerializeField] private float desumiuRadius;
    [SerializeField] AnimationClip[] desumiruAnimations;
    private float test;
    private float slowWaitingTime;
    public Vector2 point2;
    Coroutine desumiruAttackCorutine;
    public bool desumiruPressed;

    [Header("Death")]
    [SerializeField] private AnimationClip deathAnim;

    [Header("Itaiken")]
    [SerializeField] private GameObject itaikenToSpawn;
    [SerializeField] private AnimationClip startItaiken;
    public bool itaiken;

    [Header("Inventory")]
    [SerializeField] private Image shade;
    public bool inventoryOpen; // change to private
    public List<GameObject> candy;
    public List<GameObject> candySpawned = new List<GameObject>(); // this too
    private int currentCandyIndex;
    private float maxItemYPos;
    private List<GameObject> itemsOnScreen =  new List<GameObject>();
    private bool snap;
    private Text inventoryText;
    private GameObject topItem;

    [Header("Power ups")]
    public float sakuramochi;
    public float yokan;
    public float dango;
    public float mizuame;
    public float powerCoolDown;

    public bool godMode;

    [Header("things to find")]
    public List<Find> thingsToFind;
    public Canvas findDisplay;
    private bool skipPress;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        //facing
        rb = GetComponent<Rigidbody2D>();
        myAnim = transform.Find("Grafika").GetComponent<Animator>();
        CurrentState = IdleAnim;
        canMove = true;
        startAttackCooldown = attackCooldown;
        attackCooldown = 0;
        maxStamina = stamina;
        cam = FindAnyObjectByType<CinemachineVirtualCamera>();
        startLensSize = cam.m_Lens.OrthographicSize;
        movementDirection = Vector2.zero;
        movementInput = Vector2.zero;
        canAttack = true;
        canHeal = false;

        snap = true;
        inventoryText = shade.GetComponentInChildren<Text>();
        hp = GetComponent<PlayerHealth>();
        findDisplay.gameObject.SetActive(false);
        staminaText = staminaSlider.transform.parent.GetComponentInChildren<Text>();
        startSpeed = MoveSpeed;
    }

    private void OnEnable()
    {
        if (shokyaku)
        {
            stopShokyaku = true;
        }
    }

    private void Update()
    {       
        timeToEndAnimation -= Time.deltaTime;


        if (Input.GetKeyDown(KeyCode.P))
        {
            godMode = !godMode;
        }
        if (currentState == ThrowAnim || currentState == AttackAnim )
        {
            if (timeToEndAnimation <= 0 && !dialogue)
            {

                if (!isDashing && !shokyakuAttack && !desumiru)
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
            else if (timeToEndAnimation <= 0 && !dialogue && itaiken)
            {
                if (movementInput != Vector2.zero)
                {
                    CurrentState = ItaikenRunAnim;
                }
                else
                {
                    CurrentState = ItaikenIdleAnim;
                }
            }
        }
        else if (currentState.CanExitWhilePlaying || timeToEndAnimation <= 0)
        {
            if (!isDashing && !shokyakuAttack && !isSpeedingUp && !itaiken && !dialogue &&!desumiru)
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
            else if (!isDashing && !shokyaku && isSpeedingUp && !itaiken)
            {
                if (movementInput != Vector2.zero)
                {
                    CurrentState = MezameAnim;
                }
                else
                {
                    CurrentState = IdleAnim;
                }
            }
            else if (!isDashing && !shokyaku && !isSpeedingUp && !dialogue && itaiken && canMove)
            {
                if (movementInput != Vector2.zero)
                {
                    CurrentState = ItaikenRunAnim;
                }
                else
                {
                    CurrentState = ItaikenIdleAnim;
                }
            }
            if (!(itaiken && !canMove))
            {
                ChangeClip();
            }
       


        }

        //attack cooldwon
        if (attackCooldown > 0)
        { 
            attackCooldown -= Time.deltaTime;
        }
        projectileText.text = projectileNumber.ToString();

        //stamina
        staminaSlider.fillAmount = stamina/maxStamina;
        staminaText.text = ((int) stamina).ToSafeString();
        if (canMove && !isDashing && canAttack)
        {
            if (stamina < maxStamina)
            {
                stamina += staminaRegRate * Time.deltaTime;
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
 
        if (!Dialogue && !isHealing && cam.m_Lens.OrthographicSize <= startLensSize && cam.Follow == transform)
        {
            cam.m_Lens.OrthographicSize += 8*Time.deltaTime;
        }
        



        //speed
        if (isSpeedingUp)
        { 
            UseStamina(14* Time.deltaTime);

            if (stamina <= 0)
            {
                isSpeedingUp = false;
                canDash = true;
                canAttack = true;
                MoveSpeed = startSpeed;
                buttonUp = true;
            }
        }
        if (shokyaku && stopShokyaku)
        {
            movementInput = saveDirection;
            shokyaku = false;
            shokyakuAttack = false;
            canDash = true;
            canMove = true;
            canAttack = true;
            currentState = RunAnim;
            ChangeClip();
            stopShokyaku = false;
        }

        //Shokyaku
        if (shokyakuAttack)
        {
            canDash = false;
            canMove = false;
            canAttack = false;
            if (currentState != ShokyakuAnim)
            {
                
                currentState = ShokyakuAnim;
                ChangeClip();

            }
            ParticleManager.instance.UseParticle("Fire", projectileSpawnPoint.position, projectileSpawnPoint.rotation.eulerAngles);
            hits =Physics2D.OverlapAreaAll(new Vector2(projectileSpawnPoint.GetChild(0).position.x, projectileSpawnPoint.GetChild(0).position.y), new Vector2(projectileSpawnPoint.GetChild(1).position.x, projectileSpawnPoint.GetChild(1).position.y), enemyLayer);
            foreach(Collider2D enemy in hits)
            {
                if (enemy.GetComponent<EnemyHealth>())
                {
                    enemy.GetComponent<EnemyHealth>().enemyAddDamage(40 * Time.deltaTime, false, false);
                    if (!enemy.GetComponent<EnemyHealth>().isStuned)
                    {
                        StartCoroutine(enemy.GetComponent<EnemyHealth>().Stuned(false));
                    }
                }
            }
            UseStamina(10 * Time.deltaTime);
            if (mizuame <= 0)
            {
                hp.AddDamage(4 * Time.deltaTime);
            }
            shokyakuTimer -= Time.deltaTime;
            if (shokyakuTimer <= 0 || stamina < 1)
            {
                shokyakuAttack = false;
                shokyaku = false;
                movementInput = saveDirection;
                shokyaku = false;
                canDash = true;
                canMove = true;
                canAttack = true;
            }
            facingDirection = projectileSpawnPoint.position - transform.position;
        }

        //inventory
        if (inventoryOpen)
        {
            if (!shade.gameObject.activeInHierarchy )
            {
                 if (shade.transform.childCount - 2 != candy.Count)
                 {
                     foreach (Transform child in shade.transform)
                     {
                         if (child.name != "position" && !child.GetComponent<Text>())
                         { 
                             Destroy(child.gameObject);
                         }
                     }
                     if (candySpawned.Count > 0)
                     {
                         candySpawned.Clear();
                     }
                    shade.transform.rotation = quaternion.Euler(0, 0, 0);
                     float r = (shade.transform.GetChild(0).localPosition.y + shade.transform.position.y) / 2;
                     float maxcount = candy.Count;
                     float index = 0;
                     foreach (GameObject x in candy)
                     {
                         GameObject sqr = Instantiate(x, new Vector3(r * Mathf.Cos(((index / maxcount) * 360 * Mathf.Deg2Rad  + (90*Mathf.Deg2Rad))) + shade.transform.position.x, r * Mathf.Sin(((index / maxcount) * 360 * Mathf.Deg2Rad + (90 * Mathf.Deg2Rad))) + shade.transform.position.y, 0), Quaternion.Euler(0, 0, 0));
                         sqr.transform.localScale = shade.transform.localScale;
                         sqr.transform.SetParent(shade.transform);
                         sqr.GetComponent<candyScript>().id = (int)index;
                         sqr.GetComponent<candyScript>().angle = ((index / maxcount) * 360);
                         candySpawned.Add(sqr);
                         if (index == 0)
                         {
                             maxItemYPos = sqr.transform.position.y;
                         }
                         index++;
                     }
                 }
                shade.gameObject.SetActive(true);
            }
            if ((shade.GetComponent<Rigidbody2D>().angularVelocity > 0 && Input.GetAxis("Mouse ScrollWheel") < 0 || shade.GetComponent<Rigidbody2D>().angularVelocity < 0 && Input.GetAxis("Mouse ScrollWheel") > 0))
            {
                snap = false;
                shade.GetComponent<Rigidbody2D>().angularVelocity = 0;
                shade.GetComponent<Rigidbody2D>().AddTorque(Input.GetAxis("Mouse ScrollWheel") * 800 * Time.deltaTime, ForceMode2D.Impulse);
            }
            else if(Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                snap = false;
                shade.GetComponent<Rigidbody2D>().AddTorque(Input.GetAxis("Mouse ScrollWheel") * 800 * Time.deltaTime, ForceMode2D.Impulse);
            }
            if (Mathf.Abs(shade.GetComponent<Rigidbody2D>().angularVelocity) < 5f && !snap && itemsOnScreen.Count >0)
            {
                shade.GetComponent<Rigidbody2D>().angularVelocity = 0;
                Transform itemOnTop = null;
                GameObject[] itemsToRemove = itemsOnScreen.FindAll(x => x == null).ToArray();
                foreach (GameObject itemToRemove in itemsToRemove)
                {
                    itemsOnScreen.Remove(itemToRemove);
                }
                foreach (GameObject item in itemsOnScreen)
                {
                    if (item.GetComponent<Image>())
                    {
                        if (itemOnTop == null)
                        {
                            itemOnTop = item.transform;
                        }
                        else if (itemOnTop.position.y < item.transform.position.y)
                        {
                            itemOnTop = item.transform;
                        }

                    }                 
                }
                if (itemOnTop != null)
                {
                    int id = itemOnTop.GetComponent<candyScript>().id;
                    float angle = candySpawned[id].GetComponent<candyScript>().angle;
                    shade.transform.rotation = Quaternion.Euler(0, 0, -angle);
                }
                snap = true;
            }
            if (inventoryText.text != "Inventory Empty :(" && candySpawned.Count < 1)
            {
                inventoryText.text = "Inventory Empty :(";
            }
        }
        else
        {
            if (shade.gameObject.activeInHierarchy)
            {
                if (itemsOnScreen.Count > 2)
                {
                    foreach (GameObject item in candySpawned)
                    {
                        item.GetComponent<candyScript>().onScreen = false;
                        itemsOnScreen.Clear();
                    }
                }
                else
                {
                    if (itemsOnScreen.Count == 0 && candySpawned.Count >0)
                    {
                        shade.transform.rotation = Quaternion.Euler(0, 0, -candySpawned[UnityEngine.Random.Range(0, itemsOnScreen.Count)].GetComponent<candyScript>().angle);
                    }
                }
            }
            shade.gameObject.SetActive(false);
        }



        //power ups
        if (sakuramochi > 0)
        {
            sakuramochi -= Time.deltaTime;
        }
        else if (yokan > 0)
        {
            yokan -= Time.deltaTime;
        }
        else if (dango > 0)
        {
            dango -= Time.deltaTime;
        }
        else if (mizuame > 0)
        { 
            mizuame -= Time.deltaTime;
        }
        if (powerCoolDown > 0)
        { 
            powerCoolDown -= Time.deltaTime;
        }

        //desumiru
        myAnim.SetBool("Desumiru", desumiruPressed);

        if (Time.timeScale == 1 && Input.GetKeyDown(KeyCode.O))
        {
            Time.timeScale = 0.3f;
        }
        else if (Time.timeScale == 0.3f && Input.GetKeyDown(KeyCode.O))
        {
            Time.timeScale = 1f;
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
        else if (!canMove && !desumiru)
        {
            rb.AddForce(Vector2.zero);
        }
        else if (!canMove && desumiruPressed)
        {
            Vector2 moveForce = movementInput/4 * MoveSpeed/3 * Time.deltaTime;
            rb.AddForce(moveForce);

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

        if (dashCooldown < dashMaxCooldown)
        {
            t += Time.deltaTime / dushCooldownSpeed;

            dashCooldown = Mathf.Lerp(dashMinCooldown, dashMaxCooldown, t);
        }
        inventoryText.transform.rotation = Quaternion.Euler(0, 0, 0);
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
            //AkSoundEngine.PostEvent("Player_Dash", gameObject);
            UseStamina(10);
            canDash = false;            
            isDashing = true;
            CurrentState = DashAnim;
            rb.velocity = new Vector2(movementDirection.x * dashSpeed, movementDirection.y * dashSpeed);           
            yield return new WaitForSeconds(dashDuration);
            isDashing = false;
            rb.velocity = new Vector2(movementDirection.x * dashVelocityReset, movementDirection.y * dashVelocityReset);// <--- This line is necessary if you want to stop the player after initializing "Dash"
            t = 0.0f;
            dashCooldown -= dashCooldown;
            yield return new WaitUntil(() => dashCooldown >= dashMaxCooldown);
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
        if (canMove && !desumiru)
        {
            movementInput = movementValue.Get<Vector2>();

            if (movementInput != Vector2.zero)
            {
                facingDirection = movementInput;
            }
        }
        else if(!desumiruPressed && !canMove)
        {
            saveDirection = movementValue.Get<Vector2>();
        }
        if (desumiru && canAttack && !desumiruPressed)
        {
            StopDesumiru();
        }
        else if (desumiru && desumiruPressed)
        {
            movementInput = movementValue.Get<Vector2>()/4;
        }
    }

    //When Space was pressed
    public void OnDash(InputValue dashValue)
    {
        if (dashValue.Get<float>() == 1 && stamina >= 10)
        {
            StartCoroutine(Dash());
        }

    }

    //When RightMouseButton(RMB) was pressed
    public void OnAttack()
    {
        if (canAttack && !isHealing && !desumiru)
        {
            if (shokyaku)
            {
                movementInput = saveDirection;
                shokyaku = false;
                shokyakuAttack = false;
                canDash = true;
                canMove = true;
                canAttack = true;
                currentState = RunAnim;
                ChangeClip();
                stopShokyaku = false;
            }
            StopItaken();
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
                            enemy.gameObject.GetComponent<EnemyHealth>().enemyAddDamage(attackDamage, true, true);
                        }
                        if (enemy.gameObject.GetComponent<Rigidbody2D>() != null && enemy.GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static && enemy.gameObject.GetComponent<EnemyHealth>().canBeAttacked && enemy.gameObject.GetComponent<EnemyHealth>().canDoDmg)
                        {
                            enemy.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                            enemy.gameObject.GetComponent<Rigidbody2D>().AddForce(projectileSpawnPoint.right * pushForce, ForceMode2D.Impulse);
                            StartCoroutine(enemy.gameObject.GetComponent<EnemyHealth>().Stuned(true));
                        }
                    }
                }
                SaveMovement();
                facingDirection = projectileSpawnPoint.position - transform.position;
                attackCooldown = startAttackCooldown;
                CurrentState = AttackAnim;
                canMove = false;
                if (yokan <=0)
                {
                    UseStamina(5);
                }
            }
        }
        else if (canAttack && !isHealing && desumiru)
        {
            StopDesumiru();
        }
        else if (canAttack && !isHealing && itaiken)
        { 
            StopItaken();
        }

    }

    //When LeftMouseButton(LMB) was pressed
    public void OnFire(InputValue inputValue)
    {
        if (inputValue.Get<float>() == 1)
        {
            if (canAttack && !isHealing && !desumiru && !itaiken && !inventoryOpen & !shokyaku && !findDisplay.gameObject.activeInHierarchy)
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
                        if (yokan <= 0)
                        {
                            UseStamina(5);
                        }
                    }
                }
            }
            else if (desumiru && !inventoryOpen)
            {
                desumiruPressed = true;
            }
            else if (itaiken && canAttack && !inventoryOpen)
            {
                canMove = false;
                canAttack = false;
                CurrentState = ItaikenAttackAnim;
                ChangeClip();
            }
            else if (inventoryOpen && candySpawned.Count > 0 && powerCoolDown <= 0)
            {
                topItem.GetComponent<candyScript>().DoStuff();
                candy.Remove(candy.Find(x => x.name.ToString() + "(Clone)" == topItem.name.ToString()));
                itemsOnScreen.Remove(itemsOnScreen.Find(x => x.name.ToString() + "(Clone)" == topItem.name.ToString()));
                Destroy(topItem);
                foreach (Transform child in shade.transform)
                {
                    if (child.name != "position" && !child.GetComponent<Text>())
                    {
                        Destroy(child.gameObject);
                    }
                }
                inventoryOpen = false;
            }
            else if (canAttack && shokyaku)
            {
                shokyakuAttack = true;
            }
            else if (findDisplay.gameObject.activeInHierarchy)
            {
                skipPress = true;
            }
        }
        else if (inputValue.Get<float>() == 0)
        {
            desumiruPressed = false;
            if (desumiru)
            {
                StopDesumiru();
            }
            if (shokyakuAttack)
            {
                canAttack = true;
                canDash = true;
                canMove = true;
                shokyakuAttack = false;
            }
        }
    }

    public void OnHeal()
    {
        if (canAttack)
        {
            if (shokyaku)
            {
                stopShokyaku = true;
            }
            StopItaken();
            StopDesumiru();
            if (hp.playerCourrentHealth < hp.playerMaxHealth)
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
                        if (sakuramochi > 0)
                        {
                            hp.AddHealth(20);
                        }
                        else
                        {
                            hp.AddHealth(10);
                        }

                    }
                }
            }
        }
    }

    public void OnSpeedUp()
    {
        if (stamina > 0.1f && !buttonUp && canMove)
        {
            if (itaiken)
            {
                StopItaken();
            }
            isSpeedingUp = !isSpeedingUp;
            if (isSpeedingUp)
            {
                canDash = false;
                canAttack = false;
                MoveSpeed = startSpeed + startSpeed/3;
            }
            else
            {
                canDash = true;
                canAttack = true;
                MoveSpeed = startSpeed;
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
            if ((itaiken || desumiru) && canAttack)
            { 
                StopDesumiru();
                StopItaken();
            }
            else if (canAttack && !isHealing && canMove && !itaiken)
            {
                shokyakuTimer = 4;
                canDash = false;
                canMove = false;
                canAttack = false;
                SaveMovement();
                myAnim.Play(startShokyaku.name);
                currentClip = startShokyaku;
            }
        }
    }

    void OnInventory(InputValue inputValue)
    {
        if (inputValue.Get<float>() == 1)
        {
            inventoryOpen = true;
        }
        else
        {
            inventoryOpen = false;
        }
    }


    public void OnItaiken()
    {
        if (canAttack && !isHealing && stamina > 30 && !itaiken)
        {
            if (shokyaku)
            {
                stopShokyaku = true;
            }
            canAttack = false;
            canMove = false;
            myAnim.Play(startItaiken.name);
        }
        else if (itaiken)
        {
            StopItaken();
        }
        
    }
    void StopItaken()
    {
        if (itaiken)
        {
            itaiken = false;
        }

    }

    public void SpawnItaiken(bool stopAfter)
    {
        if (hp.playerCourrentHealth > 5)
        {
            Instantiate(itaikenToSpawn, transform.position, Quaternion.Euler(projectileRotation.transform.eulerAngles.x, projectileRotation.transform.eulerAngles.y, projectileRotation.transform.eulerAngles.z - 90));
            canAttack = true;
            facingDirection = projectileSpawnPoint.position - transform.position;
            ChangeClip();
            if (stopAfter)
            {
                UseStamina(15);
                hp.AddDamage(10);
                StopItaken();
            }
            else
            {
                //UseStamina(20);
                hp.AddDamage(5);
            }
        }
        else
        {
            StopItaken();
        }
    }

    public void OnDesumiru()
    {
        if (canAttack && !desumiru && !itaiken)
        {
            if (shokyaku)
            {
                stopShokyaku = true;
            }
            desumiru = true;
            canDash = false;
            canMove = false;
            canAttack = false;
            myAnim.Play(desumiruAnimations[0].name);
            movementInput = Vector2.zero;
        }
        else if (itaiken)
        {
            StopItaken();
        }
    }


    void DesumiruUse()
    {
        canAttack = false;
        slowWaitingTime = 0.05f;        
    }

    public void StopDesumiru()
    {
        if (desumiru)
        {
            if (myAnim.GetComponent<AnimationToCode>().slowing != null)
            {
                StopCoroutine(myAnim.GetComponent<AnimationToCode>().slowing);
            }
            StopCoroutine(TimeToNormal());
            StartCoroutine(TimeToNormal());
            desumiru = false;
            canDash = true;
            canMove = true;
            canAttack = true;
            desumiruPressed = false;
            slowWaitingTime = 0.01f;
            myAnim.Play(currentClip.name);
            ChangeClip();
        }
    }

    public void DesumiruAttack(bool right)
    {
        if (myAnim.GetComponent<AnimationToCode>().slowing != null)
        {
            StopCoroutine(myAnim.GetComponent<AnimationToCode>().slowing);
            StopCoroutine(TimeToNormal());
            StartCoroutine(TimeToNormal());
        }
        if (desumiruAttackCorutine == null)
        {
            desumiruAttackCorutine = StartCoroutine(DesumiruAttackUse(right));
        }
        else
        {
            StopCoroutine(desumiruAttackCorutine);
            desumiruAttackCorutine = StartCoroutine(DesumiruAttackUse(right));
        }
        
    }

    public void StartHealing()
    {
        StartCoroutine(ChangeCamSizeUp());
    }

    public void StopHealing()
    {
        if (isHealing)
        {
            isHealing = false;
            canHeal = false;
            movementInput = saveDirection;
            canMove = true;
        }
    }

    public void UseStamina(float staminaToUse)
    {
        if (!godMode)
        {
            if (mizuame <= 0)
            {
                if (yokan > 0)
                {
                    stamina -= staminaToUse / 2;
                }
                else
                {
                    stamina -= staminaToUse;
                }
            }
        }
    }

    public void MakeDeath()
    {
        canMove = false;
        canAttack = false;
        canDash = false;
        canHeal = false;
        myAnim.Play(deathAnim.name);
        StopAllCoroutines();
        this.enabled = false;
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
    public void AddObjectOnScreen(GameObject objectToAdd)
    {
        itemsOnScreen.Add(objectToAdd);
    }
    public void RemoveObjectOnScreen(GameObject objectToRemove)
    {
        if (itemsOnScreen.Find(x => x.gameObject == objectToRemove))
        {
            itemsOnScreen.Remove(objectToRemove);
        }
    }
    public void CandyText(string text, GameObject itemOnTop)
    { 
        inventoryText.text = text;
        topItem = itemOnTop;
    }


    public void MoveToPlace(Vector3 point)
    {
        if (!dialogue && (transform.position - point).magnitude < 10f)
        {
            StartCoroutine(MoveNow(point));
        }
    }



    //things to find
    public void AddFind(Find thatToAdd)
    { 
        thingsToFind.Add(thatToAdd);
        StartCoroutine(DisplayText(thatToAdd.dialogue,thatToAdd));
        CurrentState = IdleAnim;
        canMove = true;
    }

    private IEnumerator DisplayText(string text, Find find)
    {
        skipPress = false;
        yield return new WaitForSeconds(0.2f);
        canAttack = false;
        findDisplay.gameObject.SetActive(true);
        findDisplay.transform.GetChild(0).gameObject.SetActive(true);
        findDisplay.transform.GetChild(1).gameObject.SetActive(false);
        Text textObject = findDisplay.transform.GetChild(0).GetComponentInChildren<Text>();
        textObject.text = string.Empty;
        foreach (char letter in text.ToCharArray())
        {
            if (skipPress)
            {
                break;
            }
            textObject.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        if (skipPress)
        {
            textObject.text = find.description;
        }
        skipPress = false;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));
        findDisplay.gameObject.SetActive(false);
        findDisplay.transform.GetChild(0).gameObject.SetActive(false);
        findDisplay.transform.GetChild(1).gameObject.SetActive(false);
        StartCoroutine(DisplayFind(find));
    }

    private IEnumerator DisplayFind(Find find)
    {
        skipPress = false;
        yield return new WaitForSeconds(0.1f);
        findDisplay.gameObject.SetActive(true);
        findDisplay.transform.GetChild(1).gameObject.SetActive(true);
        findDisplay.transform.GetChild(0).gameObject.SetActive(false);
        Text textObject = findDisplay.transform.GetChild(1).GetComponentInChildren<Text>();
        textObject.text = string.Empty;
        textObject.gameObject.GetComponentInChildren<Image>().sprite = find.sprite;
        foreach (char letter in find.description.ToCharArray())
        {
            if (skipPress)
            {
                break;
            }
            textObject.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        if (skipPress)
        {
            textObject.text = find.description;
        }
        skipPress = false;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));
        findDisplay.gameObject.SetActive(false);
        findDisplay.transform.GetChild(0).gameObject.SetActive(false);
        findDisplay.transform.GetChild(1).gameObject.SetActive(false);
        canAttack = true;
    }

    private IEnumerator MoveNow(Vector3 point)
    {
        Dialogue = true;
        while ((transform.position - point).magnitude >0.1f)
        {
            facingDirection = -(transform.position - point).normalized;
            TryMove(-(transform.position - point).normalized);
            yield return new WaitForEndOfFrame();
        }
        if (itaiken)
        {
            CurrentState = ItaikenIdleAnim;
        }
        else
        {
            CurrentState = IdleAnim;
        }

    }
    private IEnumerator ChangeCamSizeUp()
    {
        if (cam.Follow == transform)
        {
            if (isHealing)
            {
                myAnim.Play(startHeal.name);
                currentClip = startHeal;
                while (cam.m_Lens.OrthographicSize >= changeLensSize)
                {
                    cam.m_Lens.OrthographicSize -= Time.deltaTime *4;
                    yield return new WaitForEndOfFrame();
                }
            }
            else if (dialogue)
            {
                while (cam.m_Lens.OrthographicSize >= changeLensSize * 1.5f)
                {
                    cam.m_Lens.OrthographicSize -= Time.deltaTime * 4;
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        else
        {
            myAnim.Play(startHeal.name);
            currentClip = startHeal;
        }
    }


    public IEnumerator SlowTime()
    {
        StopCoroutine(TimeToNormal());
        slowWaitingTime = 0.4f;
        if (Time.timeScale > 0.4f)
        {
            while (Time.timeScale > 0.4f)
            {
                Time.timeScale -= Time.deltaTime * 4;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            Debug.LogError("to fast use");
        }
        if (desumiruPressed)
        {
            if (stamina >= 12)
            {
                canAttack = false;
                DesumiruUse();
            }
            else
            {
                StopDesumiru();
            }
        }
        else
        {
            while (slowWaitingTime > 0 && !desumiruPressed)
            {
                if (desumiruAttackCorutine == null)
                {
                    canAttack = true;
                }

                slowWaitingTime -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            if (slowWaitingTime <= 0 && !desumiruPressed)
            {
                StopDesumiru();
            }
        }
        StopCoroutine(TimeToNormal());
        StartCoroutine(TimeToNormal());
    }

    private IEnumerator TimeToNormal()
    {
        if (Time.timeScale < 1)
        {
            while (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime *4;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private IEnumerator DesumiruAttackUse(bool right)
    {
        canAttack = false;
        List<Vector2> vectors = new List<Vector2>();
        float animSpeed = myAnim.GetCurrentAnimatorStateInfo(0).speed;
        test = myAnim.GetCurrentAnimatorStateInfo(0).length * animSpeed;
        float starAnimTime = myAnim.GetCurrentAnimatorStateInfo(0).length * animSpeed;
        vectors.Add(Vector2.zero);
        vectors.Add(Vector2.zero);
        vectors[1] = point2;
        while (test > 0)
        {
            if (!myAnim.GetComponent<EdgeCollider2D>().enabled)
            {
                myAnim.GetComponent<EdgeCollider2D>().enabled = true;
            }
            if (right)
            {
                point2 = new Vector2(desumiuRadius * Mathf.Cos((((starAnimTime - test)/starAnimTime) * 360 * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)), desumiuRadius * Mathf.Sin((((starAnimTime - test) / starAnimTime) * 360 * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)));
            }
            else
            {
                point2 = new Vector2(desumiuRadius * Mathf.Cos((( test/starAnimTime) * 360 * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)), desumiuRadius * Mathf.Sin(((test/starAnimTime) * 360 * Mathf.Deg2Rad) - (Mathf.Deg2Rad * 90)));
            }
            test -= Time.deltaTime * animSpeed * 1.6f;
            vectors[1] = point2;
            myAnim.GetComponent<EdgeCollider2D>().SetPoints(vectors);
            yield return new WaitForEndOfFrame();
        }
        myAnim.GetComponent<EdgeCollider2D>().enabled = false;
        canAttack = true;
        desumiruAttackCorutine = null;
    }


    public float GetShadePos()
    {
        return maxItemYPos;
    }
    public GameObject GetPlayer()
    {
        return gameObject;
    }

    public GameObject GetHead()
    { 
        return myAnim.transform.GetChild(0).gameObject;
    }
}
