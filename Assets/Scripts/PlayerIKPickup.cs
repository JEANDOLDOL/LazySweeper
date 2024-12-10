using UnityEngine;
using RootMotion.FinalIK;
using System.Collections;

public class PlayerIKPickup : MonoBehaviour
{
    public FullBodyBipedIK ik;           // FullBodyBipedIK ������Ʈ
    public Transform rightHandTarget;   // ������ Ÿ�� (������ ��ġ�� �̵�)
    public Animator animator;           // �ִϸ����� ��Ʈ�ѷ�
    private bool isPickingUp = false;   // ���� �Ⱦ� ����

    void Update()
    {
        // ����ĳ��Ʈ�� ������ ����
        if (Input.GetKeyDown(KeyCode.E) && rightHandTarget != null && !isPickingUp)
        {
            StartPickup();
        }
    }

    void StartPickup()
    {
        isPickingUp = true;
        animator.SetTrigger("Pickup"); // �ִϸ��̼� Ʈ���� ����

        // �ڷ�ƾ���� IK Weight ����
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

            // IK Weight ����
            ik.solver.rightHandEffector.positionWeight = Mathf.Lerp(0f, 1f, t);
            ik.solver.rightHandEffector.rotationWeight = Mathf.Lerp(0f, 1f, t);

            // �� Ÿ�� ��ġ ����
            ik.solver.rightHandEffector.target = rightHandTarget;

            yield return null;
        }

        CompletePickup();
    }

    void CompletePickup()
    {
        // ������ ó�� ����
        Debug.Log("Pickup complete!");

        // IK Weight ����
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

            // IK Weight ����
            ik.solver.rightHandEffector.positionWeight = Mathf.Lerp(1f, 0f, t);
            ik.solver.rightHandEffector.rotationWeight = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        isPickingUp = false;
    }
}
