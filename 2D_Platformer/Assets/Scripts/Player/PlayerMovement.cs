using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement
{
    private readonly Player_Input playerInput;
    private readonly Transform transformToMove;
    private readonly PlayerMovementSettings playerSettings;
    private readonly Controller_2D controller;
    private readonly PlayerDash playerDash;

    private readonly float gravity;

    private readonly float maxJumpVelocity;
    private readonly float minJumpVelocity;

    private float wallDirX;

    private Vector2 velocity;
    private float velocityXSmoothing;
    private float velocityYSmoothing;
    public bool isDead;

    public bool WallSliding { get; private set; }
    public Vector2 Velocity { get => velocity; set => velocity = value; }

    public bool IsAttacking { get; set; }
    public bool IsSwinging { get; set; }
    public Vector2 RopeHook { get; set; }

    public PlayerMovement(Transform transformToMove, PlayerMovementSettings playerSettings)
    {
        this.playerInput = transformToMove.GetComponent<Player_Input>();
        this.transformToMove = transformToMove;
        controller = transformToMove.GetComponent<Controller_2D>();
        playerDash = transformToMove.GetComponent<PlayerDash>();
        this.playerSettings = playerSettings;
        playerInput.OnJumpDown += OnJumpInputDown;
        playerInput.OnJumpUp += OnJumpInputUp;

        gravity = -(2 * playerSettings.MaxJumpHeight) / Mathf.Pow(playerSettings.TimeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * playerSettings.TimeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * playerSettings.MinJumpHeight);
    }

    public void Movement()
    {
        if (isDead)
        {
            velocity.x = 0;
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime, new Vector2(-1, playerInput.directionalInput.y));
            return;
        }
        
        SetPlayerOrientation(playerInput.directionalInput);
        if (!IsSwinging && !playerInput.attacking)
        {
            CalculateVelocity(velocityXSmoothing);
        }
        if (IsSwinging)
        {
            Swing();
        }
        HandleWallSliding(velocityXSmoothing);
        HandleDash();
        if(!playerInput.attacking) controller.Move(velocity * Time.deltaTime, new Vector2(-1, playerInput.directionalInput.y));
        HandleMaxSlope();
    }

    public void Knockback(Vector3 dir, float kockbackDistance)
    {
        velocity = Vector3.zero;
        velocity.x += dir.x * kockbackDistance;
        velocity.y += dir.y * kockbackDistance;
        //controller.Move(velocity * Time.deltaTime, new Vector2(-1, playerInput.directionalInput.y));
    }

    void HandleJump()
    {
        if (playerInput.jumping)
        {
            OnJumpInputDown();
        }
        else if(!playerInput.jumping && !IsSwinging)
        {
            OnJumpInputUp();
        }
    }

    void HandleDash()
    {
        if (playerInput.dashing)
        {
            playerDash.OnDashInput();
        }
        playerDash.airborne = (!controller.collitions.below && !WallSliding);
        playerDash.DashController(ref velocity, playerInput, playerSettings);
    }

    public void CalculateVelocity(float velocityXSmoothing)
    {
        float targetVelocityX = playerSettings.MoveSpeed * playerInput.directionalInput.x;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collitions.below ? playerSettings.AccelerationTimeGrounded : playerSettings.AccelerationTimeAirborne));
        if (playerDash.dashHover)
        {

            velocity.y = 0;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

    }

    void HandleMaxSlope()
    {
        if (controller.collitions.above || controller.collitions.below)
        {
            if (controller.collitions.slidingDownMaxSlope)
            {
                velocity.y += controller.collitions.slopeNormal.y * gravity * -1 * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
    }

    void HandleWallSliding(float velocityXSmoothing)
    {
        wallDirX = (controller.collitions.left) ? -1 : 1;
        WallSliding = false;
        if ((controller.collitions.left || controller.collitions.right) && !controller.collitions.below && velocity.y < 0)
        {
            WallSliding = true;

            if (velocity.y < playerSettings.WallSlideSpeedMax)
            {
                velocity.y = -playerSettings.WallSlideSpeedMax;
            }
            if (playerSettings.TimeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (playerInput.directionalInput.x != wallDirX && playerInput.directionalInput.x != 0)
                {
                    playerSettings.TimeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    playerSettings.TimeToWallUnstick = playerSettings.WallStickTime;
                }
            }
            else
            {
                playerSettings.TimeToWallUnstick = playerSettings.WallStickTime;
            }
        }
    }

    public void OnJumpInputDown()
    {
        if (WallSliding)
        {
            if (wallDirX == playerInput.directionalInput.x)
            {
                velocity.x = -wallDirX * playerSettings.WallJumpclimb.x;
                velocity.y = playerSettings.WallJumpclimb.y;
            }
            else if (playerInput.directionalInput.x == 0)
            {
                velocity.x = -wallDirX * playerSettings.WallJumpOff.x;
                velocity.y = playerSettings.WallJumpOff.y;
            }
            else
            {
                velocity.x = -wallDirX * playerSettings.WallLeap.x;
                velocity.y = playerSettings.WallLeap.y;
            }
        }

        if (controller.collitions.below)
        {
            if (controller.collitions.slidingDownMaxSlope)
            {
                if (playerInput.directionalInput.x != -Mathf.Sign(controller.collitions.slopeNormal.x))
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

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = 0;
        }
    }

    public void SetPlayerOrientation(Vector2 input)
    {
        if (!playerDash.dashHover)
        {
            playerInput.directionalInput = input;
            if (playerInput.directionalInput.x < 0)
            {
                transformToMove.localScale = new Vector3(-1, 1, -1);
                playerDash.AfterImage.transform.localScale = new Vector3(-2, 2, -1);
            }
            else if (playerInput.directionalInput.x > 0)
            {
                transformToMove.localScale = new Vector3(1, 1, -1);
                playerDash.AfterImage.transform.localScale = new Vector3(2, 2, -1);
            }
        }
    }

    public void Swing()
    {
        var playerToHookDirection = (RopeHook - (Vector2)transformToMove.position).normalized;
        var pullforce = playerToHookDirection * playerSettings.SwingForce;
        AddForce(pullforce);
    }

    public void AddForce(Vector2 force)
    {
        velocity.x = Mathf.SmoothDamp(velocity.x, force.x, ref velocityXSmoothing, playerSettings.AccelerationTimeSwing);
        velocity.y = Mathf.SmoothDamp(velocity.y, force.y, ref velocityYSmoothing, playerSettings.AccelerationTimeSwing);
    }

}
