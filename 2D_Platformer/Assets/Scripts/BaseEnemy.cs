using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(Controller_2D))]
public class BaseEnemy : MonoBehaviour
{

    Controller_2D controller;

    Player player;
    Transform playerTransfrom;
    public LayerMask viewMask;

    private Vector2 attackPos;
    public LayerMask playerMask;

    public float lookRadius;
    public float attackRange;

    public float waitTime;

    float Health;
    public float maxHealth;

    public Vector2 knockback;

    [HideInInspector]
    public Vector3 velocity;
    
    public float accelerationTimeGrounded = .2f;

    public float moveSpeed = 5;

    public float gravity = -10;
    public float viewDistance = 5f;
    RaycastHit2D hit;

    SpriteRenderer sprite;
    Color colorStart = Color.white;
    Color colorEnd = Color.red;
    public float damageDealt;

    Animator anim;

    public Canvas canvas;
    public Slider healthSlider;

    public bool isAggro = false;
    public bool inRange = false;
    private readonly float flashTime = 0.3f;

    public int coinDrop;
    public GameObject coinPrefab;

    public GameSceneManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameSceneManager>();
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        player = gm.player.GetComponent<Player>();
        playerTransfrom = player.transform;
        Health = maxHealth;
        controller = GetComponent<Controller_2D>();
        healthSlider.value = CalculateHealthPercent();
        canvas.enabled = false;
    }

    private void Update()
    {
        InAttackRange();
        if (isAggro)
        {
            anim.SetBool("isChasing", true);
            anim.SetBool("isPatroling", false);
        }
        else
        {
            anim.SetBool("isChasing", false);
            anim.SetBool("isPatroling", true);
        }
        if (inRange)
        {
            anim.SetBool("Attack",true);
        }
        else
        {
            anim.SetBool("Attack", false);
        }

    }

    public void CoinSpawner()
    {
        for (int i = 0; i < coinDrop; i++)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
    }

    public void InAttackRange()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);
        inRange = (playerCollider != null);
    }

    public void TakeDamage(int dmg)
    {
        canvas.enabled = true;
        isAggro = true; 
        velocity += 20 * Vector3.Normalize(transform.position - player.transform.position);
        Health -= dmg;
        healthSlider.value = CalculateHealthPercent();
        StartCoroutine(Flash());
        sprite.material.color = Color.Lerp(colorStart, colorEnd, Mathf.PingPong(.5f, 1));
        if (Health <= 0)
        {
            CoinSpawner();
            Destroy(gameObject);
        }
    }

    public IEnumerator Flash()
    {
        sprite.material.color = colorEnd;
        yield return new WaitForSeconds(flashTime);
        sprite.material.color = colorStart;
    }
    private float CalculateHealthPercent()
    {
        return Health / maxHealth;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger != true && collision.tag == "Player")
        {
            player.Knockback(Vector3.Normalize(player.transform.position - transform.position), knockback);
            player.DealDamage(damageDealt);

        }
    }
}
