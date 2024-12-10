using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위해 필요

public class CrosshairRaycaster : MonoBehaviour
{
    public Camera playerCamera; // 플레이어 카메라
    public Image crosshair;     // 크로스헤어 이미지
    public TextMeshProUGUI pickupMessage;  // "Press 'E' to pickup" 텍스트
    public Color defaultColor = Color.white; // 기본 색상
    public Color highlightColor = Color.red; // 쓰레기를 감지했을 때 색상
    public float rayDistance = 100f;         // 레이캐스트 최대 거리

    void Start()
    {
        // 메시지 초기 상태 숨기기
        if (pickupMessage != null)
        {
            pickupMessage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 레이 발사
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // 쓰레기 태그 확인
            if (IsTrash(hit.collider.tag))
            {
                crosshair.color = highlightColor; // 크로스헤어 색상 변경

                // 메시지 표시
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

    // UI 초기화 메서드
    void ResetUI()
    {
        crosshair.color = defaultColor; // 크로스헤어 색상 복구
        if (pickupMessage != null)
        {
            pickupMessage.gameObject.SetActive(false); // 메시지 숨기기
        }
    }

    // 쓰레기 태그 확인 메서드
    private bool IsTrash(string tag)
    {
        return tag == "Food" || tag == "Recycle" || tag == "Disposable";
    }
}
