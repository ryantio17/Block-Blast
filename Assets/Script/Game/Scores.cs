using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Scores : MonoBehaviour
{
    public Text scoreText;

    private int currentScores_;
    void Start()
    {
        currentScores_ = 0;
        UpdateScoreText();
    }

    private void OnEnable()
    {
        TheGameEvents.AddScores += AddScores;
    }

    private void OnDisable()
    {
        TheGameEvents.AddScores -= AddScores;
    }

    private void AddScores(int scores)
    {
        currentScores_ += scores;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = currentScores_.ToString();
    }
}
