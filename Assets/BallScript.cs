using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class BallScript : MonoBehaviour
{
    public Rigidbody ballRigidbody;
    public float ballMass;
    public float ballRadius;
    public Vector3 velocity;

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
        transform.position = new Vector3(transform.position.x, radius / 2, transform.position.z);
        this.velocity = velocity;
    }

    private void Start()
    {
        SetProperties(velocity, ballMass, ballRadius);
    }

    private void FixedUpdate()
    {
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
            Debug.Log("Zdrzenie Kul");
            HandleBallCollision(other);
        }
    }

    private void HandleBallCollision(Collider otherBall) // Change to elastic collison
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
    }

    private void HandleWallCollision(Collider collision)
    {
       
        Vector3 wallNormal = (transform.position - collision.ClosestPoint(transform.position)).normalized;

        Vector3 reflectedVelocity = Vector3.Reflect(velocity, wallNormal);
        velocity = reflectedVelocity.normalized * velocity.magnitude;

        transform.position += wallNormal * 0.01f;

        Debug.Log("After collision: " + velocity);
    }
}
