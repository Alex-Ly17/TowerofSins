using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovementInput : MonoBehaviour
{
    // Links the unity input system
    [Header("Input")]
    [Tooltip("Drag PlayerControls.inputactions here.")]
    [SerializeField] public InputActionAsset inputActions;

    // Declares the variables used for animation
    [Header("Animator Parameters")]
    [SerializeField] private string moveXParam = "MoveX";
    [SerializeField] private string moveYParam = "MoveY";

    // Low number = fast, high number = slow
    [Header("Smoothing")]
    [SerializeField] private float dampTime = 0.1f;

    // Variables that determine the games movement physics
    [Header("Physics")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravityValue = -9.8f;

    // Variables corresponding to player actions
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    // Links various outside components
    private Animator animator;
    private CharacterController controller;

    // Variable for tracking player input
    public Vector2 moveInput;
    public Vector2 lookInput;

    // Variable for determining if player is moving
    Vector3 playerMove;

    // Variable for player overall speed (not base)
    Vector3 playerVelocity;

    // Checks if player is on ground
    private bool isGrounded;

    private void OnEnable()
    {
        // enables the player inputs in input system
        inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        // disable the player inputs in input system
        inputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        // connect outside components
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        // connect variables to player input
        moveAction = inputActions.FindAction("Move");
        lookAction = inputActions.FindAction("Look");
        jumpAction = inputActions.FindAction("Jump");
    }

    private void Update()
    {
        // read player move and look input  
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();

        // checks if player is on ground
        Debug.Log(isGrounded);

        isGrounded = controller.isGrounded;

        // ensures player is stable on ground
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        // apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // read player input and try to jump
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            Jump();
        }

        if (isGrounded != true)
        {
            animator.SetTrigger("isFalling");
        }

    }

    private void FixedUpdate()
    {
        Move();
    }

    // for character jump + animation
    public void Jump()
    {
        playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        animator.SetTrigger("isJump");
    }

    // for character movement + animation
    public void Move()
    {
        // sends move input to animator
        animator.SetFloat(moveXParam, moveInput.x, dampTime, Time.deltaTime);
        animator.SetFloat(moveYParam, moveInput.y, dampTime, Time.deltaTime);

        // transforms 2d moveInput to measurement in 3d space
        playerMove = new Vector3(moveInput.x, 0f, moveInput.y);
        playerMove = Vector3.ClampMagnitude(playerMove, 1f);

        // updates if player is moving
        if (playerMove != Vector3.zero)
        {
            transform.forward = playerMove;
        }

        // tells player character to move
        Vector3 finalMove = playerMove * moveSpeed + Vector3.up * playerVelocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }
}
