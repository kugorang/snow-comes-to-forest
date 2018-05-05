using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1_Tutorial : MonoBehaviour {

    public GameObject girlfriend;
    public GameObject girlfriend_1;
    public GameObject bus;
    public GameObject controlPanel;

    public GameObject player;

    public float girlSpeed;
    public float girlTime;

    public float busSpeed;

    private bool panelOn = true;
    private bool busMove = true;

    private void FixedUpdate () {

        if (Time.time < girlTime)
        {
            GirlfriendMove();
        }
        else if (panelOn)
        {
            girlfriend.SetActive(false);
            controlPanel.SetActive(true);
            player.GetComponent<PlayerPlatformerController>().enabled = true;

            if(player.transform.position.x>=-6)
            {
                controlPanel.SetActive(false);
                panelOn = false;
            }

        }
        else if (busMove)
        {
            StartCoroutine("BusFirstMove");
            
        }

    }

    private void GirlfriendMove()
    {
        girlfriend.transform.Translate(Vector2.right * girlSpeed * Time.deltaTime, Space.World);

    }

     private IEnumerator BusFirstMove()
    {
        yield return new WaitForSeconds(10);
        bus.SetActive(true);
        if (bus.transform.position.x <= 4)
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
        if(bus.transform.position.x>=22)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

}
