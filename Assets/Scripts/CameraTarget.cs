using UnityEngine;

public class LockRotation : MonoBehaviour
{
    public float rotationSpeed = 5f; // 마우스 감도
    private float pitch = 0f;       // 상하 회전 각도

    void LateUpdate()
    {
        // 마우스 입력
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Yaw(좌우 회전)
        transform.Rotate(Vector3.up, mouseX, Space.World);

        // Pitch(상하 회전)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f); // 상하 회전 각도 제한
        transform.localRotation = Quaternion.Euler(pitch, transform.localRotation.eulerAngles.y, 0f);
    }
}
