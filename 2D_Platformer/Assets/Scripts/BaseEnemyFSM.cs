using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyFSM : StateMachineBehaviour
{
    Controller_2D controller;
    Player player;
    Transform playerTransfrom;
    GameObject enemy;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = FindObjectOfType<GameSceneManager>().player.GetComponent<Player>();
        playerTransfrom = player.transform;
        enemy = animator.gameObject;
        controller = enemy.GetComponent<Controller_2D>();
    }
    
}
