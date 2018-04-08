using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage_Button : MonoBehaviour
{ 
    public Button[] seasonStageBtn = new Button[3]; //계절 스테이지 버튼
    public GameObject seasonPanel;  //파넬

    private int seasonIndex;    //계절값

    public GameObject exit;

    public void SeasonBtnPress(int seasonValue)
    {
        seasonIndex = seasonValue;
        seasonPanel.SetActive(true);
        SeasonBtnAddListener();       
    }

    public void SeasonBtnAddListener()
    {
        switch (seasonIndex)
        {
            case 1:
                SeasonBtnAddListenrStageLoad("Winterstage1_modified", "2", "3");
                break;
            case 2:
                SeasonBtnAddListenrStageLoad("1", "2", "3");
                break;
            case 3:
                SeasonBtnAddListenrStageLoad("1", "2", "3");
                break;
            case 4:
                SeasonBtnAddListenrStageLoad("1", "2", "3");
                break;
            default:
                Debug.LogError("계절 값 오류");
                break;
        }
    }

    public void SeasonBtnAddListenrStageLoad(string stage1, string stage2, string stage3)
    {
        seasonStageBtn[0].onClick.AddListener(() => SeasonStageLoad(stage1));
        seasonStageBtn[1].onClick.AddListener(() => SeasonStageLoad(stage2));
        seasonStageBtn[2].onClick.AddListener(() => SeasonStageLoad(stage3));
    }

    public void SeasonStageLoad(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitBtnPress()
    {
        seasonPanel.SetActive(false);
    }
}
