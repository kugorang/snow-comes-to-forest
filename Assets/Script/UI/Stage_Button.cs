using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage_Button : MonoBehaviour
{ 

    public void StageLoad(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}