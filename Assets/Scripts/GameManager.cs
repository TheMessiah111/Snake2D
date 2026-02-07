using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool isGameOver;
    public TextMeshProUGUI scoreTextInGame;
    public TextMeshProUGUI highScoreTextInGame;
    public TextMeshProUGUI scoreTextOnGameOver;
    public TextMeshProUGUI todays_BestTextInGame;
    public TextMeshProUGUI weekly_BestTextInGame;
    public TextMeshProUGUI allTime_BestTextInGame;
    public TextMeshProUGUI modeInGame;
    public TextMeshProUGUI modeOnGameOver;
    
    public int score;
    public int todayBest;
    public int weeklyBest;
    public int allTimeBest;
    
    [SerializeField] private GameObject gameOverPanel;
    private bool isPaused;
    private bool gameOverPanelDisplayed = false; // Add this flag
    
    // Keys for PlayerPrefs
    private const string ALLTIME_BEST_KEY = "AllTimeBest";
    private const string WEEKLY_BEST_KEY = "WeeklyBest";
    private const string TODAY_BEST_KEY = "TodayBest";
    private const string LAST_PLAY_DATE_KEY = "LastPlayDate";
    private const string WEEK_START_DATE_KEY = "WeekStartDate";
    private int mode;

    [SerializeField]private Sprite smileImage;
    [SerializeField]private Sprite paleImage;
    [SerializeField]private Sprite devilImage;
    [SerializeField]private Image gameOverDisplayImage;
    [SerializeField]private GameObject food1;
    [SerializeField]private GameObject food2;

    void Start()
    {
        isGameOver = false;
        gameOverPanel.SetActive(false);
        isPaused = false;
        gameOverPanelDisplayed = false;

        mode = PlayerPrefs.GetInt("SelectedDifficulty");
         switch (mode)
        {
            case 0:
                modeInGame.text = "Easy Mode";
                modeOnGameOver.text = "    EASY               MODE";
                modeOnGameOver.color = Color.green;
                gameOverDisplayImage.sprite = smileImage;
            break;
            case 1:
                modeInGame.text = "Medium Mode";
                modeOnGameOver.text = "MEDIUM                MODE";
                 modeOnGameOver.color = Color.darkOrange;
                 gameOverDisplayImage.sprite = paleImage;
                 Destroy(food1);
            break;
            case 2:
                modeInGame.text = "Hard Mode";
                modeOnGameOver.text = "    HARD              MODE";
                 modeOnGameOver.color = Color.red;
                 gameOverDisplayImage.sprite = devilImage;
                  Destroy(food1);
                 Destroy(food2);
            break;
            // default:
        }
        
        LoadBestScores();
        UpdateBestScoresUI();
        UpdateHighScoreUI(); // Update once at start
    }

    void Update()
    {
        scoreTextInGame.text = score.ToString();
        
        if(isGameOver && !gameOverPanelDisplayed) // Only call once
        {
            DisplayGameOverPanel();
            gameOverPanelDisplayed = true;
        }
    }

    private void LoadBestScores()
    {
        // Load all-time best
        allTimeBest = PlayerPrefs.GetInt(ALLTIME_BEST_KEY, 0);
        
        // Get current date
        string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        string lastPlayDate = PlayerPrefs.GetString(LAST_PLAY_DATE_KEY, "");
        
        // Check if it's a new day
        if (currentDate != lastPlayDate)
        {
            // Reset today's best for a new day
            todayBest = 0;
            PlayerPrefs.SetString(LAST_PLAY_DATE_KEY, currentDate);
        }
        else
        {
            // Load today's best
            todayBest = PlayerPrefs.GetInt(TODAY_BEST_KEY, 0);
        }
        
        // Check if it's a new week
        string weekStartDate = PlayerPrefs.GetString(WEEK_START_DATE_KEY, "");
        DateTime currentDateTime = DateTime.Now;
        
        if (string.IsNullOrEmpty(weekStartDate) || IsNewWeek(weekStartDate, currentDateTime))
        {
            // Reset weekly best for a new week
            weeklyBest = 0;
            PlayerPrefs.SetString(WEEK_START_DATE_KEY, GetWeekStartDate(currentDateTime));
        }
        else
        {
            // Load weekly best
            weeklyBest = PlayerPrefs.GetInt(WEEKLY_BEST_KEY, 0);
        }
        
        PlayerPrefs.Save();
    }

    private void SaveBestScores()
    {
        PlayerPrefs.SetInt(TODAY_BEST_KEY, todayBest);
        PlayerPrefs.SetInt(WEEKLY_BEST_KEY, weeklyBest);
        PlayerPrefs.SetInt(ALLTIME_BEST_KEY, allTimeBest);
        PlayerPrefs.Save();
    }

    private void UpdateBestScores()
    {
        bool scoresUpdated = false;
        
        // Update today's best
        if (score > todayBest)
        {
            todayBest = score;
            scoresUpdated = true;
        }
        
        // Update weekly best
        if (score > weeklyBest)
        {
            weeklyBest = score;
            scoresUpdated = true;
        }
        
        // Update all-time best
        if (score > allTimeBest)
        {
            allTimeBest = score;
            scoresUpdated = true;
            UpdateHighScoreUI(); // Update UI when high score changes
        }
        
        if (scoresUpdated)
        {
            SaveBestScores();
            UpdateBestScoresUI();
        }
    }

    private void UpdateBestScoresUI()
    {
        if (todays_BestTextInGame != null)
            todays_BestTextInGame.text = "Today's Best: " + todayBest.ToString();
        
        if (weekly_BestTextInGame != null)
            weekly_BestTextInGame.text = "Week's Best: " + weeklyBest.ToString();
        
        if (allTime_BestTextInGame != null)
            allTime_BestTextInGame.text = "All-Time Best: " + allTimeBest.ToString();
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreTextInGame != null)
            highScoreTextInGame.text = allTimeBest.ToString();
    }

    private bool IsNewWeek(string weekStartDateStr, DateTime currentDate)
    {
        if (DateTime.TryParse(weekStartDateStr, out DateTime weekStart))
        {
            // Check if more than 7 days have passed
            TimeSpan difference = currentDate - weekStart;
            return difference.TotalDays >= 7;
        }
        return true; // If parsing fails, treat as new week
    }

    private string GetWeekStartDate(DateTime date)
    {
        // Get the Monday of the current week
        int daysUntilMonday = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        DateTime monday = date.AddDays(-daysUntilMonday).Date;
        return monday.ToString("yyyy-MM-dd");
    }

    private void DisplayGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        
        // Update best scores when game ends
        UpdateBestScores();
        
        // Display final score on game over panel
        if (scoreTextOnGameOver != null)
            scoreTextOnGameOver.text = score.ToString();
    }

    public void UpdateScore(int points = 1)
{
     Debug.Log("GameManager hears the call");
    score += points;
    // your UI update code
}

    public void OnPauseOrPlay()
    {
        if(isPaused == false)
        {
            // Debug.Log("Pause");
            isPaused = true;
            Time.timeScale = 0f;
        }
        else
        {
            // Debug.Log("Play");
            isPaused = false;
            Time.timeScale = 1f;
        }
    }

    private void OnEnable()
    {
        Snake.OnSnakeCollision += EndGame;
    }

    private void OnDisable()
    {
        Snake.OnSnakeCollision -= EndGame;
    }

    public void EndGame()
    {
        isGameOver = true;
        Time.timeScale = 0f;
    }

    public void PlayAgain()
    {
        // Debug.Log("PlayAgain");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void HomeButton()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
        
    }

    public void BackButton()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }
}