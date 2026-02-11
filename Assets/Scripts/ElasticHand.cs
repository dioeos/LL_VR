using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ElasticHand : MonoBehaviour
{
    public Transform controllerTarget;
    public float springStrength = 2000f;  // position stiffness
    public float damping = 60f;
    public float rotationSpring = 300f;   // rotation stiffness
    public float rotationDamping = 20f;   // rotation damping

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void FixedUpdate()
    {
        if (controllerTarget == null) return;

        Vector3 offset = controllerTarget.position - rb.position;
        Vector3 springForce = offset * springStrength - rb.velocity * damping;
        rb.AddForce(springForce * Time.fixedDeltaTime, ForceMode.VelocityChange);

        Quaternion deltaRot = controllerTarget.rotation * Quaternion.Inverse(rb.rotation);
        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f) angle -= 360f; // normalize angle
        if (Mathf.Abs(angle) > 0.001f)
        {
            Vector3 torque = axis.normalized * angle * Mathf.Deg2Rad * rotationSpring;
            Vector3 angularDamping = -rb.angularVelocity * rotationDamping;
            rb.AddTorque((torque + angularDamping) * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
