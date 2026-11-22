using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopUp : MonoBehaviour
{
    public GameObject gameOverPopUp;
    public GameObject loosePopup;
    public GameObject newBestScorePopUp;

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
        loosePopup.SetActive(false);
        newBestScorePopUp.SetActive(true);
    }
}
