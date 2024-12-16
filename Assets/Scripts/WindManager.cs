using UnityEngine;

public class WindManager : MonoBehaviour
{
    public static WindManager Instance;

    [Header("Wind Settings")]
    public Vector3 currentWindDirection; // 현재 바람의 방향
    public float windStrength = 5f;     // 바람의 세기
    public float windChangeInterval = 5f; // 바람이 변하는 주기 (초)
    public float windChangeSmoothness = 1f; // 바람 변화의 부드러움

    [Header("UI & Effects")]
    public Transform windIndicatorArrow; // 바람 방향을 표시하는 화살표
    public AudioSource windAudioSource;  // 바람 소리 재생
    public AudioClip windSound;          // 바람 소리 클립

    private Vector3 targetWindDirection; // 목표 바람 방향
    private float windTimer;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentWindDirection = Vector3.zero;
        SetRandomWindDirection();
        windTimer = windChangeInterval;

        // 바람 소리 설정
        if (windAudioSource != null && windSound != null)
        {
            windAudioSource.clip = windSound;
            windAudioSource.loop = true;
        }

        // 화살표 초기화
        if (windIndicatorArrow != null)
        {
            windIndicatorArrow.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 바람 방향 점진적 변화
        currentWindDirection = Vector3.Slerp(currentWindDirection, targetWindDirection, windChangeSmoothness * Time.deltaTime);

        // 타이머 감소
        windTimer -= Time.deltaTime;
        if (windTimer <= 0f)
        {
            SetRandomWindDirection();
            windTimer = windChangeInterval;
        }

        // 화살표와 바람 소리 업데이트
        UpdateWindIndicator();
    }

    void SetRandomWindDirection()
    {
        // 랜덤한 방향 설정
        targetWindDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    void UpdateWindIndicator()
    {
        if (windIndicatorArrow == null) return;

        // 화살표 활성화
        if (!windIndicatorArrow.gameObject.activeSelf)
            windIndicatorArrow.gameObject.SetActive(true);

        // 바람 방향을 기준으로 화살표 회전
        Quaternion targetRotation = Quaternion.LookRotation(currentWindDirection, Vector3.up);

        // 화살표의 회전 적용 (Smooth 회전)
        windIndicatorArrow.rotation = Quaternion.Slerp(windIndicatorArrow.rotation, targetRotation, Time.deltaTime * windChangeSmoothness);

        // 바람 소리 재생
        if (windAudioSource != null)
        {
            if (!windAudioSource.isPlaying)
            {
                windAudioSource.Stop(); // 오디오를 먼저 중지
                windAudioSource.Play(); // 오디오 재생
            }
        }
    }


    public Vector3 GetWindForce()
    {
        return currentWindDirection * windStrength;
    }

    public void StopWindEffects()
    {
        // 바람 소리 정지
        if (windAudioSource != null && windAudioSource.isPlaying)
        {
            windAudioSource.Stop();
        }

        // 화살표 비활성화
        if (windIndicatorArrow != null && windIndicatorArrow.gameObject.activeSelf)
        {
            windIndicatorArrow.gameObject.SetActive(false);
        }
    }
}
