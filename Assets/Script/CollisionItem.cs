using UnityEngine;

public class CollisionItem : MonoBehaviour
{
    public float jumpValue;
    public float windValue;
    private bool checkContact;

    private void Awake()
    {
        checkContact = false;
    }

    private void Update()
    {
        if (GetComponent<Rigidbody2D>().gravityScale < 1)
        {
            GetComponent<Rigidbody2D>().gravityScale += 0.001f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !checkContact)
        {
            collision.gameObject.GetComponent<PlayerPlatformerController>().jumpTakeOffSpeed *= jumpValue;
            GameObject.Find("WindZone").GetComponent<WindZone>().windPulseMagnitude *= windValue;
            checkContact = true;

            Debug.Log("OnCollisionEnter2D");
            Debug.Log(collision.gameObject.GetComponent<PlayerPlatformerController>().jumpTakeOffSpeed);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && checkContact)
        {
            collision.gameObject.GetComponent<PlayerPlatformerController>().jumpTakeOffSpeed /= jumpValue;
            GameObject.Find("WindZone").GetComponent<WindZone>().windPulseMagnitude /= windValue;
            checkContact = false;

            Debug.Log("OnCollisionExit2D");
            Debug.Log(collision.gameObject.GetComponent<PlayerPlatformerController>().jumpTakeOffSpeed);
        }
    }
}
