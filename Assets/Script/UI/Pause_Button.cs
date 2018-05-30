using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_Button : MonoBehaviour {

    public GameObject setting;
    public GameObject pausebutton;
    public GameObject busSound;
    private AudioSource busSoundSource;

    public void PauseBtnPress()
    {
        Time.timeScale = 0;
        pausebutton.SetActive(false);
        setting.SetActive(true);
        busSoundSource.Stop();
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
    }

}
