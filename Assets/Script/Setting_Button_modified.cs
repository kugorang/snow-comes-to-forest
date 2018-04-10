using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting_Button_modified : MonoBehaviour {

    public Image controlImage;
    public Sprite[] controlSprite = new Sprite[2];

    public GameObject settingPanel;

    private bool easyControl;

    public GameObject creditButton;
    public GameObject creditPanel;

    public void ControlBtnPress()
    {
        if (easyControl)
        {
            controlImage.sprite = controlSprite[0];
        }
        else
        {
            controlImage.sprite = controlSprite[1];
        }

        easyControl = !easyControl;
    }

    public void CreditBtnPress()
    {
        creditPanel.SetActive(true);
    }

    public void ExitCreditPress()
    {
        creditPanel.SetActive(false);
    }

}
