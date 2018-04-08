using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionItem : PhysicsObject
{
    public float maxSpeed = 1.5f;
    public float jumpTakeOffSpeed = 6;
    public float jumpMaxValue;

    private SpriteRenderer spriteRenderer;
    private Vector2 move;
    private int collisionNum;
    private bool isMoved;

    private void Awake()
    {
        move = Vector2.zero;
        collisionNum = 0;
        isMoved = false;
    }

    protected override void ComputeVelocity()
    {
        if (!isMoved && collisionNum == 3)
        {
            isMoved = true;
            collisionNum = 0;

            if (!grounded)
            {
                if (velocity.y > 0)
                {
                    velocity.y *= 0.5f;
                }

                gravityModifier = 1.0f;
                Debug.Log("!grounded");

                targetVelocity = move * maxSpeed;
            }
            else
            {
                gravityModifier = 0.0f;
                Debug.Log("grounded");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collisionNum += 1;

            if (grounded)
            {
                collision.GetComponent<PlayerPlatformerController>().jumpTakeOffSpeed *= jumpMaxValue;
            }

            GameObject.Find("WindZone").GetComponent<WindZone>().windPulseMagnitude = 2;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerPlatformerController>().jumpTakeOffSpeed = 6;

            GameObject.Find("WindZone").GetComponent<WindZone>().windPulseMagnitude = 1;
        }
    }
}
