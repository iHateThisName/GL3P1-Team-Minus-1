using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

    /// <summary>
    /// The player's rigidbody
    /// </summary>
    [SerializeField] private Rigidbody rb;
    public bool isUnderWater = false;
    [SerializeField] private PlayerInput playerInput;

    [Header("Walk Movement")]
    public float speed = 15f;
    /// <summary>
    /// LayerMask used to determine what is considered ground for the player.
    /// </summary>
    public LayerMask ground;
    private bool isGrounded = false;
    public float groundDrag = 5;
    public float playerHeight = 1f;

    [Header("Swim Movement")]
    /// <summary>
    /// The speed at which the player accelerates
    /// </summary>
    public float acceleration = 5f;
    /// <summary>
    /// The maximum speed at which the player can travel
    /// </summary>
    public float maxSpeed = 3f;
    /// <summary>
    /// The drag which is used to slow down the player while in the water
    /// </summary>
    public float drag = 2f;

    /// <summary>
    /// The input of the player
    /// </summary>
    private Vector2 input;

    private float defaultWeight; // The player's default weight
    private float addedWeight = 0f; // The additional weight added by collectibles

    private void Awake() {
        if (rb == null) {
            rb = GetComponent<Rigidbody>();
        }
        if (this.playerInput == null) {
            this.playerInput = GetComponent<PlayerInput>();
        }

        this.defaultWeight = rb.mass; // Store the default weight of the player
        GameManager.Instance.PlayerMovement = this; // Register this instance with the GameManager
    }

    private void OnEnable() {
        playerInput.actions["Move"].performed += OnMoveAction;
        playerInput.actions["Move"].canceled += OnMoveAction;
        this.playerInput.actions["Cancel"].performed += OnPauseMenuToggle;
    }

    private void OnDisable() {
        playerInput.actions["Move"].performed -= OnMoveAction;
        playerInput.actions["Move"].canceled -= OnMoveAction;
        this.playerInput.actions["Cancel"].performed -= OnPauseMenuToggle;
    }

    private void Update() {
        if (!this.isUnderWater) {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
        }
    }
    private void OnMoveAction(InputAction.CallbackContext context) {
        this.input = context.ReadValue<Vector2>();
    }

    // TODO The player input should not be handled here, but rather in a global script so it can me runned in scenes without the player
    private void OnPauseMenuToggle(InputAction.CallbackContext context) {
        GameSceneManager.Instance.TogglePauseMenu();
    }

    private void FixedUpdate() {
        if (isGrounded) {
            rb.linearDamping = groundDrag;
        } else {
            rb.linearDamping = 0;
        }

        //If the player is above water, normal movement applies
        if (!this.isUnderWater) {
            if (rb.useGravity != true) {
                rb.useGravity = true;
            }
            if (input != Vector2.zero) {
                MovePlayer();
            } else {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
        }
        //If they're underneath the water, the water movement applies
        else {
            if (rb.useGravity != false) {
                rb.useGravity = false;
            }
            UnderwaterMovement();
            WaterPhysics();
        }
    }

    private void UnderwaterMovement() {
        //The player's current velocity
        Vector3 currentVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, 0f);

        //Find the desired velocity
        Vector3 desiredVelocity = new Vector3(input.x, input.y, 0f) * maxSpeed;

        //Calculate the velocity change
        Vector3 velocityChange = desiredVelocity - currentVelocity;

        //Clamp the acceleration of the player
        velocityChange = Vector3.ClampMagnitude(velocityChange, acceleration * Time.fixedDeltaTime);

        //Apply the force to the rigidbody
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void WaterPhysics() {
        //The velocity of the player
        Vector3 velocity = rb.linearVelocity;

        //Apply drag to the player
        velocity.x *= 1f / (1f + drag * Time.fixedDeltaTime);
        velocity.y *= 1f / (1f + drag * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(velocity.x, velocity.y, 0f);
    }

    private void MovePlayer() {
        //rb.linearVelocity = new Vector3((input.x * speed) * Time.fixedDeltaTime, 0f, 0f);
        Vector3 walkSpeed = new Vector3(input.x * speed, 0f, 0f);
        //rb.AddForce(walkSpeed);
        rb.linearVelocity = new Vector3(walkSpeed.x, rb.linearVelocity.y, 0f);
    }

    /// <summary>
    /// Increases the player's weight by the specified amount
    /// </summary>
    /// <param name="amount">How much to increase by</param>
    public void IncreaseWeight(float amount) {
        this.addedWeight += amount;
        rb.mass = 1f + this.addedWeight;
    }

    /// <summary>
    /// Decreases the player's weight by the specified amount
    /// </summary>
    /// <param name="amount">How much to decrease by</param>
    public void DecreaseWeight(float amount) {
        this.addedWeight -= amount;
        rb.mass = 1f + this.addedWeight;
    }
}
