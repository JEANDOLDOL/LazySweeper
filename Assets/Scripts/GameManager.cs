using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject startScreenCanvas;  // ���� ȭ�� Canvas
    public GameObject endScreenCanvas;    // ���� ȭ�� Canvas
    public TextMeshProUGUI startScreenText; // ���� ȭ�� �ؽ�Ʈ
    public TextMeshProUGUI scoreText;     // ���� �� ǥ�õǴ� ���� UI
    public TextMeshProUGUI endScreenText; // ���� ȭ�鿡 ǥ�õǴ� ���� UI
    public TextMeshProUGUI timerText;     // Ÿ�̸� UI
    public PlayerMovement playerController; // �÷��̾� �̵�/ȸ�� ��Ʈ�ѷ�

    private bool gameStarted = false; // ���� ���� ����
    private bool gameEnded = false;   // ���� ���� ����

    public static GameManager Instance;

    private int currentScore = 0;
    private int bestScore = 0;

    [SerializeField] private float gameDuration = 60f; // ���� ���� �ð�(��)
    private float gameTimer;

    // ����� ���� ����
    private AudioSource audioSource;
    public AudioClip backgroundMusic;    // �������
    public AudioClip throwSound;         // ���� �� ȿ����
    public AudioClip correctBinSound;    // �˸��� �������� ȿ����
    public AudioClip incorrectBinSound;  // �߸��� �������� ȿ����

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // ����� BestScore�� �ִٸ� �ε�
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
    }

    void Start()
    {
        // WindManager�� �ʱ� ���¿��� ��Ȱ��ȭ
        if (WindManager.Instance != null)
        {
            WindManager.Instance.enabled = false;
        }

        // �ڽ� ������Ʈ�� AudioSource ��������
        audioSource = GetComponentInChildren<AudioSource>();
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.loop = true;
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("AudioSource or BackgroundMusic is missing!");
        }

        // ���� ȭ�� Ȱ��ȭ �� �ؽ�Ʈ ����
        startScreenCanvas.SetActive(true);
        endScreenCanvas.SetActive(false);
        scoreText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false); // Ÿ�̸� UI ��Ȱ��ȭ

        // ���� ȭ�� �ؽ�Ʈ ����
        startScreenText.text =
            $"<size=30>Recyclables go into the <color=green>Green</color> bin.</size>\n" +
            $"<size=30>Food waste goes into the <color=black>Black</color> bin.</size>\n" +
            $"<size=30>General waste goes into the <color=blue>Blue</color> bin.</size>\n\n\n" +
            $"<size=20><color=white>Press 'Space' to start</color></size>";

        // �÷��̾� ��Ʈ�ѷ� ��Ȱ��ȭ
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    void Update()
    {
        if (!gameStarted && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }

        if (gameStarted && !gameEnded)
        {
            // Ÿ�̸� UI ������Ʈ
            timerText.text = $"{Mathf.Ceil(gameTimer)}";

            // 45�� ������ �� WindManager Ȱ��ȭ
            if (gameTimer <= 45f && !WindManager.Instance.enabled)
            {
                WindManager.Instance.enabled = true;
            }

            gameTimer -= Time.deltaTime;
            if (gameTimer <= 0f)
            {
                EndGame();
            }
        }

        if (gameEnded && Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
    }

    void StartGame()
    {
        startScreenCanvas.SetActive(false);
        if (playerController != null) playerController.enabled = true;
        scoreText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true); // Ÿ�̸� UI Ȱ��ȭ
        gameStarted = true;
        gameEnded = false;
        gameTimer = gameDuration;
        ResetScore();
    }

    void EndGame()
    {
        endScreenCanvas.SetActive(true);
        if (playerController != null) playerController.enabled = false;
        scoreText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false); // Ÿ�̸� UI ��Ȱ��ȭ
        UpdateEndScreenText();
        gameEnded = true;
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        scoreText.text = $"Score: {currentScore}";
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        scoreText.text = "Score: 0";
    }

    void UpdateEndScreenText()
    {
        string currentScoreColor = currentScore >= bestScore ? "<color=green>" : "<color=yellow>";
        string bestScoreColor = "<color=white>";

        endScreenText.text =
            $"Current Score: {currentScoreColor}{currentScore}</color>\n" +
            $"Best Score: {bestScoreColor}{bestScore}</color>\n\n" +
            "<size=20><color=white>Press 'R' to Restart</color></size>";
    } 

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayThrowSound()
    {
        PlaySound(throwSound);
    }

    public void PlayCorrectBinSound()
    {
        PlaySound(correctBinSound);
    }

    public void PlayIncorrectBinSound()
    {
        PlaySound(incorrectBinSound);
    }
}
