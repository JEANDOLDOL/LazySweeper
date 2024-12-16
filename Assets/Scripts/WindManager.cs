using UnityEngine;

public class WindManager : MonoBehaviour
{
    public static WindManager Instance;

    [Header("Wind Settings")]
    public Vector3 currentWindDirection; // ���� �ٶ��� ����
    public float windStrength = 5f;     // �ٶ��� ����
    public float windChangeInterval = 5f; // �ٶ��� ���ϴ� �ֱ� (��)
    public float windChangeSmoothness = 1f; // �ٶ� ��ȭ�� �ε巯��

    [Header("UI & Effects")]
    public Transform windIndicatorArrow; // �ٶ� ������ ǥ���ϴ� ȭ��ǥ
    public AudioSource windAudioSource;  // �ٶ� �Ҹ� ���
    public AudioClip windSound;          // �ٶ� �Ҹ� Ŭ��

    private Vector3 targetWindDirection; // ��ǥ �ٶ� ����
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

        // �ٶ� �Ҹ� ����
        if (windAudioSource != null && windSound != null)
        {
            windAudioSource.clip = windSound;
            windAudioSource.loop = true;
        }

        // ȭ��ǥ �ʱ�ȭ
        if (windIndicatorArrow != null)
        {
            windIndicatorArrow.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // �ٶ� ���� ������ ��ȭ
        currentWindDirection = Vector3.Slerp(currentWindDirection, targetWindDirection, windChangeSmoothness * Time.deltaTime);

        // Ÿ�̸� ����
        windTimer -= Time.deltaTime;
        if (windTimer <= 0f)
        {
            SetRandomWindDirection();
            windTimer = windChangeInterval;
        }

        // ȭ��ǥ�� �ٶ� �Ҹ� ������Ʈ
        UpdateWindIndicator();
    }

    void SetRandomWindDirection()
    {
        // ������ ���� ����
        targetWindDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    void UpdateWindIndicator()
    {
        if (windIndicatorArrow == null) return;

        // ȭ��ǥ Ȱ��ȭ
        if (!windIndicatorArrow.gameObject.activeSelf)
            windIndicatorArrow.gameObject.SetActive(true);

        // �ٶ� ������ �������� ȭ��ǥ ȸ��
        Quaternion targetRotation = Quaternion.LookRotation(currentWindDirection, Vector3.up);

        // ȭ��ǥ�� ȸ�� ���� (Smooth ȸ��)
        windIndicatorArrow.rotation = Quaternion.Slerp(windIndicatorArrow.rotation, targetRotation, Time.deltaTime * windChangeSmoothness);

        // �ٶ� �Ҹ� ���
        if (windAudioSource != null)
        {
            if (!windAudioSource.isPlaying)
            {
                windAudioSource.Stop(); // ������� ���� ����
                windAudioSource.Play(); // ����� ���
            }
        }
    }


    public Vector3 GetWindForce()
    {
        return currentWindDirection * windStrength;
    }

    public void StopWindEffects()
    {
        // �ٶ� �Ҹ� ����
        if (windAudioSource != null && windAudioSource.isPlaying)
        {
            windAudioSource.Stop();
        }

        // ȭ��ǥ ��Ȱ��ȭ
        if (windIndicatorArrow != null && windIndicatorArrow.gameObject.activeSelf)
        {
            windIndicatorArrow.gameObject.SetActive(false);
        }
    }
}
