using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBehaviour : StateMachineBehaviour
{
    Controller_2D controller;
    Player player;
    Transform playerTransfrom;
    GameObject enemyGameObject;
    BaseEnemy enemy;
    GameObject enemyHitBox;
    SpriteRenderer sprite;

    [HideInInspector]
    public Vector3 velocity;

    RaycastHit2D hit;

    float velocityXSmoothing;
    
    public float accelerationTimeGrounded = .2f;

    float directionX = 1;

    public float moveSpeed = 5;
    public float gravity = -10;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyGameObject = animator.gameObject;
        player = enemyGameObject.GetComponent<BaseEnemy>().gm.player.GetComponent<Player>();
        playerTransfrom = player.transform;
        enemy = enemyGameObject.GetComponent<BaseEnemy>();
        enemyHitBox = enemyGameObject.transform.GetChild(0).gameObject;
        controller = enemyGameObject.GetComponent<Controller_2D>();
        sprite = enemyGameObject.GetComponent<SpriteRenderer>();
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hit = Physics2D.Raycast(animator.transform.position + Vector3.right / 2f * directionX, Vector3.down, .5f + animator.transform.parent.localScale.y, controller.collitionMask);
        directionX = (animator.transform.position.x < playerTransfrom.position.x) ? 1 : -1;
        Flip();
        CalculateVelocity();
        controller.Move(velocity * Time.deltaTime);
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    void CalculateVelocity()
    {
        velocity.y += gravity * Time.deltaTime;
        if ((!hit && !controller.collitions.desendingSlope) || controller.collitions.right || controller.collitions.left)
        {
            velocity.x = 0;
            enemy.isAggro = false;
        }
        else
        {
            float targetVelocity = moveSpeed * directionX;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity, ref velocityXSmoothing, accelerationTimeGrounded);
        }
        
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
