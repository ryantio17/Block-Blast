using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{

    [Header("Pengaturan Tombol Musik")]
    public Image musicButtonImage;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

   private void Awake()
    {
        if(Application.isEditor == false)
            Debug.unityLogger.logEnabled = false;
    }

    private void Start()
    {
        UpdateMusicButtonSprite();
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void ToggleMusic()
    {
        if (MusicManager.instance_ != null)
        {
            MusicManager.instance_.ToggleMusic();
            UpdateMusicButtonSprite();
        }
    }

    private void UpdateMusicButtonSprite()
    {
        if (musicButtonImage != null && MusicManager.instance_ != null)
        {
            bool isMuted = MusicManager.instance_.IsMusicMuted;
            musicButtonImage.sprite = isMuted ? musicOffSprite : musicOnSprite;
        }
    }
}
