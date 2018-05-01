﻿using UnityEngine;
using System.Collections;

public class CreateApple : MonoBehaviour
{
    public Rigidbody2D applePrefab;
    public Transform generatePos;
    private bool initStart;
    private bool genStart;

    private void Awake()
    {
        initStart = false;
        genStart = false;
    }

    private void Update()
    {
        if (initStart && !genStart)
        {
            StartCoroutine("AppleGen");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            initStart = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            initStart = false;
        }
    }

    IEnumerator AppleGen()
    {
        //genStart = true;

        Instantiate(applePrefab, generatePos.position, generatePos.rotation);
        //yield return new WaitForSeconds(3.0f);
        yield return null;

        //genStart = false;
    }
}