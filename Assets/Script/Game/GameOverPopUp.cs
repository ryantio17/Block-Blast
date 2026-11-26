using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPopUp : MonoBehaviour
{
    public GameObject gameOverPopUp;
    public GameObject loosePopup;
    public GameObject newBestScorePopUp;

    public Text currentScoreText;
    public Scores scores;

    void Start()
    {
        gameOverPopUp.SetActive(false);
    }

    private void OnEnable()
    {
        TheGameEvents.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        TheGameEvents.GameOver -= OnGameOver;
    }

    private void OnGameOver(bool newBestScore)
    {
        gameOverPopUp.SetActive(true);
        bool isActualNewBest = scores != null ? scores.IsNewBestScore : newBestScore;

        loosePopup.SetActive(!isActualNewBest);
        newBestScorePopUp.SetActive(isActualNewBest);

        if (currentScoreText != null && scores != null)
        {
            currentScoreText.text = scores.CurrentScores.ToString();
        }
    }
}
