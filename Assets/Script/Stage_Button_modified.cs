using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage_Button_modified : MonoBehaviour
{ 

    public void StageLoad(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}