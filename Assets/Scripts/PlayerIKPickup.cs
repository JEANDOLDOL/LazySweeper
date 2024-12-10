using UnityEngine;
using RootMotion.FinalIK;
using System.Collections;

public class PlayerIKPickup : MonoBehaviour
{
    public FullBodyBipedIK ik;           // FullBodyBipedIK 컴포넌트
    public Transform rightHandTarget;   // 오른손 타겟 (쓰레기 위치로 이동)
    public Animator animator;           // 애니메이터 컨트롤러
    private bool isPickingUp = false;   // 현재 픽업 상태

    void Update()
    {
        // 레이캐스트로 쓰레기 감지
        if (Input.GetKeyDown(KeyCode.E) && rightHandTarget != null && !isPickingUp)
        {
            StartPickup();
        }
    }

    void StartPickup()
    {
        isPickingUp = true;
        animator.SetTrigger("Pickup"); // 애니메이션 트리거 설정

        // 코루틴으로 IK Weight 증가
        StartCoroutine(IncreaseIKWeight());
    }

    IEnumerator IncreaseIKWeight()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // IK Weight 증가
            ik.solver.rightHandEffector.positionWeight = Mathf.Lerp(0f, 1f, t);
            ik.solver.rightHandEffector.rotationWeight = Mathf.Lerp(0f, 1f, t);

            // 손 타겟 위치 설정
            ik.solver.rightHandEffector.target = rightHandTarget;

            yield return null;
        }

        CompletePickup();
    }

    void CompletePickup()
    {
        // 쓰레기 처리 로직
        Debug.Log("Pickup complete!");

        // IK Weight 복원
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

            // IK Weight 감소
            ik.solver.rightHandEffector.positionWeight = Mathf.Lerp(1f, 0f, t);
            ik.solver.rightHandEffector.rotationWeight = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        isPickingUp = false;
    }
}
