using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// LAB 3 - GOAL 2 & GOAL 3
///
/// You don't need to know C# to finish this script. Read the plain-language
/// comments above each section, then fill in the ONE blank near the bottom
/// marked "TODO" - that's the only line you need to write yourself.
///
/// What this script does, in order, every time the game runs:
///   1. Find the "Move" action inside PlayerControls.inputactions.
///   2. Turn that action on so it starts listening for WASD / left stick.
///   3. Every single frame, ask it "what's the current input?" and store
///      the answer.
///   4. Hand that answer to the Animator so your 2D Freeform Directional
///      blend tree (Goal 3) knows which way to blend.
///
/// Setup reminder: this script goes on your character, alongside its
/// Animator component, and you drag PlayerControls.inputactions into the
/// "Input Actions" slot in the Inspector.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerMovementInput : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Drag PlayerControls.inputactions here.")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string moveActionName = "Move";

    [Header("Animator Parameters")]
    // These two names must exactly match the float parameters you created
    // inside the Animator (Goal 3). Capitalization matters in C# - "movex"
    // and "MoveX" are treated as two different names.
    [SerializeField] private string moveXParam = "MoveX";
    [SerializeField] private string moveYParam = "MoveY";

    [Header("Smoothing")]
    // This one number IS a response curve, in the sense from this week's
    // reading. A small number = fast Attack = tight, snappy blending.
    // A bigger number = slow Attack = floatier, smoother blending.
    // Try changing this value and see how differently the blend feels.
    [SerializeField] private float dampTime = 0.1f;

    [Header("Movement")] // Added
    [Tooltip("How fast the character moves.")]
    [SerializeField] private float moveSpeed = 5f;

    private InputAction moveAction;
    private Animator animator;
    private CharacterController controller; // Added

    /// <summary>
    /// The current input direction, updated every frame. You won't need to
    /// touch this yourself today, but this is what a future movement script
    /// would read to actually move the character (not just animate it).
    /// </summary>
    public Vector2 MoveInput { get; private set; }

    // ------------------------------------------------------------------
    // Awake() runs ONCE, right when the game starts - before anything else.
    // This block looks inside the Input Actions asset you dragged in, finds
    // the "Player" map, and finds the "Move" action inside it. Think of it
    // as Unity double-checking everything is wired up correctly and giving
    // you a clear error message in the Console if it isn't.
    // You don't need to write anything in this block.
    // ------------------------------------------------------------------
    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>(); // Added

        if (inputActions == null)
        {
            Debug.LogError($"{name}: No InputActionAsset assigned to PlayerMovementInput. " +
                            "Drag PlayerControls.inputactions into the Input Actions field.");
            return;
        }

        var map = inputActions.FindActionMap(actionMapName, throwIfNotFound: false);
        if (map == null)
        {
            Debug.LogError($"{name}: Action map \"{actionMapName}\" not found in {inputActions.name}.");
            return;
        }

        moveAction = map.FindAction(moveActionName, throwIfNotFound: false);
        if (moveAction == null)
        {
            Debug.LogError($"{name}: Action \"{moveActionName}\" not found in map \"{actionMapName}\".");
        }
    }

    // ------------------------------------------------------------------
    // OnEnable / OnDisable turn the action's "listening" on and off,
    // matching whether this GameObject is active in the scene.
    //
    // This is the single most common thing people forget when using the
    // Input System - if you skip this, moveAction never wakes up and
    // your input will silently do nothing. It's the #1 pitfall from the
    // deck ("Actions never fire") for a reason.
    // You don't need to write anything in this block.
    // ------------------------------------------------------------------
    private void OnEnable()
    {
        moveAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
    }

    // ------------------------------------------------------------------
    // Update() runs once per frame - potentially dozens or hundreds of
    // times per second. This is where the "click" from the reading
    // actually gets read and turned into something the game can use.
    // ------------------------------------------------------------------
    private void Update()
    {
        // Ask the Move action: "what's the current WASD / left-stick value
        // right now?" It comes back as a Vector2: x is left/right, y is
        // up/down. (0,0) means no input at all.
        MoveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

        // ------------------------------------------------------------
        // TODO (this is the one part you write):
        // Feed MoveInput into the Animator so the blend tree from Goal 3
        // can use it. You need TWO lines - one for MoveX, one for MoveY.
        //
        // The method you want is:
        //     animator.SetFloat(parameterName, value, dampTime, Time.deltaTime);
        //
        // Hints:
        //   - The parameter names are already stored for you above:
        //     moveXParam and moveYParam.
        //   - The values you want to send are MoveInput.x and MoveInput.y.
        //   - dampTime is already declared above too - just reuse it.
        //
        // Write your two lines below this comment:
        // ------------------------------------------------------------

        animator.SetFloat(moveXParam, MoveInput.x, dampTime, Time.deltaTime);
        animator.SetFloat(moveYParam, MoveInput.y, dampTime, Time.deltaTime);

        controller.Move(MoveInput * Time.deltaTime); // Added

    }
}
