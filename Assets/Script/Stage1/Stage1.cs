using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage1 : MonoBehaviour {

    public GameObject girlfriend;
    public GameObject girlfriend_1;
    public GameObject bus;
    public GameObject controlPanel1;
    public GameObject controlPanel2;
    public GameObject player;

    public float girlSpeed;
    public float busSpeed;

    private bool panel2On = false;
    
    public float animTime = 2f;

    public GameObject fade;
    private Image fadeImage;

    private float start = 1f;
    private float end = 0f;
    private float time = 0f;
    
    private bool isPlaying = false;

    private void Awake()
    {
        fadeImage = fade.GetComponent<Image>();
    }

    private void Start()
    {
        StartFadeInAnim();
    }

    private void FixedUpdate () {
       
        if(player.transform.position.x < 5)
        {
            controlPanel1.SetActive(false);
        }

        if (player.transform.position.x <= -6)
        {
            if (!panel2On)
            {
                player.GetComponent<PlayerPlatformerController>().enabled = false;
                if (girlfriend.transform.position.x<0)
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


    private bool isLeft()
    {
        if (player.transform.position.x - girlfriend.transform.position.x >= 0)
            return true;
        else return false;
    }
    
    public void StartFadeInAnim()
    {
        Debug.Log("fade in");
			
        if (isPlaying == true)
            return;

        StartCoroutine("PlayFadeIn");
    }

    IEnumerator PlayFadeIn()
    {
        isPlaying = true;

        Color color = fadeImage.color;
        time = 0f;
        color.a = Mathf.Lerp(start, end, time);

        while (color.a > 0f)
        {
            time += Time.deltaTime / animTime;

            color.a = Mathf.Lerp(start, end, time);

            fadeImage.color = color;

            yield return null;
        }

        isPlaying = false;
        player.GetComponent<PlayerPlatformerController>().enabled = true;
        controlPanel1.SetActive(true);
    }
    
}
