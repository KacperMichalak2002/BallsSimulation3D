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
        Vector3 wallNormal = (transform.position - otherBall.ClosestPoint(transform.position)).normalized;

        Vector3 reflectedVelocity = Vector3.Reflect(velocity, wallNormal);
        velocity = reflectedVelocity.normalized * velocity.magnitude;

        transform.position += wallNormal * 0.01f;

        Debug.Log("After collision: " + velocity);
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
