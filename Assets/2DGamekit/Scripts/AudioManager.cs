using UnityEngine;

public class AudioManager : MonoBehaviour 
{
    private static AudioManager _instance;

    // 배경음악 상태
    private int _bgmAlive;
    
    // 효과음 상태
    private int _effAlive;

    public static AudioManager GetInstance()
    {
        if (_instance != null)
            return _instance;
        
        _instance = FindObjectOfType<AudioManager>();

        if (_instance != null) 
            return _instance;
        
        var container = new GameObject("AudioManager");
        _instance = container.AddComponent<AudioManager>();

        return _instance;
    }
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 음악 설정
        _bgmAlive = PlayerPrefs.GetInt("BGM", 1);
        _effAlive = PlayerPrefs.GetInt("Effect", 1);
    }

    // alive = 1 -> ON
    // alive = 0 -> OFF

    /// <summary>
    /// 배경음악 상태 가져오기
    /// </summary>
    /// <returns></returns>
    public int GetBgmAlive()
    {
        return _bgmAlive;
    }

    /// <summary>
    /// 배경음악 상태 설정
    /// </summary>
    /// <param name="alive">ON OFF 상태</param>
    private void SetBgmAlive(int alive)
    {
        _bgmAlive = alive;
        PlayerPrefs.SetInt("BGM", _bgmAlive);
    }

    /// <summary>
    /// 효과음 상태 가져오기
    /// </summary>
    /// <returns></returns>
    public int GetEffAlive()
    {
        return _effAlive;
    }

    /// <summary>
    /// 효과음 상태 설정
    /// </summary>
    /// <param name="alive">On Off 상태</param>
    private void SetEffAlive(int alive)
    {
        _effAlive = alive;
        PlayerPrefs.SetInt("Effect", _effAlive);
    }

    /// <summary>
    /// 배경음악 끄기
    /// </summary>
    public void BgmOff()
    {
        SetBgmAlive(0);
    }

    /// <summary>
    /// 배경음악 켜기
    /// </summary>
    public void BgmOn()
    {
        SetBgmAlive(1);
    }
    
    /// <summary>
    /// 배경음악 끄기
    /// </summary>
    public void EffOff()
    {
        SetEffAlive(0);
    }

    /// <summary>
    /// 배경음악 켜기
    /// </summary>
    public void EffOn()
    {
        SetEffAlive(1);
    }
}
