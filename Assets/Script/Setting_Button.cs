using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting_Button : MonoBehaviour {

    public Image controlImage;
    public Sprite[] controlSprite = new Sprite[2];

    public Image bgmImage;
    public Image sfxImage;
    public Sprite[] soundSprite = new Sprite[2];

    public GameObject settingPanel;

    private bool easyControl;
    private bool bgmOn;
    private bool sfxOn;

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

    public void BGMBtnPress()
    {
        if (bgmOn)
        {
            bgmImage.sprite = soundSprite[0];
        }
        else
        {
            bgmImage.sprite = soundSprite[1];
        }

        bgmOn = !bgmOn;
    }

    public void SFXBtnPress()
    {
        if (sfxOn)
        {
            sfxImage.sprite = soundSprite[0];
        }
        else
        {
            sfxImage.sprite = soundSprite[1];
        }

        sfxOn = !sfxOn;
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
