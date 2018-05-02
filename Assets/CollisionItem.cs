using UnityEngine;

public class CollisionItem : MonoBehaviour
{
    public int jumpValue;
    public int windValue;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //collision.gameObject.GetComponent<PlayerPlatformerController>().jumpTakeOffSpeed *= jumpValue;
            GameObject.Find("WindZone").GetComponent<WindZone>().windPulseMagnitude *= windValue;

            //Debug.Log("OnCollisionEnter2D");
            //Debug.Log(collision.gameObject.GetComponent<PlayerPlatformerController>().jumpTakeOffSpeed);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //collision.gameObject.GetComponent<PlayerPlatformerController>().jumpTakeOffSpeed /= jumpValue;
            GameObject.Find("WindZone").GetComponent<WindZone>().windPulseMagnitude /= windValue;

            //Debug.Log("OnCollisionExit2D");
            //Debug.Log(collision.gameObject.GetComponent<PlayerPlatformerController>().jumpTakeOffSpeed);
        }
    }
}
