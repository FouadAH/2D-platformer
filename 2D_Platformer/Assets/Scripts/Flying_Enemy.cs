using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Controller_2D))]
public class Flying_Enemy : MonoBehaviour
{

    Controller_2D controller;

    Player player;
    Transform playerTransfrom;
    public LayerMask viewMask;

    public float lookRadius;
    float distance;

    public Transform pathHolder;
    Vector3[] waypoints;

    public float waitTime;

    float Health;
    public float maxHealth;

    [HideInInspector]
    public Vector3 velocity;

    public Vector2 knockback;

    float velocityXSmoothing;
    float velocityYSmoothing;
    
    public float accelerationTime = .2f;

    public float moveSpeed = 5;
    
    float directionX = 1;
    float directionY = 1;
    float playerX;
    float playerY;

    GameManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }
    void Start()
    {
        sprite = GetComponent<Renderer>();
        player = gm.player.GetComponent<Player>();
        playerTransfrom = player.transform;
        Health = maxHealth;
        controller = GetComponent<Controller_2D>();
        waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < pathHolder.childCount; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
        }
        StartCoroutine(AutoMove());

    }

    private void Update()
    {
        distance = Vector2.Distance(playerTransfrom.position, transform.position);
        playerX = (transform.position.x < playerTransfrom.position.x) ? 1 : -1;
        playerY = (transform.position.y < playerTransfrom.position.y) ? 1 : -1;

        if (CanSeePlayer())
        {
            isPaused = true;
            Aggro();
        }
        else
        {
            isPaused = false;
        }

        CalculateVelocity();
        controller.Move(velocity * Time.deltaTime);
        pathHolder.position = new Vector3(pathHolder.position.x, transform.position.y, 0);
    }

    IEnumerator AutoMove()
    {
        while (true)
        {
            if (isPaused)
            {
                yield return null;
            }
            if (controller.collitions.above || controller.collitions.below )
            {
                directionY *= -1;
                velocity.y = 0;
            }
            if ( controller.collitions.left || controller.collitions.right)
            {
                directionX *= -1;
                velocity.x = 0;
            }
            yield return null;
        }
    }

    bool isPaused = false;
    int targetWayPointIndex = 0;
    IEnumerator FollowPath(Vector3[] waypoints)
    {
        Vector3 targetWaypoint = waypoints[targetWayPointIndex];
        directionX = (transform.position.x < targetWaypoint.x) ? 1 : -1;

        while (true)
        {
            if (isPaused)
            {
                yield return null;
            }
            else if ((transform.position.x >= targetWaypoint.x && directionX == 1) || (transform.position.x <= targetWaypoint.x && directionX == -1))
            {
                velocity.x = 0;
                targetWayPointIndex = (targetWayPointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWayPointIndex];
                directionX = (transform.position.x < targetWaypoint.x) ? 1 : -1;
                yield return new WaitForSeconds(waitTime);
            }
            yield return null;

        }
    }

    void Aggro()
    {
        directionX = playerX;
        directionY = playerY;
    }

    bool CanSeePlayer()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, playerTransfrom.position, viewMask);
        if (distance < lookRadius && !hit)
        {
            return true;
        }
        return false;
    }

    void CalculateVelocity()
    {
        float targetVelocityX = moveSpeed * directionX;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime);
        float targetVelocityY = moveSpeed * directionY;
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, accelerationTime);
    }


    Renderer sprite;
    Color colorStart = Color.red;
    Color colorEnd = Color.white;
    float duration = .05F;
    public float damageDealt;

    void Damage(int dmg)
    {
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            Aggro();
            velocity += 20 * Vector3.Normalize(transform.position - player.transform.position);
            Health -= dmg;
        }

        float lerp = Mathf.PingPong(Time.time, duration) / duration;
        sprite.material.color = Color.Lerp(colorStart, colorEnd, lerp);

    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        float size = .3f;

        Gizmos.color = new Color(255, 0, 0);
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawLine(waypoint.position - Vector3.up * size, waypoint.position + Vector3.up * size);
            Gizmos.DrawLine(waypoint.position - Vector3.left * size, waypoint.position + Vector3.left * size);
            previousPosition = waypoint.position;
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lookRadius);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger != true && collision.tag == "Player")
        {
            player.DealDamage(damageDealt, Vector3.Normalize(player.transform.position - transform.position));
        }
    }
}
