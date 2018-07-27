#region

using UnityEngine;

#endregion

public class Teddy : MonoBehaviour
{
    /*private Rigidbody2D rb2d;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }*/

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")) Debug.Log(other.contacts[0].normal.y);
    }
}