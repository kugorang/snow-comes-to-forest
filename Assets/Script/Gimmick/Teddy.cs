using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teddy : MonoBehaviour {

    public GameObject speechBubble;

    private void OnTriggerEnter2D(Collider2D other)
    {
        speechBubble.SetActive(true);
    }
}
