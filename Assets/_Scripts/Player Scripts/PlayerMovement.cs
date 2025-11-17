using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

    /// <summary>
    /// The player's rigidbody
    /// </summary>
    public Rigidbody rb;
    public bool isUnderWater = false;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject model;

    [Header("Walk Movement")]
    public float walkSpeed = 7f;
    public float sprintSpeed = 12f;
    private float moveSpeed;
    [SerializeField] private float rotationSpeed;
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
    public float smallAcceleration = 5f;
    public float fastAcceleration = 10f;
    private float acceleration;
    /// <summary>
    /// The maximum speed at which the player can travel
    /// </summary>
    public float normalSpeed = 3;
    public float fastSpeed = 6;
    private float maxSpeed;
    private bool isSprinting;
    /// <summary>
    /// The drag which is used to slow down the player while in the water
    /// </summary>
    public float drag = 2f;

    /// <summary>
    /// The input of the player
    /// </summary>
    private Vector2 input;
    private Quaternion targetRotation;

    private float defaultWeight; // The player's default weight
    private float addedWeight = 0f; // The additional weight added by collectibles

    //[SerializeField] private InputActionReference moveActionRef; <-- This seems to be a cool way to do it for the new input system.

    private void Awake() {
        if (rb == null) {
            rb = GetComponent<Rigidbody>();
        }
        if (this.playerInput == null) {
            this.playerInput = GetComponent<PlayerInput>();
        }

        // Setting default values
        this.moveSpeed = this.walkSpeed;
        this.maxSpeed = this.normalSpeed;
        this.acceleration = this.smallAcceleration;
        this.defaultWeight = rb.mass;

        // Register this instance with the GameManager
        GameManager.Instance.PlayerMovement = this;
    }

    public Rigidbody GetRigidbody() => this.rb;

    private void OnEnable() {
        playerInput.actions["Move"].performed += OnMoveAction;
        playerInput.actions["Move"].canceled += OnMoveAction;
        this.playerInput.actions["Cancel"].performed += OnPauseMenuToggle;
        this.playerInput.actions["Sprint"].performed += OnSprintAction;
        this.playerInput.actions["Sprint"].canceled += OnSprintCancel;
    }

    private void OnDisable() {
        playerInput.actions["Move"].performed -= OnMoveAction;
        playerInput.actions["Move"].canceled -= OnMoveAction;
        this.playerInput.actions["Cancel"].performed -= OnPauseMenuToggle;
        this.playerInput.actions["Sprint"].performed -= OnSprintAction;
        this.playerInput.actions["Sprint"].canceled -= OnSprintCancel;
    }

    private void FixedUpdate() {
        // Check if the movment is disabled
        if (!GameManager.Instance.IsPlayerMovementEnabled) return;

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

        UpdateRotation();
    }

    private void UpdateRotation()
    {
        if (!this.isUnderWater)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

            if (input.x > 0f)
            {
                targetRotation = Quaternion.Euler(0, 0, 0); // facing right
            }
            else if (input.x < 0f)
            {
                targetRotation = Quaternion.Euler(0, 180f, 0); // facing left
            }
        }
        else
        {
            if (input.x > 0f)
            {
                if(input.y > 0f)
                {
                    targetRotation = Quaternion.Euler(0, 0, 45f);
                }
                else if(input.y < 0f)
                {
                    targetRotation = Quaternion.Euler(0, 0, -45f);
                }
                else
                {
                    targetRotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else if (input.x < 0f)
            {
                if (input.y > 0f)
                {
                    targetRotation = Quaternion.Euler(0, 180f, 45f);
                }
                else if(input.y < 0f)
                {
                    targetRotation = Quaternion.Euler(0, 180f, -45f);
                }
                else
                {
                    targetRotation = Quaternion.Euler(0, 180f, 0);
                }
            }
            else
            {
                if (input.y > 0f)
                {
                    targetRotation = Quaternion.Euler(0, 0, 90f);
                }
                else if (input.y < 0f)
                {
                    targetRotation = Quaternion.Euler(0, 0, -90f);
                }
                else
                {
                    targetRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }

        model.transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void OnMoveAction(InputAction.CallbackContext context) {
        this.input = context.ReadValue<Vector2>();
    }

    public void OnMoveCanceled() => this.input = Vector2.zero;

    private void OnSprintAction(InputAction.CallbackContext context) {
        moveSpeed = sprintSpeed;
        maxSpeed = fastSpeed;
        acceleration = fastAcceleration;
        if (isUnderWater) {
            GameManager.Instance.BreathingScript.sprintMultiplier = 5f;
        }
        isSprinting = true;
    }

    private void OnSprintCancel(InputAction.CallbackContext context) {
        moveSpeed = walkSpeed;
        maxSpeed = normalSpeed;
        acceleration = smallAcceleration;
        if (isUnderWater) {
            GameManager.Instance.BreathingScript.sprintMultiplier = 0f;
        }
        isSprinting = false;
    }

    // TODO The player input should not be handled here, but rather in a global script so it can me runned in scenes without the player
    private void OnPauseMenuToggle(InputAction.CallbackContext context) {
        if (GameSceneManager.Instance.IsShopMenuLoaded) {
            GameSceneManager.Instance.ToggleShopMenu();
        } else {
            GameSceneManager.Instance.TogglePauseMenu();
        }
    }



    private void UnderwaterMovement() {
        anim.SetBool("IsWalking", false);
        if(input != Vector2.zero)
        {
            anim.SetBool("IsIdleSwim", false);
            if (isSprinting)
            {
                //anim.SetBool("IsSwimming", false);
                anim.SetBool("IsFastSwimming", true);
            }
            else
            {
                anim.SetBool("IsFastSwimming", false);
                anim.SetBool("IsSwimming", true);
            }
        }
        else
        {
            anim.SetBool("IsSwimming", false);
            anim.SetBool("IsFastSwimming", false);
            anim.SetBool("IsIdleSwim", true);
        }

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
        anim.SetBool("IsWalking", true);
        anim.SetBool("IsSwimming", false);
        anim.SetBool("IsFastSwimming", false);
        anim.SetBool("IsIdleSwim", false);
        //rb.linearVelocity = new Vector3((input.x * speed) * Time.fixedDeltaTime, 0f, 0f);
        Vector3 finalSpeed = new Vector3(input.x * moveSpeed, 0f, 0f);
        //rb.AddForce(walkSpeed);
        rb.linearVelocity = new Vector3(finalSpeed.x, rb.linearVelocity.y, 0f);
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

    public void ResetMomentum() {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void ResetAnims()
    {
        anim.SetBool("IsWalking", false);
        anim.SetBool("IsSwimming", false);
        anim.SetBool("IsFastSwimming", false);
        anim.SetBool("IsIdleSwim", false);
    }
}
