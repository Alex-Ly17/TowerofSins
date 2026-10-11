using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
 
public class ThirdPersonController : MonoBehaviour
{
    // variables that connect to animator
    private const string moveXParamName = "MoveX";
    private const string moveYParamName = "MoveY";
    private const string jumpParamName = "Jump";
    private const string groundedParamName = "Grounded";
    private const string fallingParamName = "Falling";
    private const string dodgeParamName = "Dodge";

    // prevents jitter from micro movement
    private const float lookThreshold = 0.01f;

    // connect Cinemachine
    [Header("Cinemachine")]
    [SerializeField]
    private Transform cameraTarget;

    // connect Cinemachine breathing effect
    [Header("Camera Breathing")]
    public CinemachineCamera virtualCam; 
    private CinemachineBasicMultiChannelPerlin camNoise;

    // for walking audio
    [Header("Audio")]
    [SerializeField]
    public AudioSource footstepSource;

    // representing chest heaving speed
    private float normalAmplitude = 0.2f;
    private float tiredAmplitude = 1.5f;

    // representing breathing speed
    private float normalFrequency = 0.2f;
    private float tiredFrequency = 1.5f;

    // stamina system to represent if player is tired
    private bool isTired = false;

    // how far camera can tilt up
    [SerializeField]
    private float topClamp = 70.0f;

    // how far camera can tilt down
    [SerializeField]
    private float bottomClamp = -30.0f;

    // how fast player base movement is
    [Header("Speed")]
    [SerializeField]
    private float movementSpeed = 3f;

    // how fast player can pan the camera
    [SerializeField]
    private float lookSpeed = 10f;

    // Determines how high the player can jump, and how long till next jump
    private float jumpStrength = 7f;
    private float jumpDowntime = 1f;
    private float jumpCooldown = 0.5f;
    private float jumpTimer = 0f;

    // Determines dodge
    private float dodgeForce = 5f;
    private float dodgeDowntime = 5f;
    private float dodgeCooldown = 0.5f;
    private float dodgeTimer = 0f;
    private float dodgeHeight = 1f;

    // player attached reference point to check ground
    [Header("Grounded")]
    [SerializeField]
    private Transform groundCheckPoint;

    // the radius that the reference point checks for ground
    [SerializeField]
    private float groundCheckRadius = 0.2f;

    // turns selected (default) layer into ground
    [SerializeField]
    private LayerMask groundLayer;

    // declare component variables
    private Rigidbody body;
    private CapsuleCollider collider;
    private Animator animator;
    
    // track player move and look inputs
    private Vector2 move;
    private Vector2 look;

    // player height
    float standingHeight;

    // track player current speed
    private float currentSpeed;

    // keep track of where camera is
    private float yaw;
    private float pitch;

    // bools that detect if character is doing action
    private bool isGrounded = true;
    private bool isRunning;
    private bool canJump = true;
    private bool isJump = false;
    private bool falling;
    private bool canDodge = true;
    private bool isDodging = false;

    // tracks animation values for 2D blend tree
    private Vector2 currentAnimVelocity;

    // when script launch, connect component variables to outer components
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();

        // Save base height of character
        standingHeight = collider.height;

        // connect cinemachine camera effect to script
        camNoise = virtualCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // updates every frame
    private void Update()
    {
        GroundedCheck();
        Look();

        // checks if character is tired or normal
        float targetAmplitude = isTired ? tiredAmplitude : normalAmplitude;
        float targetFrequency = isTired ? tiredFrequency : normalFrequency;

        // puts in the value to adjust amplitude and frequency of perlin nouse
        camNoise.AmplitudeGain = Mathf.Lerp(camNoise.AmplitudeGain, targetAmplitude, Time.deltaTime * 2f);
        camNoise.FrequencyGain = Mathf.Lerp(camNoise.FrequencyGain, targetFrequency, Time.deltaTime * 2f);

        // if player is not on ground or dodging, stop audio 
        if (!isGrounded || isDodging)
        {
            footstepSource.Stop();
        }
    }

    // updates when the action is detected
    private void FixedUpdate()
    {
        // checks if player is dodging; breaks move physics so player can be propelled forward during dodging animation
        if (isDodging)
        {

            // timer for dodge physics
            dodgeTimer -= Time.fixedDeltaTime;

            // when timer runs out, dodging is finished and players can move normally again
            if (dodgeTimer <= 0)
            {
                isDodging = false;

                // adjust character hitbox
                collider.height = standingHeight;
            }

            // stops Move() so players get propelled forward
            return;
        }

        // checks if player is jumping, and if so gives slight delay to jump animation
        if (isJump)
        {
            // timer for jump delay
            jumpTimer -= Time.fixedDeltaTime;
            
            // when timer runs out, launch player up to match animation
            if (jumpTimer <= 0)
            {
                isJump = false;

                // after a delay, apply the jump force to player to match the actual animation
                body.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
            }
            return;
        }

        Move();
    }

    // allows the player to move
    private void Move()
    {
        // create acceleration speed where player moves faster if they are running & ensure diagonal movement is not faster
        float targetSpeed = (isRunning ? movementSpeed * 2f : movementSpeed) * move.magnitude;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime * 8f);

        // sends animation values to the 2D blend tree
        float animMultiplier = isRunning ? 2f : 1f;
        Vector2 targetAnimVelocity = move * animMultiplier;
        currentAnimVelocity = Vector2.Lerp(currentAnimVelocity, targetAnimVelocity, Time.fixedDeltaTime * 8f);

        // send animation 
        animator.SetFloat(moveXParamName, currentAnimVelocity.x);
        animator.SetFloat(moveYParamName, currentAnimVelocity.y);

        // ensures character moves in direction of camera rather then world compass value
        Vector3 forward = cameraTarget.forward;
        Vector3 right = cameraTarget.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * move.y + right * move.x).normalized;

        // make sure user can't jitter move from misinput
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);

            Vector3 currentVelocity = body.linearVelocity;
            body.linearVelocity = new Vector3(moveDirection.x * currentSpeed, currentVelocity.y, moveDirection.z * currentSpeed);

            // if player is moving play audio
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
        }
        else
        {
            Vector3 currentVelocity = body.linearVelocity;
            body.linearVelocity = new Vector3(0, currentVelocity.y, 0);
            footstepSource.Stop();
        }

        // tracks if player is falling
        falling = !isGrounded && body.linearVelocity.y < -0.1f;
        animator.SetBool(fallingParamName, falling);
    }

    // allows the player to jump
    private void Jump()
    {
        // if the player is grounded or can't jump (mid animation already); stop and don't jump
        if (!isGrounded || !canJump || !canDodge)
        {
            return;
        }

        // jump duration
        jumpTimer = jumpCooldown;

        // tell the game the player is currently jumping
        isJump = true;

        // tells the jump animation to trigger
        animator.SetTrigger(jumpParamName);

        // set canJump to false so player can't jump mid-animation
        canJump = false;
        
        // timer for when player can jump again
        StartCoroutine(JumpDowntimeCoroutine(skipLaunchTime: false));

    }

    // set delay to next jump
    private IEnumerator JumpDowntimeCoroutine(bool skipLaunchTime)
    {
        // makes character tired
        isTired = true;

        if (!skipLaunchTime)
        {
            // wait for players to be off ground
            yield return new WaitForSeconds(0.25f);
        }

        // wait for player to be grounded again
        var waitForGrounded = new WaitUntil(() => isGrounded);
        yield return waitForGrounded;

        // wait for jump down time before player can jump again
        yield return new WaitForSeconds(jumpDowntime);
        canJump = true;

        // makes character not tired
        isTired = false;
    }

    // allows the player to dodge
    private void Dodge()
    {
        // cant dodge when these are happening
        if (!isGrounded || !canJump || !canDodge)
        {
            return;
        }

        // adjust character hitbox
        collider.height = dodgeHeight;

        // applies the actual dodge to player using their current speed
        body.AddForce(transform.forward * dodgeForce, ForceMode.Impulse);

        // dodge duration
        dodgeTimer = dodgeCooldown;

        // tells the game player is currently dodging
        isDodging = true;

        // player cannot dodge immediately after doing it
        canDodge = false;

        // next dodge timer
        StartCoroutine(DodgeDowntimeCoroutine(skipLaunchTime: false));

        // animation
        animator.SetTrigger(dodgeParamName);
    }

    // set delay to next dodge
    private IEnumerator DodgeDowntimeCoroutine(bool skipLaunchTime)
    {
        // makes character tired
        isTired = true;

        if (!skipLaunchTime)
        {
            // wait for players 
            yield return new WaitForSeconds(0.25f);
        }

        // wait for dodge down time before player can jump again
        yield return new WaitForSeconds(dodgeDowntime);
        canDodge = true;

        // makes character not tired
        isTired = false;
    }

    // adjust player camera angle
    private void Look()
    {
        // check for any look inputs
        if (look.sqrMagnitude >= lookThreshold)
        {
            // calculates how much to rotate view frame
            float deltaTimeMultiplier = Time.deltaTime * lookSpeed;

            // adjust horizontal and vertical camera movement
            yaw += look.x * deltaTimeMultiplier;
            pitch -= look.y * deltaTimeMultiplier;
        }

        yaw = Mathf.Repeat(yaw, 360f);
        pitch = ClampAngle(pitch, bottomClamp, topClamp);

        // set rotation on camera object
        cameraTarget.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
    }

    // clamp angle within certain range
    private float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        // make sure angle is not less then -360
        if (lfAngle < -360f)
        {
            lfAngle += 360f;
        }

        // make sure angle is not more then 360
        if (lfAngle > 360f)
        {
            lfAngle -= 360f;
        }

        // ensures number is within range of min & max
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    // checks if player is on ground
    private void GroundedCheck()
    {
        // check if the radius around the reference point collides with the ground layer
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
        
        // tells animator if the player is grounded
        animator.SetBool(groundedParamName, isGrounded);

        if (canJump && falling && isGrounded)
        {
            canJump = false;
            StartCoroutine(JumpDowntimeCoroutine(skipLaunchTime: true));
        }
    }

    // draws the radius around the reference point for grounded check
    private void OnDrawGizmosSelected()
    {
        // makes sure the reference point actually exists first
        if (groundCheckPoint == null)
        {
            return;
        }

        // draws the reference point radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheckPoint.position, groundCheckRadius);
    }

    // connects move input value to player input system
    private void OnMove(InputValue inputValue)
    {
        move = inputValue.Get<Vector2>();
    }

    // connects jump input value to player input system
    private void OnJump()
    {
        Jump();
    }

    // connects run input value to player input system
    private void OnRun(InputValue inputValue)
    {
        isRunning = inputValue.isPressed;
    }

    // connects look input value to player input system
    private void OnLook(InputValue inputValue)
    {
        look = inputValue.Get<Vector2>();
    }

    // connects dodge input value to player input system
    private void OnDodge(InputValue inputValue)
    {
        Dodge();
    }

}