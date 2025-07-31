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

     [SerializeField]
    private GameObject soundOn; 

    [SerializeField]
    private GameObject soundOff;

    [SerializeField]
    private GameObject musicOn;

    [SerializeField]
    private GameObject musicOff;

    [SerializeField]
    private GameObject vibrateOn;

    [SerializeField]
    private GameObject vibrateOff;


    private bool hasBackgroundMusic = false; // Biến để theo dõi trạng thái âm nhạc nền
    private bool hasVibrate = false; // Biến để theo dõi trạng thái rung
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
        hasVibrate = PlayerPrefs.GetInt("hasVibrate", 1) == 1; // Lấy trạng thái rung từ PlayerPrefs

        setBackgroundMusic(hasBackgroundMusic); // Thiết lập trạng thái âm nhạc nền
        setEffectSound(hasEffectSound); // Thiết lập trạng thái âm thanh hiệu ứng
        setVibrate(hasVibrate); // Thiết lập trạng thái rung
        Debug.Log("AudioManager Start: hasBackgroundMusic = " + hasBackgroundMusic + ", hasEffectSound = " + hasEffectSound + ", hasVibrate = " + hasVibrate);
        if (hasBackgroundMusic)
        {
            PlayBackgroundMusic(); // Phát âm thanh nền nếu đã bật
        }
    }
    
    public void setBackgroundMusic(bool value)
    {
        hasBackgroundMusic = value;
        PlayerPrefs.SetInt("hasBackgroundMusic", value ? 1 : 0); // Lưu trạng thái vào PlayerPrefs
        if (value)
        {
            PlayBackgroundMusic(); // Phát âm thanh nền nếu đã bật
            musicOn.SetActive(true);
            musicOff.SetActive(false);
        }
        else
        {
            musicSource.Stop(); // Dừng âm thanh nền nếu đã tắt
            musicOn.SetActive(false);
            musicOff.SetActive(true);
           
        }
    }

    public void Vibrate()
    {
        if (!hasVibrate) return;
        #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
        #endif
    }

    public void setVibrate(bool value)
    {
        hasVibrate = value;
        PlayerPrefs.SetInt("hasVibrate", value ? 1 : 0); // Lưu trạng thái vào PlayerPrefs
        if (value)
        {
            vibrateOn.SetActive(true);
            vibrateOff.SetActive(false);
        }
        else
        {
            vibrateOn.SetActive(false);
            vibrateOff.SetActive(true);
        }
    }

    
    public void setEffectSound(bool value)
    {
        hasEffectSound = value;
        PlayerPrefs.SetInt("hasEffectSound", value ? 1 : 0); // Lưu trạng thái vào PlayerPrefs
        if (!value)
        {
            soundOn.SetActive(false);
            soundOff.SetActive(true);
            audioSource.Stop(); // Dừng âm thanh hiệu ứng nếu đã tắt
        }
        else
        {
            soundOn.SetActive(true);
            soundOff.SetActive(false);
        }
        Debug.Log("setEffectSound: hasEffectSound = " + hasEffectSound);
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
        Debug.Log("Playing merge sound" + hasEffectSound);
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
