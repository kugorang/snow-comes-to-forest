using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit_Button : MonoBehaviour {

    public GameObject exitButton;

    /*// Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
                return;
            }
        }
    }*/

    public void ExitBtnPress()
    {
        Application.Quit();
        Debug.LogError("Button Pressed");
        return;
    }

}
