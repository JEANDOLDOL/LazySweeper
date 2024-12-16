using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance;

    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateScoreUI(int currentScore, int bestScore)
    {
        currentScoreText.text = "Score: " + currentScore.ToString();
        bestScoreText.text = "Best: " + bestScore.ToString();
    }
}
