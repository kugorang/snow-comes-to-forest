using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubble : MonoBehaviour {

    private Rigidbody2D rb2D;
	public float jumpForce;

    public bool isTouching = false;
  
    void Start()
    {
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isTouching)
        {
            rb2D.AddForce(Vector2.up * jumpForce);
         }

    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("speechBubble"))
        {
            isTouching = true;
        }

    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("speechBubble"))
        {
            isTouching = false;
        }

    }

}
