using UnityEngine;

public class Bin : MonoBehaviour
{
    // 이 쓰레기통이 받아들일 쓰레기 태그를 Inspector에서 지정할 수 있게 함.
    public string acceptableTag;

    void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체의 태그를 확인
        if (other.CompareTag(acceptableTag))
        {
            // 태그가 맞으면 점수 증가
            GameManager.Instance.AddScore(1);
            GameManager.Instance.PlayCorrectBinSound();
        }
        else
        {
            // 태그가 안 맞으면 점수 증가 없음
            // 필요하다면 감점이나 다른 로직을 넣어도 됨.
            GameManager.Instance.PlayIncorrectBinSound();
        }

        // 쓰레기 제거 (필요하다면)
        Destroy(other.gameObject);
    }
}
