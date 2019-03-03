using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Controller_2D))]

public class Player : MonoBehaviour {

    Controller_2D controller;

    [HideInInspector]
    public Vector3 velocity;
    private float velocityXSmoothing;

    public Vector2 wallJumpclimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public Vector2 playerPosition;

    bool wallSliding;
    int wallDirX;
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = 1f;
    float timeToWallUnstick;

    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;

    public float maxJumpHeight = 4f;
    public float minJumpHeight = .5f;
    public float timeToJumpApex = .4f;

    public float moveSpeed = 6;

    float gravity;

    float maxJumpVelocity;
    float minJumpVelocity;
    private bool dashLock = false;
    bool dashHover = false;
    public float dashFactor = 10;

    public int damage;
    public  float attackCooldown;
    public LayerMask enemyMask;
    public Vector2 swordKnockback;
    public Vector2 facingDir;

    float iFrames = 0f;
    float iFrameTime = 1f;
    bool invinsible = false;
    
    Vector2 directionalInput;

    SpriteRenderer sprite;
    Color colorStart = Color.white;
    Color colorEnd = Color.red;

    Animator anim;

    public bool CanAttack;
    public bool CanMove;

    public float health;
    public float maxHealth;

    public float aggroRange;
    public int currency;

    [SerializeField] private GameSceneManager gm;
    public float dashCooldown = .3f;
    private bool canDash = false;
    private float targetVelocityX;
    private bool airborne = false;
    private bool dashedInAir;
    private bool hasLanded = false;
    private bool takingInput;

    private void Awake()
    {
        
    }
    void Start () {

        gm = FindObjectOfType<GameSceneManager>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller_2D>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SceneManager.LoadScene(2);
        }
        airborne = (!controller.collitions.below && !wallSliding);
        OnDamage();
        Aggro();
        DashController();
        CalculateVelocity();
        HandleWallSliding();
        LerpColor();
        
        controller.Move(velocity * Time.deltaTime, new Vector2(-1, directionalInput.y));
        
        if (controller.collitions.above || controller.collitions.below)
        {
            if(controller.collitions.slidingDownMaxSlope)
            {
                velocity.y += controller.collitions.slopeNormal.y*gravity*-1*Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
    }

    public void Aggro()
    {
        Collider2D[] enemiesToAggro = Physics2D.OverlapCircleAll(transform.position, aggroRange, enemyMask);
        for (int i = 0; i < enemiesToAggro.Length; i++)
        {
            enemiesToAggro[i].GetComponent<BaseEnemy>().isAggro = true;
        }

    }

    void LerpColor()
    {
        if (invinsible)
        {
            sprite.material.color = Color.Lerp(colorStart, colorEnd, Mathf.PingPong(.5f, 1));
        }
        else
        {
            sprite.material.color = Color.white;
        }
    }

    public void SetDirectionalnput(Vector2 input)
    {
        if (!dashHover)
        {
            directionalInput = input;
            if(directionalInput.x < 0)
            {
                transform.localScale = new Vector2(-1, 1);
            }
            else if (directionalInput.x > 0)
            {
                transform.localScale = new Vector2(1, 1);
            }
        }

    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    public void OnJumpInputDown()
    {
        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpclimb.x;
                velocity.y = wallJumpclimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
        }

        if (controller.collitions.below)
        {
            if (controller.collitions.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(controller.collitions.slopeNormal.x))
                {
                    velocity.y = maxJumpVelocity * controller.collitions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collitions.slopeNormal.x;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
    }
    
    public void OnDashInput()
    {
        if (!dashLock)
        {
            canDash = true;
        }
    }

    public bool DashController()
    {
        if (canDash)
        {
            if (velocity.y > 0)
            {
                velocity.y = 0;
            }

            StartCoroutine(DashLockout());

            if (directionalInput.x == 0)
            {
                velocity.x = transform.localScale.x * moveSpeed * dashFactor;
            }
            else
            {
                velocity.x = ((Mathf.Sign(directionalInput.x) == -1) ? -1 : 1) * moveSpeed * dashFactor;
            }
        }
        canDash = false;
        
        return dashHover;
    }

    public IEnumerator DashLockout()
    {
        dashLock = true;
        dashHover = true;
        yield return new WaitForSeconds(dashCooldown);
        dashHover = false;
        yield return new WaitWhile(() => airborne);
        dashLock = false;
    }
    
    public IEnumerator SwordAttack()
    {
        anim.SetTrigger("isAttacking");
        yield return new WaitForSeconds(attackCooldown);
        CanAttack = true;
    }
    

    void CalculateVelocity()
    {
        float targetVelocityX = moveSpeed * directionalInput.x;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collitions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
        if (!dashHover)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = 0;
        }
        
    }

    void HandleWallSliding()
    {
        wallDirX = (controller.collitions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collitions.left || controller.collitions.right) && !controller.collitions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }
            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
        anim.SetBool("isWallSliding", wallSliding);
    }
    
    public void OnDamage()
    {
        if (invinsible)
        {
            if (iFrames > 0)
            {
                iFrames -= Time.deltaTime;
            }   
            else
            {
                invinsible = false;
            }
        }
    }

    public void Knockback(Vector3 dir, Vector2 kockbackDistance)
    {
        if (!invinsible)
        {
            velocity = Vector3.zero;
            velocity.x += dir.x * kockbackDistance.x;
            velocity.y += dir.y * kockbackDistance.y;
        }
        
    }

    public void DealDamage(float damage)
    {
        if (!invinsible)
        {
            iFrames = iFrameTime;
            invinsible = true;
            gm.health -= damage;
            CheckDeath();
        }
        
    }

    private void CheckDeath()
    {
        if (gm.health <= 0)
        {
            gm.health = gm.maxHealth;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
