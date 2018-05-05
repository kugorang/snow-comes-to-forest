﻿using System.Collections;
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
            StartCoroutine("PanelOnOff");
            player.GetComponent<PlayerPlatformerController>().enabled = true;

        }
        else if (busMove)
        {
            StartCoroutine("BusMove");
        }

    }

    private void GirlfriendMove()
    {
        girlfriend.transform.Translate(Vector2.right * girlSpeed * Time.deltaTime, Space.World);

    }

     private IEnumerator BusMove()
    {
        yield return new WaitForSeconds(10);
        bus.SetActive(true);
        while (bus.transform.position.x == 3)
        {
            bus.transform.Translate(Vector2.right * busSpeed * Time.deltaTime, Space.World);
        }
        yield return new WaitForSeconds(1);

        bus.transform.Translate(Vector2.right * busSpeed * Time.deltaTime, Space.World);
        busMove = false;


        // 버스 움직이는 코드 미완성.
        // 버스가 멈추지 않고 무한히 달림....... 수정필요
    }

    private IEnumerator PanelOnOff()
    {
        controlPanel.SetActive(true);
        yield return new WaitForSeconds(1);
        controlPanel.SetActive(false);
        panelOn = false;

    }

}