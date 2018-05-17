using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1_Tutorial : MonoBehaviour {

    public GameObject girlfriend;
    public GameObject girlfriend_1;
    public GameObject bus;
    public GameObject controlPanel1;
    public GameObject controlPanel2;
    public GameObject player;
    public GameObject mainCamera;
    public GameObject playerCamera;

    public float girlSpeed;
    public float busSpeed;

    private bool panel2On = false;

    public UnityEngine.UI.Image fade;
    private float fades = 1.0f;
    private float time = 0;
    private bool isFade = true;

    private void FixedUpdate () {

        if (isFade)
        {
            FadeIn();
        }
        
        if(player.transform.position.x < 5)
        {
            controlPanel1.SetActive(false);
        }

        if (player.transform.position.x <= -6)
        {
            if (!panel2On)
            {
                player.GetComponent<PlayerPlatformerController>().enabled = false;
                if (girlfriend.transform.position.x<=-2)
                {
                    girlfriend.transform.Translate(Vector2.right * girlSpeed * Time.deltaTime, Space.World);
                    if (isLeft())
                    {
                        player.GetComponent<SpriteRenderer>().flipX = false;
                    }
                    else player.GetComponent<SpriteRenderer>().flipX = true;
                    
                }
                else
                {
                    panel2On = true;
                }
            }
            else
            {
                player.GetComponent<PlayerPlatformerController>().enabled = true;
                girlfriend.SetActive(false);
                girlfriend_1.SetActive(true);
                controlPanel2.SetActive(true);
            }
        }

        if (panel2On)
        {
            StartCoroutine("BusFirstMove");

            if (player.transform.position.x > -1)
            {
                controlPanel2.SetActive(false);
            }
        }

    }

     private IEnumerator BusFirstMove()
    {
        yield return new WaitForSeconds(10);
        bus.SetActive(true);
        if (bus.transform.position.x <= 4.5)
        {
            bus.transform.Translate(Vector2.right * busSpeed * Time.deltaTime, Space.World);
        }
        else StartCoroutine("BusSecondMove");

    }

    private IEnumerator BusSecondMove()
    {
        yield return new WaitForSeconds(1);

        bus.transform.Translate(Vector2.right * busSpeed * Time.deltaTime, Space.World);
        girlfriend_1.SetActive(false);
        if(bus.transform.position.x>=13)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			girlfriend.transform.position = new Vector3(-9.5f, -2.83f, 0);
			girlfriend.SetActive(true);
        }
    }

    private void FadeIn()
    {
        time += Time.deltaTime;
        if (fades > 0.0f && time >= 0.1f)
        {
            fades -= 0.1f;
            fade.color = new Color(0, 0, 0, fades);
            time = 0;
        }
        else if (fades <= 0.0f)
        {
            player.GetComponent<PlayerPlatformerController>().enabled = true;
            isFade = false;
            controlPanel1.SetActive(true);
            time = 0;
        }
    }

    private bool isLeft()
    {
        if (player.transform.position.x - girlfriend.transform.position.x >= 0)
            return true;
        else return false;
    }

}
