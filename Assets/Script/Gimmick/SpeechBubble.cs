#region

using UnityEngine;

#endregion

public class SpeechBubble : MonoBehaviour
{
    public bool isTouching;
    public float jumpForce;

    private Rigidbody2D rb2D;

    private void Start()
    {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isTouching) rb2D.AddForce(Vector2.up * jumpForce);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("speechBubble")) isTouching = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("speechBubble")) isTouching = false;
    }
}