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
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (trashCollider != null)
        {
            // 물리 충돌을 막기 위해 트리거로 전환
            trashCollider.isTrigger = true;

            // 플레이어와의 충돌을 무시할 필요가 없다면, 아래는 생략 가능
            // Physics.IgnoreCollision(trashCollider, playerCollider, true);
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
            // 던지기 전에 다시 콜라이더를 일반 콜라이더로 되돌림
            trashCollider.isTrigger = false;
        }

        Rigidbody rb = currentTrash.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }

        currentTrash = null;
    }

    public void DropTrash()
    {
        if (currentTrash == null) return;

        Collider trashCollider = currentTrash.GetComponent<Collider>();
        if (trashCollider != null)
        {
            // 버릴 때도 마찬가지로 다시 일반 콜라이더로 되돌림
            trashCollider.isTrigger = false;
        }

        currentTrash.transform.SetParent(null);

        Rigidbody rb = currentTrash.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        currentTrash = null;
    }
}
