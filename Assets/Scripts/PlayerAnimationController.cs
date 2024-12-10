using UnityEngine;
using TMPro;
using RootMotion.FinalIK;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.UI; // UI ������Ʈ�� ����ϱ� ���� �߰�

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    private enum State
    {
        Idle,
        Picked,
        Throw
    }
    private State state;

    public TextMeshProUGUI pickupMessage;
    public CinemachineCamera pickupVirtualCamera; // CinemachineVirtualCamera�� ����
    public CinemachineCamera mainVirtualCamera;   // CinemachineVirtualCamera�� ����

    // IK ���� ����
    public FullBodyBipedIK ik;
    public Transform rightHandTarget;
    private bool isPickingUp = false;

    // ����ĳ��Ʈ ���� ����
    public Camera playerCamera;
    public float rayDistance = 3f;
    public LayerMask interactionLayer;

    // TrashPickupController�� ����
    public TrashPickupController trashPickupController;
    public float minThrowForce = 0f; // �ּ� ������ ��
    public float maxThrowForce = 40f; // �ִ� ������ ��

    // UI ���� ����
    public Image throwForceSlider; // ThrowForceSlider Image�� �Ҵ�

    private Vector3 throwTargetPosition; // ������ ��ǥ ��ġ ����

    // ������ �� ���� ����
    private bool isChargingThrow = false;
    private float currentThrowForce = 10f;
    private float throwForceCycleSpeed = 2f; // ������ ���� ��ȯ�ϴ� �ӵ�

    void Start()
    {
        state = State.Idle;
        animator = GetComponent<Animator>();

        if (animator == null) Debug.LogError("Animator not found on the player!");
        if (pickupMessage == null) Debug.LogError("Pickup message UI not assigned!");
        if (pickupVirtualCamera == null || mainVirtualCamera == null) Debug.LogError("Cinemachine Virtual Cameras not assigned!");
        if (ik == null) Debug.LogError("FullBodyBipedIK not assigned!");
        if (rightHandTarget == null) Debug.LogError("RightHandTarget not assigned!");
        if (trashPickupController == null) Debug.LogError("TrashPickupController not assigned!");
        if (throwForceSlider == null) Debug.LogError("ThrowForceSlider UI Image not assigned!");
    }

    void Update()
    {
        // ����ĳ��Ʈ�� ������ ����
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        bool hitTrash = false;
        GameObject hitTrashObject = null;
        Vector3 currentHitPoint = Vector3.zero;

        if (Physics.Raycast(ray, out hit, rayDistance, interactionLayer))
        {
            if (hit.transform.CompareTag("Food") || hit.transform.CompareTag("Recycle") || hit.transform.CompareTag("Disposable"))
            {
                pickupMessage.gameObject.SetActive(true);
                hitTrash = true;
                hitTrashObject = hit.transform.gameObject;
                rightHandTarget.position = hit.transform.position;
                currentHitPoint = hit.point; // ��Ʈ ����Ʈ ����
            }
        }

        if (!hitTrash)
        {
            pickupMessage.gameObject.SetActive(false);
        }

        // E Ű �Է� �� �Ⱦ� �õ�
        if (Input.GetKeyDown(KeyCode.E) && pickupMessage.gameObject.activeSelf && state == State.Idle && !isPickingUp)
        {
            if (hitTrashObject != null)
            {
                // TrashPickupController�� ���õ� ������ ���
                trashPickupController.PreparePickup(hitTrashObject);
                StartPickup();

                // ������ ��ǥ ��ġ ����
                throwTargetPosition = currentHitPoint;
            }
        }

        // �ֿ� ���¿��� ���콺 ��Ŭ�� �� Throw
        if (state == State.Picked)
        {
            HandleThrowInput();
        }
    }

    void HandleThrowInput()
    {
        if (Input.GetMouseButtonDown(0) && state == State.Picked)
        {
            // ������ �غ� ����
            isChargingThrow = true;
            throwForceSlider.gameObject.SetActive(true);
            currentThrowForce = minThrowForce;
        }

        if (Input.GetMouseButton(0) && isChargingThrow)
        {
            // ������ ���� 10~40 ���̿��� ����Ŭ��
            currentThrowForce += Mathf.Sin(Time.time * throwForceCycleSpeed) * Time.deltaTime * (maxThrowForce - minThrowForce) / 2f;

            // ������ ���� �ּҰ��� �ִ밪 ���̷� Ŭ����
            currentThrowForce = Mathf.Clamp(currentThrowForce, minThrowForce, maxThrowForce);

            // UI �����̴� ������Ʈ (min~max�� 0~1�� ����ȭ)
            float normalizedForce = (currentThrowForce - minThrowForce) / (maxThrowForce - minThrowForce);
            throwForceSlider.fillAmount = normalizedForce * 2;
        }

        if (Input.GetMouseButtonUp(0) && isChargingThrow)
        {
            // ������ ���� Ȯ���ϰ� ������ ����
            isChargingThrow = false;
            throwForceSlider.gameObject.SetActive(false);

            // ������ �ִϸ��̼� Ʈ����
            state = State.Throw;
            animator.SetTrigger("Throw");
        }
    }

    void StartPickup()
    {
        isPickingUp = true;
        state = State.Picked;
        animator.SetTrigger("Pickup");

        // Pickup �� ī�޶� ��ȯ
        ActivateVirtualCamera(pickupVirtualCamera);

        // IK weight�� ������ ������Ű�� �ڷ�ƾ ����
        StartCoroutine(IncreaseIKWeight());
    }

    IEnumerator IncreaseIKWeight()
    {
        float duration = 1.3f;
        float elapsed = 0f;

        // IK target ����
        ik.solver.rightHandEffector.target = rightHandTarget;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            ik.solver.rightHandEffector.positionWeight = Mathf.Lerp(0f, 1f, t);
            ik.solver.rightHandEffector.rotationWeight = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        CompletePickup();
    }

    void CompletePickup()
    {
        Debug.Log("Pickup complete!");

        // IK �� �ִϸ��̼� �Ϸ� �� ���� ������ �տ� ���̱�
        trashPickupController.PickUpTrash();

        StartCoroutine(ResetIKWeight());
    }

    IEnumerator ResetIKWeight()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            ik.solver.rightHandEffector.positionWeight = Mathf.Lerp(1f, 0f, t);
            ik.solver.rightHandEffector.rotationWeight = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        isPickingUp = false;
        // ���� ������ Throw ���� �� ReturnToMainCamera���� ó��
    }

    private void ActivateVirtualCamera(CinemachineCamera vcam)
    {
        if (pickupVirtualCamera != null) pickupVirtualCamera.Priority = 0;
        if (mainVirtualCamera != null) mainVirtualCamera.Priority = 0;
        vcam.Priority = 10;
    }

    public void ReturnToMainCamera()
    {
        // Throw �Ϸ� ������ �ִϸ��̼� �̺�Ʈ�� ȣ��ȴٰ� ����
        ActivateVirtualCamera(mainVirtualCamera);
        isPickingUp = false;
    }

    public void ReturnToIdle()
    {
        // Idle�� ���ư� �� ȣ��Ǵ� �޼���
        state = State.Idle;
        ActivateVirtualCamera(mainVirtualCamera);
        isPickingUp = false;
    }

    // ������ �ִϸ��̼� ���� ������ �ִϸ��̼� �̺�Ʈ�� ȣ���� �� �ֽ��ϴ�.
    public void DoThrow()
    {
        // �÷��̾ ���� �ٶ󺸴� �������� ������ ���� ���
        Vector3 throwDirection = playerCamera.transform.forward;

        // throwDirection�� ���� ���� �߰��Ͽ� ������ �ణ ���� ���ϰ� ��
        float elevationFactor = 0.3f; // ���� ���ϴ� ������ �����ϴ� ���� (�ʿ信 ���� ����)
        throwDirection += Vector3.up * elevationFactor;
        throwDirection.Normalize(); // ������ ����ȭ�Ͽ� ũ�Ⱑ 1�� �ǵ��� ��

        // TrashPickupController�� ������ ȣ��
        trashPickupController.ThrowTrash(throwDirection, currentThrowForce);

        // ������ �� ReturnToIdle ȣ��
        ReturnToIdle();
    }

}
