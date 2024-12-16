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
            // WindManager에서 바람의 힘을 가져와 적용
            Vector3 windForce = WindManager.Instance.GetWindForce();
            rb.AddForce(windForce, ForceMode.Force);
        }
    }
}
