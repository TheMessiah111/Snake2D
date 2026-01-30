using TMPro;
using UnityEngine;

public class ScoreUpdate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI scoreText;
    void Start()
    {
        scoreText.text = "0";
    }
     public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

}
