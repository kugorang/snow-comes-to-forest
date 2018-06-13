using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject Cinemachine;
    public GameObject EndPos;
    public GameObject[] Lights;
    public SpriteRenderer[] SpriteRenderers;
    public Sprite StreeLightOn;
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
        var moveGap = 0.0037f;
        
        while (MainCamera.transform.position.x < EndPos.transform.position.x)
        {
            var cameraPos = MainCamera.transform.position;
            
            cameraPos.x += moveGap;
            moveGap += 0.0037f;

            MainCamera.transform.position = cameraPos;
            
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(3.0f);
        
        MainCamera.transform.position = posOrigin;
        
        Player.enabled = true;
        Player.GetComponent<Animator>().enabled = true;
        Cinemachine.SetActive(true);
        
        yield return null;
    }

    private IEnumerator LightsOn()
    {
        for (var index = 0; index < 10; index++)
        {
            Lights[index].SetActive(true);
            SpriteRenderers[index].sprite = StreeLightOn;
            
            yield return new WaitForSeconds(0.35f);
        }
        
        yield return new WaitForSeconds(0.29f);
        
        Lights[10].SetActive(true);

        yield return null;
    }
}
