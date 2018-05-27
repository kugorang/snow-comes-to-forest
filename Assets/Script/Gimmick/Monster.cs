using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Monster : MonoBehaviour {

    public float leftmax;
    public float rightmax;
    public float speed;

    public bool isright;

    private SpriteRenderer spriteRenderer;

 void Update()
    {
        Move();
    }

    void Move()
    {
        if (this.transform.position.x < leftmax)
        {
            isright = true;
        }
        else if (this.transform.position.x > rightmax)
        {
            isright = false;
        }

        if (isright == true)
        {
            this.transform.Translate(Vector2.right * speed * Time.deltaTime, Space.World);
        }

        else if (isright == false)
        {
            this.transform.Translate(Vector2.left * speed * Time.deltaTime, Space.World);
        }
    }

}
