using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    public GameObject girlfriend;
    public GameObject girlfriend_1;
    public GameObject bus;
    public GameObject controlPanel;

    public GameObject player;

    public float girlSpeed;
    public float girlTime;

    public float busSpeed;

    private bool girlfriendOn = true;


    void FixedUpdate () {

        if (Time.time < girlTime)
        {
            GirlfriendMove();
        }
        else
        {
            girlfriend.SetActive(false);
            girlfriendOn = false;
        }

        if(!girlfriendOn)
        {
            controlPanel.SetActive(true);
            player.GetComponent<PlayerPlatformerController>().enabled = true;

        }

        if(Time.time > 15)
        {
            BusMove();
        }

    }

    private void GirlfriendMove()
    {
        girlfriend.transform.Translate(Vector2.right * girlSpeed * Time.deltaTime, Space.World);

    }

     private void BusMove()
    {
        bus.SetActive(true);
        bus.transform.Translate(Vector2.right * busSpeed * Time.deltaTime, Space.World);
       
    }

}
