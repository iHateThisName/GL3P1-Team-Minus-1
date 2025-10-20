using Pathfinding;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
    [SerializeField] Transform target;

    [SerializeField] private float speed = 200f;
    public float nextWayointDistance = 3f;

    [SerializeField] Path path;
    private int currentWaypoint = 0;
    public bool IsReachedEndOfPath { get; private set; } = false;

    [SerializeField] private Seeker seeker;
    [SerializeField] private Rigidbody rb;

    private void Start() {
        this.seeker = GetComponent<Seeker>();
        this.rb = GetComponent<Rigidbody>();
        this.target = GameManager.Instance.PlayerMovement.gameObject.transform;

        InvokeRepeating(nameof(UpdatePath), 0f, 2f); // dynamic path update every 2 seconds
    }

    private void UpdatePath() {
        if (seeker.IsDone()) {
            // StartPath will invoke OnPathComplete when the path calculation finishes.
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void OnPathComplete(Path path) {
        if (!path.error) {
            this.path = path;
            this.currentWaypoint = 0;
        }
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

        float distance = Vector2.Distance(this.rb.position, this.path.vectorPath[this.currentWaypoint]);

        if (distance < this.nextWayointDistance) {
            this.currentWaypoint++;
        }
    }
}
