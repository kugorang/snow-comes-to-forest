using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject Cinemachine;
    public GameObject EndPos;
    public GameObject[] Lights;
    public PlayerPlatformerController Player;
    public Sprite StandImg;

    public void AnimStart()
    {
        StartCoroutine("Move");
        StartCoroutine("LightsOn");
    }

    private IEnumerator Move()
    {        
        Cinemachine.SetActive(false);
        Player.GetComponent<Animator>().enabled = false;
        Player.GetComponent<SpriteRenderer>().sprite = StandImg;
        Player.enabled = false;

        var posOrigin = MainCamera.transform.position;
        var moveGap = 0.001f;
        
        while (MainCamera.transform.position.x < EndPos.transform.position.x)
        {
            var cameraPos = MainCamera.transform.position;
            
            cameraPos.x += moveGap;
            moveGap += 0.01f;

            MainCamera.transform.position = cameraPos;
            
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(2.0f);
        
        MainCamera.transform.position = posOrigin;
        
        Player.enabled = true;
        Player.GetComponent<Animator>().enabled = true;
        Cinemachine.SetActive(true);
        
        yield return null;
    }

    private IEnumerator LightsOn()
    {
        foreach (var li in Lights)
        {
            li.SetActive(true);
            
            yield return new WaitForSeconds(0.4f);
        }

        yield return null;
    }
}
