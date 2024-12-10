using UnityEngine;
using TMPro;
using RootMotion.FinalIK;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.UI; // UI 컴포넌트를 사용하기 위해 추가

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
    public CinemachineCamera pickupVirtualCamera; // CinemachineVirtualCamera로 수정
    public CinemachineCamera mainVirtualCamera;   // CinemachineVirtualCamera로 수정

    // IK 관련 변수
    public FullBodyBipedIK ik;
    public Transform rightHandTarget;
    private bool isPickingUp = false;

    // 레이캐스트 관련 변수
    public Camera playerCamera;
    public float rayDistance = 3f;
    public LayerMask interactionLayer;

    // TrashPickupController를 연결
    public TrashPickupController trashPickupController;
    public float minThrowForce = 0f; // 최소 던지기 힘
    public float maxThrowForce = 40f; // 최대 던지기 힘

    // UI 관련 변수
    public Image throwForceSlider; // ThrowForceSlider Image를 할당

    private Vector3 throwTargetPosition; // 던지기 목표 위치 저장

    // 던지기 힘 조절 변수
    private bool isChargingThrow = false;
    private float currentThrowForce = 10f;
    private float throwForceCycleSpeed = 2f; // 던지기 힘이 순환하는 속도

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
        // 레이캐스트로 쓰레기 감지
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
                currentHitPoint = hit.point; // 히트 포인트 저장
            }
        }

        if (!hitTrash)
        {
            pickupMessage.gameObject.SetActive(false);
        }

        // E 키 입력 시 픽업 시도
        if (Input.GetKeyDown(KeyCode.E) && pickupMessage.gameObject.activeSelf && state == State.Idle && !isPickingUp)
        {
            if (hitTrashObject != null)
            {
                // TrashPickupController에 선택된 쓰레기 등록
                trashPickupController.PreparePickup(hitTrashObject);
                StartPickup();

                // 던지기 목표 위치 저장
                throwTargetPosition = currentHitPoint;
            }
        }

        // 주운 상태에서 마우스 좌클릭 시 Throw
        if (state == State.Picked)
        {
            HandleThrowInput();
        }
    }

    void HandleThrowInput()
    {
        if (Input.GetMouseButtonDown(0) && state == State.Picked)
        {
            // 던지기 준비 시작
            isChargingThrow = true;
            throwForceSlider.gameObject.SetActive(true);
            currentThrowForce = minThrowForce;
        }

        if (Input.GetMouseButton(0) && isChargingThrow)
        {
            // 던지기 힘을 10~40 사이에서 사이클링
            currentThrowForce += Mathf.Sin(Time.time * throwForceCycleSpeed) * Time.deltaTime * (maxThrowForce - minThrowForce) / 2f;

            // 던지기 힘을 최소값과 최대값 사이로 클램프
            currentThrowForce = Mathf.Clamp(currentThrowForce, minThrowForce, maxThrowForce);

            // UI 슬라이더 업데이트 (min~max를 0~1로 정규화)
            float normalizedForce = (currentThrowForce - minThrowForce) / (maxThrowForce - minThrowForce);
            throwForceSlider.fillAmount = normalizedForce * 2;
        }

        if (Input.GetMouseButtonUp(0) && isChargingThrow)
        {
            // 던지기 힘을 확정하고 던지기 실행
            isChargingThrow = false;
            throwForceSlider.gameObject.SetActive(false);

            // 던지기 애니메이션 트리거
            state = State.Throw;
            animator.SetTrigger("Throw");
        }
    }

    void StartPickup()
    {
        isPickingUp = true;
        state = State.Picked;
        animator.SetTrigger("Pickup");

        // Pickup 시 카메라 전환
        ActivateVirtualCamera(pickupVirtualCamera);

        // IK weight를 서서히 증가시키는 코루틴 시작
        StartCoroutine(IncreaseIKWeight());
    }

    IEnumerator IncreaseIKWeight()
    {
        float duration = 1.3f;
        float elapsed = 0f;

        // IK target 설정
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

        // IK 및 애니메이션 완료 후 실제 쓰레기 손에 붙이기
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
        // 상태 복구는 Throw 동작 후 ReturnToMainCamera에서 처리
    }

    private void ActivateVirtualCamera(CinemachineCamera vcam)
    {
        if (pickupVirtualCamera != null) pickupVirtualCamera.Priority = 0;
        if (mainVirtualCamera != null) mainVirtualCamera.Priority = 0;
        vcam.Priority = 10;
    }

    public void ReturnToMainCamera()
    {
        // Throw 완료 시점에 애니메이션 이벤트로 호출된다고 가정
        ActivateVirtualCamera(mainVirtualCamera);
        isPickingUp = false;
    }

    public void ReturnToIdle()
    {
        // Idle로 돌아갈 때 호출되는 메서드
        state = State.Idle;
        ActivateVirtualCamera(mainVirtualCamera);
        isPickingUp = false;
    }

    // 던지기 애니메이션 종료 시점에 애니메이션 이벤트로 호출할 수 있습니다.
    public void DoThrow()
    {
        // 플레이어가 현재 바라보는 방향으로 던지기 방향 계산
        Vector3 throwDirection = playerCamera.transform.forward;

        // throwDirection에 위쪽 벡터 추가하여 방향을 약간 위로 향하게 함
        float elevationFactor = 0.3f; // 위로 향하는 정도를 조절하는 변수 (필요에 따라 조정)
        throwDirection += Vector3.up * elevationFactor;
        throwDirection.Normalize(); // 방향을 정규화하여 크기가 1이 되도록 함

        // TrashPickupController에 던지기 호출
        trashPickupController.ThrowTrash(throwDirection, currentThrowForce);

        // 던지기 후 ReturnToIdle 호출
        ReturnToIdle();
    }

}
