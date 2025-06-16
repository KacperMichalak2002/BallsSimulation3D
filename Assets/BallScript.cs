using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class BallScript : MonoBehaviour
{
    private Camera mainCamera;
    public Rigidbody ballRigidbody;
    public float ballMass;
    public float ballRadius;
    public Vector3 velocity;

    //Gravity variables
    private Vector3 gravity = new Vector3(0, -9.81f, 0);
    private bool isOnGround = false;
    private float groundY = 0f;

    public KeyCode startKey = KeyCode.Space;
    private bool isMoving = false;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 dragPlaneOffset;
    private Plane dragPlane;
    private Bounds arenaBounds;

    public void SetProperties(Vector3 velocity, float mass, float radius)
    {
        ballRigidbody = GetComponent<Rigidbody>();
        ballMass = mass;
        ballRadius = radius;

        ballRigidbody.isKinematic = true;
        ballRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        ballRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        ballRigidbody.useGravity = false;

        transform.localScale = new Vector3(radius, radius, radius);
        transform.position = new Vector3(transform.position.x, radius / 2 + 0.1f, transform.position.z);
        this.velocity = velocity;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        dragPlane = new Plane(Vector3.up, Vector3.zero);
        SetProperties(velocity, ballMass, ballRadius);
        isMoving = false;

        var walls = GameObject.FindGameObjectsWithTag("Wall");

        if (walls.Length == 0)
        {
            Debug.LogWarning("No walls found. Clamping won't work.");
            return;
        }

        arenaBounds = new Bounds(walls[0].transform.position, Vector3.zero);
        foreach (var wall in walls)
        {
            Collider col = wall.GetComponent<Collider>();
            if (col != null)
                arenaBounds.Encapsulate(col.bounds);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(startKey))
        {
            if (!isMoving)  StartMovement();
        }

        if (!isMoving)
        {
            HandleDrag();
        }
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Wall"))
                    continue;

                if (hit.collider.gameObject == gameObject)
                {
                    isDragging = true;
                    float enter;
                    dragPlane.Raycast(ray, out enter);
                    dragPlaneOffset = transform.position - ray.GetPoint(enter);

                    break;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging && mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            float enter;

            if (dragPlane.Raycast(ray, out enter))
            {
                Vector3 newPos = ray.GetPoint(enter) + dragPlaneOffset;
                newPos.y = ballRadius / 2 + 0.1f;
                transform.position = ClampPosition(newPos);
                ResolveInitialOverlaps();

                Debug.DrawLine(ray.origin, newPos, Color.green, 0.1f);
            }
        }
    }

    Vector3 ClampPosition(Vector3 position)
    {
        Vector3 min = arenaBounds.min + new Vector3(ballRadius / 2, 0, ballRadius / 2);
        Vector3 max = arenaBounds.max - new Vector3(ballRadius / 2, 0, ballRadius / 2);

        position.x = Mathf.Clamp(position.x, min.x, max.x);
        position.z = Mathf.Clamp(position.z, min.z, max.z);
        position.y = ballRadius / 2 + 0.1f;

        return position;
    }

    private void ResolveInitialOverlaps()
    {
        BallScript[] balls = FindObjectsByType<BallScript>(FindObjectsSortMode.None);

        for (int i = 0; i < balls.Length; i++)
        {
            for (int j = i + 1; j < balls.Length; j++)
            {
                BallScript a = balls[i];
                BallScript b = balls[j];

                Vector3 delta = b.transform.position - a.transform.position;
                float dist = delta.magnitude;
                float minDist = a.ballRadius / 2 + b.ballRadius / 2;

                if (dist < minDist && dist > 0.001f)
                {
                    Vector3 correction = delta.normalized * (minDist - dist) * 0.5f;
                    a.transform.position -= correction;
                    b.transform.position += correction;
                }
                else if (dist < 0.001f)
                {
                    Vector3 randomOffset = new Vector3(Random.value, 0, Random.value).normalized * 0.01f;
                    a.transform.position += randomOffset;
                    b.transform.position -= randomOffset;
                }
            }
        }
    }


    private void FixedUpdate()
    {
        if (!isMoving) return;

        if (!isOnGround || velocity.y > 0)
        {
            velocity += gravity * Time.fixedDeltaTime;
        }
        else
        {
            float targetY = groundY + ballRadius / 2;
            transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
        }

        Vector3 nextPosition = transform.position + velocity * Time.fixedDeltaTime;
        ballRigidbody.MovePosition(nextPosition);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            HandleWallCollision(other);
        }

        if (other.CompareTag("Ball"))
        {
            SessionLogger.Log("Zdrzenie Kul");
            HandleBallCollision(other);
        }

        if (other.CompareTag("Ground"))
        {
            HandleGroundCollision(other);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground")){
            isOnGround = false;
        }
    }

    private void HandleGroundCollision(Collider ground)
    {

        groundY = 0;

        if (velocity.y < 0)
        {
            Vector3 groundNormal = Vector3.up;
            velocity = Vector3.Reflect(velocity, groundNormal);
            velocity.y *= 1.0f; // Reducing bounce height;
        }

        isOnGround = true;
    }

    private void HandleBallCollision(Collider otherBall) // Add ball separation balls sometimes gets stuck to each other while rolling close to each other
    {
       
        BallScript other = otherBall.GetComponent<BallScript>();
        if (other == null)
            return;

        if (other.GetInstanceID() < this.GetInstanceID())
            return;

        Vector3 pos1 = transform.position;
        Vector3 pos2 = other.transform.position;

        Vector3 vel1 = velocity;
        Vector3 vel2 = other.velocity;

        float r1 = ballRadius;
        float r2 = other.ballRadius;

        float m1 = ballMass;
        float m2 = other.ballMass;

        Vector3 delta = pos2 - pos1;
        float distance = delta.magnitude;

        if (distance == 0)
            return;

        Vector3 normal = delta / distance;
        Vector3 tangent = new Vector3(-normal.z, 0, normal.x);

        float v1n = Vector3.Dot(vel1, normal);
        float v1t = Vector3.Dot(vel1, tangent);

        float v2n = Vector3.Dot(vel2, normal);
        float v2t = Vector3.Dot(vel2, tangent);

        float v1nAfter = (v1n * (m1 - m2) + 2 * m2 * v2n) / (m1 + m2);
        float v2nAfter = (v2n * (m2 - m1) + 2 * m1 * v1n) / (m1 + m2);

        Vector3 v1nAfterVec = normal * v1nAfter;
        Vector3 v1tAfterVec = tangent * v1t;

        Vector3 v2nAfterVec = normal * v2nAfter;
        Vector3 v2tAfterVec = tangent * v2t;

        velocity = v1nAfterVec + v1tAfterVec;
        other.velocity = v2nAfterVec + v2tAfterVec;
        SessionLogger.Log("After collision with ball: " + velocity + " ID " + other.GetInstanceID());
    }

    private void HandleWallCollision(Collider collision)
    {
       
        Vector3 wallNormal = (transform.position - collision.ClosestPoint(transform.position)).normalized;

        Vector3 reflectedVelocity = Vector3.Reflect(velocity, wallNormal);
        velocity = reflectedVelocity.normalized * velocity.magnitude;

        transform.position += wallNormal * 0.01f;

        SessionLogger.Log("After collision: " + velocity);
    }

    public void StartMovement()
    {
        isMoving = true;
        SessionLogger.Log("Ball movement started!");
    }
}
