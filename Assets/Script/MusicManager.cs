using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance_;
    private AudioSource musicSource_;
    private const string MusicMuteKey = "MusicMuted";

    public bool IsMusicMuted { get; private set; }

    private void Awake()
    {
        if (instance_ == null)
        {
            instance_ = this;
            DontDestroyOnLoad(gameObject); // Menjaga lagu tetap hidup saat pindah scene

            musicSource_ = GetComponent<AudioSource>();
            IsMusicMuted = PlayerPrefs.GetInt(MusicMuteKey, 0) == 1;
            ApplyMuteState();
        }
        else
        {
            Destroy(gameObject); // Mencegah lagu ganda saat kembali ke Main Menu
        }
    }

    public void ToggleMusic()
    {
        IsMusicMuted = !IsMusicMuted;
        PlayerPrefs.SetInt(MusicMuteKey, IsMusicMuted ? 1 : 0);
        PlayerPrefs.Save();
        ApplyMuteState();
    }

    private void ApplyMuteState()
    {
        if (musicSource_ != null)
        {
            // Hanya mematikan/menyalakan AudioSource lagu di objek Music, SFX tetap bunyi
            musicSource_.mute = IsMusicMuted;
        }
    }
}