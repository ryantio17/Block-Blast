using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance_;

    private void Awake()
    {
        if (instance_ == null)
        {
            instance_ = this;
            DontDestroyOnLoad(gameObject); // Menjaga lagu tetap hidup saat pindah scene
        }
        else
        {
            Destroy(gameObject); // Mencegah lagu ganda saat kembali ke Main Menu
        }
    }
}