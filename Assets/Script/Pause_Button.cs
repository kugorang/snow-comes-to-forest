﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_Button : MonoBehaviour {

    public GameObject setting;
    public GameObject pausebutton;

    public void PauseBtnPress()
    {
        pausebutton.SetActive(false);
        setting.SetActive(true);
    }

    public void MainBtnPress()
    {
        SceneManager.LoadScene("Main");
    }

    public void RestartBtnPress(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

    }

    public void ResumeBtnPress()
    {
        pausebutton.SetActive(true);
        setting.SetActive(false);
    }

}
