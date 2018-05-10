using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubble : MonoBehaviour {

    private Rigidbody2D rb2D;
    public float jumpForce = 100;

    public bool isTouching = false;
  
    void Start()
    {
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        Debug.Log("Start");
    }

    void FixedUpdate()
    {
        if (isTouching)
        {

            rb2D.AddForce(Vector2.up * jumpForce);
            Debug.Log("jump");
        }

    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("speechBubble"))
        {
            isTouching = true;
            Debug.Log("In");
        }

    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("speechBubble"))
        {
            isTouching = false;
            Debug.Log("Out");
        }

    }

}
