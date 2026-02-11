using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// HDRP / VR ready free camera controller using the new Unity Input System.
/// Compatible with both desktop and VR preview.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraFreeController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float fastMultiplier = 3f;
    public float smoothTime = 0.1f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public bool holdRightMouseToLook = true;
    public bool lockCursor = true;

    [Header("VR Safety")]
    public bool disableMovementInVR = true;

    private Vector3 currentVelocity;
    private Vector2 rotation;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        rotation.x = transform.localEulerAngles.y;
        rotation.y = -transform.localEulerAngles.x;
    }

    void Update()
    {
        if (disableMovementInVR && UnityEngine.XR.XRSettings.isDeviceActive)
            return;

        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        bool lookActive = !holdRightMouseToLook || Mouse.current.rightButton.isPressed;
        if (!lookActive) return;

        Vector2 delta = Mouse.current.delta.ReadValue() * mouseSensitivity;
        rotation.x += delta.x;
        rotation.y -= delta.y;
        rotation.y = Mathf.Clamp(rotation.y, -85f, 85f);

        transform.rotation = Quaternion.Euler(rotation.y, rotation.x, 0f);

        if (lockCursor)
            Cursor.lockState = CursorLockMode.Locked;
    }

    private void HandleMovement()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        Vector3 input = Vector3.zero;

        if (kb.wKey.isPressed) input.z += 1;
        if (kb.sKey.isPressed) input.z -= 1;
        if (kb.aKey.isPressed) input.x -= 1;
        if (kb.dKey.isPressed) input.x += 1;
        if (kb.eKey.isPressed) input.y += 1;
        if (kb.qKey.isPressed) input.y -= 1;

        input.Normalize();

        float speed = moveSpeed * (kb.leftShiftKey.isPressed ? fastMultiplier : 1f);
        Vector3 targetVelocity = transform.TransformDirection(input) * speed;

        transform.position += Vector3.SmoothDamp(Vector3.zero, targetVelocity, ref currentVelocity, smoothTime) * Time.deltaTime;
    }
}
