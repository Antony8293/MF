using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource; // AudioSource để phát âm thanh

    [SerializeField]
    private AudioSource musicSource; // AudioSource để phát nhạc nền

    [SerializeField]
    private AudioClip backGroundClip; // Âm thanh khi game over
    [SerializeField]
    private AudioClip gameOverClip; // Âm thanh khi game over

    [SerializeField]
    private AudioClip countdownClip; // Âm thanh đếm ngược

    [SerializeField]
    private AudioClip boosterSmallestClip; // Âm thanh khi sử dụng booster Smallest

    [SerializeField]
    private AudioClip bôosterHammerClip; // Âm thanh khi sử dụng booster Hammer

    [SerializeField]
    private AudioClip boosterUpgradeClip; // Âm thanh khi sử dụng booster Upgrade

    [SerializeField]
    private AudioClip boosterShakeClip; // Âm thanh khi sử dụng booster Shake the Box

    [SerializeField]
    private AudioClip mergeClip; // Âm thanh khi thực hiện merge

    [SerializeField]
    private AudioClip dropClip; // Âm thanh khi thả đối tượng

    [SerializeField]
    private AudioClip clickClip; // Âm thanh khi nhấn nút

    private bool hasBackgroundMusic = false; // Biến để theo dõi trạng thái âm nhạc nền

    private bool hasEffectSound = false; // Biến để theo dõi trạng thái âm thanh hiệu ứng
    public static AudioManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ AudioManager không bị hủy khi chuyển cảnh
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource.Stop(); // Dừng âm thanh khi bắt đầu
        musicSource.Stop(); // Dừng nhạc nền khi bắt đầu
        hasBackgroundMusic = PlayerPrefs.GetInt("hasBackgroundMusic", 1) == 1; // Lấy trạng thái âm nhạc nền từ PlayerPrefs
        hasEffectSound = PlayerPrefs.GetInt("hasEffectSound", 1) == 1; // Lấy trạng thái âm thanh hiệu ứng từ PlayerPrefs
        if (hasBackgroundMusic)
        {
            PlayBackgroundMusic(); // Phát âm thanh nền nếu đã bật
        }
    }

    
    public void PlayBackgroundMusic()
    {
        if (hasBackgroundMusic && !musicSource.isPlaying)
        {
            musicSource.clip = backGroundClip; // Gán âm thanh nền
            musicSource.loop = true; // Lặp lại âm thanh nền
            musicSource.Play(); // Phát âm thanh nền
        }
    }

    public void PlayClickSound()
    {
        if (hasEffectSound)
        {
            audioSource.PlayOneShot(clickClip); // Phát âm thanh khi nhấn nút
        }
    }

    public void PlayGameOverSound()
    {
        if (hasEffectSound)
        {
            audioSource.PlayOneShot(gameOverClip);  // Phát âm thanh game over
        }
    }

    public void StopSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop(); 
        }
    }

    public void PlayCountdownSound()
    {
        if (hasEffectSound)
        {
            audioSource.PlayOneShot(countdownClip); // Phát âm thanh đếm ngược
        }
    }

    public void PlayBoosterSmallestSound()
    {
        if (hasEffectSound)
        {
            audioSource.PlayOneShot(boosterSmallestClip);   // Phát âm thanh khi sử dụng booster Smallest
        }
    }

    public void PlayBoosterHammerSound()
    {
        if (hasEffectSound)
        {
            audioSource.PlayOneShot(bôosterHammerClip); // Phát âm thanh khi sử dụng booster Hammer
        }
    }

    public void PlayBoosterUpgradeSound()
    {
        if (hasEffectSound)
        {
             audioSource.PlayOneShot(boosterUpgradeClip); // Phát âm thanh khi sử dụng booster Upgrade
        }
       
    }

    public void PlayBoosterShakeSound()
    {
        if (hasEffectSound)
        {
             audioSource.PlayOneShot(boosterShakeClip); // Phát âm thanh khi sử dụng booster Shake the Box
        }
       
    }

    public void PlayMergeSound()
    {
        if (hasEffectSound)
        {
            audioSource.PlayOneShot(mergeClip); // Phát âm thanh khi thực hiện merge
        }
    }

    public void PlayDropSound()
    {
        if (hasEffectSound)
        {
            audioSource.PlayOneShot(dropClip); // Phát âm thanh khi thả đối tượng
        }
        
    }   
    
    
}
