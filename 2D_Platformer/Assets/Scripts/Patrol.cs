using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : StateMachineBehaviour
{
    Controller_2D controller;
    Player player;
    Transform playerTransfrom;
    GameObject enemy;
    GameObject enemyHitBox;
    SpriteRenderer sprite;
    RaycastHit2D hit;
    public float gravity = -10;

    [HideInInspector]
    public Vector3 velocity;

    float velocityXSmoothing;

    public float accelerationTimeGrounded = .2f;

    float directionX = 1;

    public float moveSpeed = 5;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.gameObject.GetComponent<BaseEnemy>().gm.player.GetComponent<Player>();
        playerTransfrom = player.transform;
        enemy = animator.gameObject;
        enemyHitBox = enemy.transform.GetChild(0).gameObject;
        controller = enemy.GetComponent<Controller_2D>();
        sprite = enemy.GetComponent<SpriteRenderer>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hit = Physics2D.Raycast(animator.transform.position + Vector3.right / 2f * directionX, Vector3.down, .5f + animator.transform.localScale.y, controller.collitionMask);
        Flip();
        CalculateVelocity();
        controller.Move(velocity * Time.deltaTime);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    
    public void CalculateVelocity()
{
        if (!hit && !controller.collitions.desendingSlope)
        {
            velocity.x = 0;
            directionX *= -1;
        }
        else if (controller.collitions.right)
        {
            velocity.x = 0;
            directionX = -1;
        }
        else if (controller.collitions.left)
        {
            velocity.x = 0;
            directionX = 1;
        }
        float targetVelocity = moveSpeed * directionX;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity, ref velocityXSmoothing,  accelerationTimeGrounded);
        velocity.y += gravity * Time.deltaTime;

    }

    public void Flip()
    {
        if (directionX == -1)
        {
            enemy.transform.localScale = new Vector2(-1, 1);
        }
        else if (directionX == 1)
        {
            enemy.transform.localScale = new Vector2(1, 1);
        }
    }



}
