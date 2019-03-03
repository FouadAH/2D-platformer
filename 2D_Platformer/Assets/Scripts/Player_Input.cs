using UnityEngine;
using UnityEngine.Experimental.Input;

[RequireComponent (typeof (Player))]
public class Player_Input : MonoBehaviour
{
    Player player;
    Animator animator;
    public InputMaster controls;

    private Vector2 directionalInput;

    private bool attackDown;
    private bool jumpDown;
    private bool dashDown;

    public void OnEnable()
    {
        controls.Player.Attack.performed += OnAttack;
        controls.Player.Attack.Enable();

        controls.Player.Movement.performed += OnMovement;
        controls.Player.Movement.cancelled += OnMovement;
        controls.Player.Movement.Enable();

        controls.Player.Jump.started += OnJumpStarted;
        controls.Player.Jump.cancelled += OnJumpCancelled;
        controls.Player.Jump.Enable();

        controls.Player.Dash.performed += OnDash;
        controls.Player.Dash.Enable();

    }
    
    public void OnDisable()
    {
        controls.Player.Attack.performed -= OnAttack;
        controls.Player.Attack.Disable();

        controls.Player.Movement.performed -= OnMovement;
        controls.Player.Movement.cancelled -= OnMovement;
        controls.Player.Movement.Disable();

        controls.Player.Jump.started -= OnJumpStarted;
        controls.Player.Jump.cancelled -= OnJumpCancelled;
        controls.Player.Jump.Disable();

        controls.Player.Dash.performed -= OnDash;
        controls.Player.Dash.Disable();
    }

    public void OnJumpStarted(InputAction.CallbackContext context)
    {
        player.OnJumpInputDown();
        animator.SetBool("isJumping", true);
    }

    public void OnJumpCancelled(InputAction.CallbackContext obj)
    {
        player.OnJumpInputUp();
        animator.SetBool("isJumping", false);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        directionalInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        StartCoroutine(player.SwordAttack());
        player.CanMove = false;
    }
    
    public void OnDash(InputAction.CallbackContext context)
    {
        player.OnDashInput();
    }

    void Start()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
	}
	
	void Update()
    {
        player.SetDirectionalnput(directionalInput);
        animator.SetFloat("Speed", Mathf.Abs(directionalInput.x));
        
        if (player.velocity.y < 0)
        {
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
        }
        if (player.velocity.y == 0)
        {
            animator.SetBool("isFalling", false);
        }
    }
    
}
