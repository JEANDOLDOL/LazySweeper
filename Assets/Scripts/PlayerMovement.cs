using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    CapsuleCollider capsule;

    [Range(50f, 200f)]
    public float mouseSens = 100f; // 마우스 감도
    public float moveSpeed = 5f; // 이동 속도
    public float jumpForce = 10f; // 점프 힘
    public Transform mainCameraTarget; // Main Target Transform
    public Transform pickupCameraTarget; // pickupTarget Transform

    bool grounded;
    float groundCheckCooldown = 0.1f; // 점프 후 Ground 체크를 제한하는 시간
    float groundCheckTimer = 0f; // 타이머
    float yRotation; // 좌우 회전
    float xRotation; // 상하 회전
    float horizonal;
    float vertical;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Camera mainCamera = Camera.main; // Main Camera 참조
        if (mainCamera != null)
        {
            mainCamera.nearClipPlane = 0.01f; // 최소 렌더링 거리
        }
    }

    void Update()
    {
        RotateMainTarget(); // Main Target 회전 처리
        Move();
        CheckGrounded();

        // Ground 체크 제한 타이머 갱신
        if (groundCheckTimer > 0)
            groundCheckTimer -= Time.deltaTime;

        // 점프 처리
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            Jump();
        }
    }


    void RotateMainTarget()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        // 좌우 회전 (캐릭터 기준)
        yRotation += mouseX;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        // 상하 회전 (Main Target 기준)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (mainCameraTarget != null)
        {
            mainCameraTarget.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        if (pickupCameraTarget != null)
        {
            pickupCameraTarget.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    void Move()
    {
        horizonal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = transform.forward * vertical + transform.right * horizonal;

        rb.MovePosition(rb.position + moveDir.normalized * moveSpeed * Time.deltaTime);
    }

    void CheckGrounded()
    {
        if (groundCheckTimer > 0)
        {
            grounded = false;
            return;
        }

        Vector3 raycastStart = capsule.bounds.center - new Vector3(0, capsule.bounds.extents.y - 0.1f, 0);
        float rayLength = 0.2f;

        grounded = Physics.Raycast(raycastStart, Vector3.down, rayLength);
        Debug.DrawRay(raycastStart, Vector3.down * rayLength, grounded ? Color.green : Color.red);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        groundCheckTimer = groundCheckCooldown;
        grounded = false;
    }

    void Pickup()
    {

    }
}
