using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

    //The player's rigidbody
    [SerializeField] private Rigidbody rb;
    public bool isUnderWater = false;
    [SerializeField] private PlayerInput playerInput;

    [Header("Walk Movement")]
    public float speed = 15f;
    public LayerMask ground;
    private bool isGrounded = false;
    public float groundDrag = 5;
    public float playerHeight = 1f;

    [Header("Swim Movement")]
    //The speed at which the player accelerates
    public float acceleration = 5f;
    //The maximum speed at which the player can travel
    public float maxSpeed = 3f;
    //The drag which is used to slow down the player while in the water
    public float drag = 2f;


    //The input of the player
    private Vector2 input;

    public void SetWater(bool newValue) {
        Debug.Log("Setting water to " + newValue);
        this.isUnderWater = newValue;
    }

    private void Awake() {
        if (rb == null) {
            rb = GetComponent<Rigidbody>();
        }
        if (this.playerInput == null) {
            this.playerInput = GetComponent<PlayerInput>();
        }
    }

    private void OnEnable() {
        playerInput.actions["Move"].performed += OnMoveAction;
        playerInput.actions["Move"].canceled += OnMoveAction;
    }

    private void OnDisable() {
        playerInput.actions["Move"].performed -= OnMoveAction;
        playerInput.actions["Move"].canceled -= OnMoveAction;
    }

    private void OnMoveAction(InputAction.CallbackContext context) {
        this.input = context.ReadValue<Vector2>();
    }



    // Update is called once per frame
    private void Update() {
        if (!this.isUnderWater) {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
        }
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

    //Safety mechanism to make sure the player doesn't get pushed along the z axis by other objects
    void LateUpdate() {
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}
