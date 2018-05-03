using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour {

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Girlfriend"))
        {
            other.gameObject.SetActive(false);
        }

    }
}
