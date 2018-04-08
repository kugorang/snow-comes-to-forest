using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    [SerializeField]
    private float speed;

    private Vector2 direction;

    private Transform tr;

    private Animator anim;

    private Rigidbody2D rb2d;

    private bool isGround = true;

    public GameObject player;
    public int height;

    public GameObject bubble;

    // Use this for initialization
    void Start () {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        GetInput();
        Move();
        Restart();
	}

    public void Move()
    {
        transform.Translate(direction*speed*Time.deltaTime);
    }

    private void GetInput()
    {
        direction = Vector2.zero;

        if (Input.GetKey(KeyCode.A))
        {
            anim.SetBool("isRun", true);
            direction += Vector2.left;
            tr.localScale = new Vector3(-1, 1, 1);
        }
        if (Input.GetKey(KeyCode.D))
        {
            anim.SetBool("isRun", true);
            direction += Vector2.right;
            tr.localScale = new Vector3(1, 1, 1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (isGround)
            {
                isGround = false;
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0f) + new Vector2(rb2d.velocity.x, transform.up.y) * 5;
            }
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            anim.SetBool("isRun", false);
        }


    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if(other.gameObject.CompareTag("carrot"))
        {
            other.gameObject.SetActive(false);
        }

    }

    void OnTriggerStay2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("interactObj"))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                bubble.SetActive(true);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        isGround = true;
    }

    private void Restart()
    {
        if (player.transform.position.y < height)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

}
