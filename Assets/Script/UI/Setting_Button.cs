#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class Setting_Button : MonoBehaviour
{
    public AudioManager AudioManager;

    public Image bgmImage;
    private bool bgmOn;

    public Image controlImage;
    public Sprite[] controlSprite = new Sprite[2];

    public GameObject creditButton;
    public GameObject creditPanel;

    private bool easyControl;

    public GameObject settingPanel;
    public Image sfxImage;
    private bool sfxOn;
    public Sprite[] soundSprite = new Sprite[2];

    public void ControlBtnPress()
    {
        if (easyControl)
            controlImage.sprite = controlSprite[0];
        else
            controlImage.sprite = controlSprite[1];

        easyControl = !easyControl;
    }

    public void BGMBtnPress()
    {
        if (bgmOn)
        {
            bgmImage.sprite = soundSprite[0];
            AudioManager.BgmOn();
        }
        else
        {
            bgmImage.sprite = soundSprite[1];
            AudioManager.BgmOff();
        }

        bgmOn = !bgmOn;
    }

    public void SFXBtnPress()
    {
        if (sfxOn)
            sfxImage.sprite = soundSprite[0];
        else
            sfxImage.sprite = soundSprite[1];

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