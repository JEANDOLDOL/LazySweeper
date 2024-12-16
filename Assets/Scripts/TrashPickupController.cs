using UnityEngine;

public class TrashPickupController : MonoBehaviour
{
    public Transform trashPosition;   // 플레이어 오른손의 Trash Position
    private GameObject currentTrash;  // 현재 잡고 있는 쓰레기

    public Collider playerCollider;   // 플레이어의 Collider를 인스펙터에서 지정

    public void PreparePickup(GameObject trash)
    {
        if (currentTrash == null && trash != null)
        {
            currentTrash = trash;
        }
    }

    public void PickUpTrash()
    {
        if (currentTrash == null) return;

        Rigidbody rb = currentTrash.GetComponent<Rigidbody>();
        Collider trashCollider = currentTrash.GetComponent<Collider>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // 리니어 속도 초기화
            rb.angularVelocity = Vector3.zero; // 각속도 초기화
            rb.isKinematic = true; // 물리 시뮬레이션 비활성화
            rb.useGravity = false; // 중력 비활성화
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative; // 안정적인 충돌 감지 설정
        }

        if (trashCollider != null)
        {
            trashCollider.isTrigger = true; // 충돌을 트리거로 설정하여 손에서 물리적으로 튀어나가지 않음
        }

        currentTrash.transform.SetParent(trashPosition);
        currentTrash.transform.localPosition = Vector3.zero;
        currentTrash.transform.localRotation = Quaternion.identity;
    }

    public void ThrowTrash(Vector3 throwDirection, float throwForce)
    {
        if (currentTrash == null) return;

        Collider trashCollider = currentTrash.GetComponent<Collider>();

        // 자식 관계 해제
        currentTrash.transform.SetParent(null);

        if (trashCollider != null)
        {
            trashCollider.isTrigger = false; // 트리거 해제하여 충돌 활성화
        }

        Rigidbody rb = currentTrash.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // 물리 시뮬레이션 활성화
            rb.useGravity = true; // 중력 활성화
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 고속 충돌 처리
            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse); // 던지기 힘 적용
        }

        currentTrash = null;
    }

    public void DropTrash()
    {
        if (currentTrash == null) return;

        Collider trashCollider = currentTrash.GetComponent<Collider>();
        if (trashCollider != null)
        {
            trashCollider.isTrigger = false; // 트리거 해제
        }

        Rigidbody rb = currentTrash.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // 물리 시뮬레이션 활성화
            rb.useGravity = true; // 중력 활성화
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // 고속 충돌 처리
        }

        currentTrash.transform.SetParent(null);
        currentTrash = null;
    }
}
