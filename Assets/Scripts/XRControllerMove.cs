using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(CharacterController))]
public class XRControllerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public XRNode moveHand = XRNode.LeftHand;
    public XRNode gripHand = XRNode.RightHand;
    public Transform headTransform;
    public float moveSpeed = 2.0f;
    public float acceleration = 3.0f;
    public float deadZone = 0.05f;
    public float rotationSmooth = 6.0f;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    private float verticalVelocity = 0f;
    private bool useGravity = true;
    private bool noclip = false;

    [Header("Arm Swing Settings")]
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    public float armSwingSensitivity = 0.4f;
    public float armSwingSmoothing = 6f;
    public bool enableArmSwing = true;

    private Vector3 lastLeftPos;
    private Vector3 lastRightPos;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 lastMoveDir = Vector3.zero;
    private CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        if (cc == null)
        {
            cc = gameObject.AddComponent<CharacterController>();
            cc.height = 1.7f;
            cc.radius = 0.3f;
            cc.center = new Vector3(0f, 0.9f, 0f);
        }

        if (leftHandTransform) lastLeftPos = leftHandTransform.position;
        if (rightHandTransform) lastRightPos = rightHandTransform.position;
    }

    void Update()
    {
        HandleGripToggle();
        HandleMovement();
    }

    void HandleGripToggle()
    {
        InputDevice gripDevice = InputDevices.GetDeviceAtXRNode(gripHand);
        if (gripDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gripPressed))
        {
            if (gripPressed && !noclip)
            {
                noclip = true;
                useGravity = false;
                cc.enabled = false;
                verticalVelocity = 0f;
                //Debug.Log("[XRControllerMove] Noclip ON");
            }
            else if (!gripPressed && noclip)
            {
                noclip = false;
                useGravity = true;
                cc.enabled = true;
                //Debug.Log("[XRControllerMove] Noclip OFF");
            }
        }
    }

    void HandleMovement()
    {
        Vector3 joystickMove = Vector3.zero;

        // --- Joystick movement ---
        InputDevice dev = InputDevices.GetDeviceAtXRNode(moveHand);
        if (dev.isValid && dev.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
        {
            if (axis.magnitude <= deadZone)
            {
                currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.deltaTime * acceleration);
            }
            else if (headTransform)
            {
                Vector3 fwd = headTransform.forward;
                Vector3 right = headTransform.right;
                fwd.y = 0f; right.y = 0f;
                fwd.Normalize(); right.Normalize();

                Vector3 targetDir = (fwd * axis.y + right * axis.x).normalized;
                lastMoveDir = Vector3.Slerp(
                    lastMoveDir == Vector3.zero ? targetDir : lastMoveDir,
                    targetDir,
                    Time.deltaTime * rotationSmooth
                );
                joystickMove = lastMoveDir * moveSpeed;
            }
        }

        // --- Arm-swing movement debug ---
        Vector3 armMove = Vector3.zero;
        if (enableArmSwing && leftHandTransform && rightHandTransform)
        {
            Vector3 leftPos = leftHandTransform.position;
            Vector3 rightPos = rightHandTransform.position;

            Vector3 leftVel = (leftPos - lastLeftPos) / Mathf.Max(Time.deltaTime, 0.001f);
            Vector3 rightVel = (rightPos - lastRightPos) / Mathf.Max(Time.deltaTime, 0.001f);
            Vector3 avgVel = (leftVel + rightVel) * 0.5f;
            avgVel.y = 0f;

            Vector3 headFwd = new Vector3(headTransform.forward.x, 0, headTransform.forward.z).normalized;
            float forwardAmount = Vector3.Dot(avgVel.normalized, headFwd) * avgVel.magnitude;
            float moveMagnitude = Mathf.Lerp(0, forwardAmount * armSwingSensitivity, Time.deltaTime * armSwingSmoothing);

            // Debug print
            //Debug.Log(
                //$"ArmSwing | LVel:{leftVel.magnitude:F3}  RVel:{rightVel.magnitude:F3}  Avg:{avgVel.magnitude:F3}  Dot:{forwardAmount:F3}  MoveMag:{moveMagnitude:F3}"
            //);

            if (Mathf.Abs(moveMagnitude) > 0.005f)
            {
                armMove = headFwd * moveMagnitude * 0.02f;
                //Debug.Log($"Driving move: {armMove}");
            }

            lastLeftPos = leftPos;
            lastRightPos = rightPos;
        }

        // --- Combine both movements ---
        Vector3 totalMove = joystickMove + armMove;
        currentVelocity = Vector3.Lerp(currentVelocity, totalMove, Time.deltaTime * acceleration);

        // --- Gravity ---
        if (useGravity && cc.enabled)
        {
            if (cc.isGrounded) verticalVelocity = -1f;
            else verticalVelocity += gravity * Time.deltaTime;
        }
        else verticalVelocity = 0f;

        Vector3 move = currentVelocity;
        move.y = verticalVelocity;

        if (noclip)
            transform.position += move * Time.deltaTime;
        else
            cc.Move(move * Time.deltaTime);
    }
}
