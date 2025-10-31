using Pathfinding;
using System.Collections;
using UnityEngine;

public class BaseAI : MonoBehaviour {
    [SerializeField] private Vector3 target;

    public float speed = 200f;
    [SerializeField] private float nextWayointDistance = 3f;
    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] Path path;
    private int currentWaypoint = 0;
    public bool IsReachedEndOfPath { get; private set; } = false;

    [SerializeField] private Seeker seeker;
    [SerializeField] private Rigidbody rb;

    private bool isMovingTarget = false;

    public bool IsTargetMoving {
        get => isMovingTarget;
        set {
            if (isMovingTarget != value) {
                isMovingTarget = value;
                if (isMovingTarget) {
                    this.dynamicUpdatePathCoroutine = StartCoroutine(DynamicUpdatePath());
                } else {
                    if (this.dynamicUpdatePathCoroutine != null) {
                        StopCoroutine(this.dynamicUpdatePathCoroutine);
                    }
                    UpdatePath();
                }
            }
        }
    }
    private Coroutine dynamicUpdatePathCoroutine;

    private void Start() {
        this.seeker = GetComponent<Seeker>();
        this.rb = GetComponent<Rigidbody>();
        this.target = GameManager.Instance.PlayerMovement.gameObject.transform.position;

        if (!isMovingTarget) {
            UpdatePath();
        } else {
            this.dynamicUpdatePathCoroutine = StartCoroutine(DynamicUpdatePath());
        }
    }

    private IEnumerator DynamicUpdatePath() {
        while (true) {
            UpdatePlayerPath();
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    private void UpdatePath() {
        if (seeker.IsDone()) {
            // StartPath will invoke OnPathComplete when the path calculation finishes.
            seeker.StartPath(rb.position, target, OnPathComplete);
        }
    }

    private void UpdatePlayerPath() {
        if (seeker.IsDone()) {
            seeker.StartPath(rb.position, GameManager.Instance.PlayerMovement.gameObject.transform.position, OnPathComplete);
        }
    }

    private void OnPathComplete(Path path) {
        if (!path.error) {
            this.path = path;
            this.currentWaypoint = 0;
        }
    }

    internal void SetTarget(Vector3 newTarget) {
        this.target = newTarget;
        UpdatePath();
    }

    private void FixedUpdate() {
        if (this.path == null) {
            return;
        }

        if (this.currentWaypoint >= this.path.vectorPath.Count) {
            this.IsReachedEndOfPath = true;
            return;
        } else {
            this.IsReachedEndOfPath = false;
        }

        Vector2 direction = (this.path.vectorPath[this.currentWaypoint] - this.rb.position).normalized;
        Vector2 force = this.speed * Time.fixedDeltaTime * direction;

        this.rb.AddForce(force);

        if (rb.linearVelocity.sqrMagnitude > 0.01f) {
            // Convert velocity direction (XY plane) to 3D
            Vector3 lookDir = new Vector3(rb.linearVelocity.y, -rb.linearVelocity.x, 0f);
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, lookDir);

            // Smoothly rotate toward direction of movement
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        float distance = Vector2.Distance(this.rb.position, this.path.vectorPath[this.currentWaypoint]);

        if (distance < this.nextWayointDistance) {
            this.currentWaypoint++;
        }
    }
}
