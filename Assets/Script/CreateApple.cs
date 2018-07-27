#region

using System.Collections;
using UnityEngine;

#endregion

public class CreateApple : MonoBehaviour
{
    public Rigidbody2D applePrefab;
    public Transform generatePos;
    private bool genStart;
    private bool initStart;

    private void Awake()
    {
        initStart = false;
        genStart = false;
    }

    private void Update()
    {
        if (initStart && !genStart) StartCoroutine("AppleGen");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") initStart = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") initStart = false;
    }

    private IEnumerator AppleGen()
    {
        genStart = true;

        var rb = Instantiate(applePrefab, generatePos.position, generatePos.rotation);
        rb.gravityScale = 0.0001f;
        yield return new WaitForSeconds(3.0f);
        //yield return null;

        genStart = false;
    }
}