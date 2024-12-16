using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject startScreenCanvas;  // 시작 화면 Canvas
    public GameObject endScreenCanvas;    // 종료 화면 Canvas
    public TextMeshProUGUI startScreenText; // 시작 화면 텍스트
    public TextMeshProUGUI scoreText;     // 게임 중 표시되는 점수 UI
    public TextMeshProUGUI endScreenText; // 종료 화면에 표시되는 점수 UI
    public TextMeshProUGUI timerText;     // 타이머 UI
    public PlayerMovement playerController; // 플레이어 이동/회전 컨트롤러

    private bool gameStarted = false; // 게임 시작 여부
    private bool gameEnded = false;   // 게임 종료 여부

    public static GameManager Instance;

    private int currentScore = 0;
    private int bestScore = 0;

    [SerializeField] private float gameDuration = 60f; // 게임 지속 시간(초)
    private float gameTimer;

    // 오디오 관련 변수
    private AudioSource audioSource;
    public AudioClip backgroundMusic;    // 배경음악
    public AudioClip throwSound;         // 던질 때 효과음
    public AudioClip correctBinSound;    // 알맞은 쓰레기통 효과음
    public AudioClip incorrectBinSound;  // 잘못된 쓰레기통 효과음

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 저장된 BestScore가 있다면 로드
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
    }

    void Start()
    {
        // WindManager를 초기 상태에서 비활성화
        if (WindManager.Instance != null)
        {
            WindManager.Instance.enabled = false;
        }

        // 자식 오브젝트의 AudioSource 가져오기
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

        // 시작 화면 활성화 및 텍스트 설정
        startScreenCanvas.SetActive(true);
        endScreenCanvas.SetActive(false);
        scoreText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false); // 타이머 UI 비활성화

        // 시작 화면 텍스트 설정
        startScreenText.text =
            $"<size=30>Recyclables go into the <color=green>Green</color> bin.</size>\n" +
            $"<size=30>Food waste goes into the <color=black>Black</color> bin.</size>\n" +
            $"<size=30>General waste goes into the <color=blue>Blue</color> bin.</size>\n\n\n" +
            $"<size=20><color=white>Press 'Space' to start</color></size>";

        // 플레이어 컨트롤러 비활성화
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
            // 타이머 UI 업데이트
            timerText.text = $"{Mathf.Ceil(gameTimer)}";

            // 45초 남았을 때 WindManager 활성화
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
        timerText.gameObject.SetActive(true); // 타이머 UI 활성화
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
        timerText.gameObject.SetActive(false); // 타이머 UI 비활성화
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
