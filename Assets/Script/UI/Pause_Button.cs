using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_Button : MonoBehaviour {

    public GameObject setting;
    public GameObject pausebutton;
    public GameObject busSound = null;
    private AudioSource busSoundSource = null;

    private void Awake()
    {
        if (busSound != null)
        {
            busSoundSource = busSound.GetComponent<AudioSource>();
        }
    }

    public void PauseBtnPress()
    {
        
        Time.timeScale = 0;
        pausebutton.SetActive(false);
        setting.SetActive(true);
        if (busSound != null)
        {
            busSoundSource.Pause();
        }
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
        if (busSound != null)
        {
            busSoundSource.UnPause();
        }
    }

}
