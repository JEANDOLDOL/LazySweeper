using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro�� ����ϱ� ���� �ʿ�

public class CrosshairRaycaster : MonoBehaviour
{
    public Camera playerCamera; // �÷��̾� ī�޶�
    public Image crosshair;     // ũ�ν���� �̹���
    public TextMeshProUGUI pickupMessage;  // "Press 'E' to pickup" �ؽ�Ʈ
    public Color defaultColor = Color.white; // �⺻ ����
    public Color highlightColor = Color.red; // �����⸦ �������� �� ����
    public float rayDistance = 100f;         // ����ĳ��Ʈ �ִ� �Ÿ�

    void Start()
    {
        // �޽��� �ʱ� ���� �����
        if (pickupMessage != null)
        {
            pickupMessage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // ���� �߻�
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // ������ �±� Ȯ��
            if (IsTrash(hit.collider.tag))
            {
                crosshair.color = highlightColor; // ũ�ν���� ���� ����

                // �޽��� ǥ��
                if (pickupMessage != null)
                {
                    pickupMessage.gameObject.SetActive(true);
                }
            }
            else
            {
                ResetUI();
            }
        }
        else
        {
            ResetUI();
        }
    }

    // UI �ʱ�ȭ �޼���
    void ResetUI()
    {
        crosshair.color = defaultColor; // ũ�ν���� ���� ����
        if (pickupMessage != null)
        {
            pickupMessage.gameObject.SetActive(false); // �޽��� �����
        }
    }

    // ������ �±� Ȯ�� �޼���
    private bool IsTrash(string tag)
    {
        return tag == "Food" || tag == "Recycle" || tag == "Disposable";
    }
}
