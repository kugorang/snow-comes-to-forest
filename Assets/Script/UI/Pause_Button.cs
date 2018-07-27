#region

using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

public class Pause_Button : MonoBehaviour
{
    public GameObject busSound;
    private AudioSource busSoundSource;
    public GameObject pausebutton;

    public GameObject setting;

    private void Awake()
    {
        if (busSound != null) busSoundSource = busSound.GetComponent<AudioSource>();
    }

    public void PauseBtnPress()
    {
        Time.timeScale = 0;
        pausebutton.SetActive(false);
        setting.SetActive(true);
        if (busSound != null) busSoundSource.Pause();
    }

    public void MainBtnPress()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main_modified");
    }

    public void RestartBtnPress(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    public void ResumeBtnPress()
    {
        Time.timeScale = 1;
        pausebutton.SetActive(true);
        setting.SetActive(false);
        if (busSound != null) busSoundSource.UnPause();
    }
}