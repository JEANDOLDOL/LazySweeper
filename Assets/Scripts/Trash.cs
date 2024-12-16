using UnityEngine;

public class Trash : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (WindManager.Instance != null)
        {
            // WindManager���� �ٶ��� ���� ������ ����
            Vector3 windForce = WindManager.Instance.GetWindForce();
            rb.AddForce(windForce, ForceMode.Force);
        }
    }
}
