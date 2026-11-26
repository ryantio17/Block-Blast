    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

[System.Serializable]
public class BestScoreData
{
    public int score = 0;
}

public class Scores : MonoBehaviour
{
    public SquareTextureData squareTextureData;
    public Text scoreText;
    
    private bool newBestScore_ = false;
    private BestScoreData bestScores_ = new BestScoreData();
    private int currentScores_;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            bestScores_.score = 0;
            newBestScore_ = false;
            BinaryDataStream.Save<BestScoreData>(bestScores_, bestScoreKey);
            TheGameEvents.UpdateBestScoreBar(currentScores_, bestScores_.score);
            Debug.Log("Best Score lokal berhasil direset ke 0!");
        }
#endif
    }

    public int CurrentScores => currentScores_;
    public bool IsNewBestScore => newBestScore_;

    private string bestScoreKey = "bsdata";

    private void Awake()
    {
        if(BinaryDataStream.Exist(bestScoreKey))
        {
            StartCoroutine(ReadDataFile());
        }
    }

    private IEnumerator ReadDataFile()
    {
        bestScores_ = BinaryDataStream.Read<BestScoreData>(bestScoreKey);
        yield return new WaitForEndOfFrame();
        TheGameEvents.UpdateBestScoreBar(currentScores_, bestScores_.score);
    }
    void Start()
    {
        currentScores_ = 0;
        newBestScore_ = false;
        squareTextureData.SetStartColor();
        UpdateScoreText();
    }

    private void OnEnable()
    {
        TheGameEvents.AddScores += AddScores;
        TheGameEvents.GameOver += SaveBestScores;
    }

    private void OnDisable()
    {
        TheGameEvents.AddScores -= AddScores;
        TheGameEvents.GameOver -= SaveBestScores;
    }

    public void SaveBestScores(bool newBestScore)
    {
        BinaryDataStream.Save<BestScoreData>(bestScores_, bestScoreKey);
    }

    private void AddScores(int scores)
    {
        currentScores_ += scores;
        if(currentScores_ > bestScores_.score)
        {
            newBestScore_ = true;
            bestScores_.score = currentScores_;
            SaveBestScores(true);
        }

        UpdateSquareColor();
        TheGameEvents.UpdateBestScoreBar(currentScores_, bestScores_.score);
        UpdateScoreText();
    }

    private void UpdateSquareColor()
    {
        if (TheGameEvents.UpdateSquareColor != null && currentScores_ >= squareTextureData.tresholdVal)
        {
            squareTextureData.UpdateColors(currentScores_);
            TheGameEvents.UpdateSquareColor(squareTextureData.currentColor);
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = currentScores_.ToString();
    }
}
