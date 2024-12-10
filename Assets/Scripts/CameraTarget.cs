using UnityEngine;

public class LockRotation : MonoBehaviour
{
    public float rotationSpeed = 5f; // ���콺 ����
    private float pitch = 0f;       // ���� ȸ�� ����

    void LateUpdate()
    {
        // ���콺 �Է�
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Yaw(�¿� ȸ��)
        transform.Rotate(Vector3.up, mouseX, Space.World);

        // Pitch(���� ȸ��)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f); // ���� ȸ�� ���� ����
        transform.localRotation = Quaternion.Euler(pitch, transform.localRotation.eulerAngles.y, 0f);
    }
}
