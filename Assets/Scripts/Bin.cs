using UnityEngine;

public class Bin : MonoBehaviour
{
    // �� ���������� �޾Ƶ��� ������ �±׸� Inspector���� ������ �� �ְ� ��.
    public string acceptableTag;

    void OnTriggerEnter(Collider other)
    {
        // �浹�� ��ü�� �±׸� Ȯ��
        if (other.CompareTag(acceptableTag))
        {
            // �±װ� ������ ���� ����
            GameManager.Instance.AddScore(1);
            GameManager.Instance.PlayCorrectBinSound();
        }
        else
        {
            // �±װ� �� ������ ���� ���� ����
            // �ʿ��ϴٸ� �����̳� �ٸ� ������ �־ ��.
            GameManager.Instance.PlayIncorrectBinSound();
        }

        // ������ ���� (�ʿ��ϴٸ�)
        Destroy(other.gameObject);
    }
}
