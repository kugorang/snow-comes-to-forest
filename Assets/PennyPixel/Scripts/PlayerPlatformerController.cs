using System;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{
    public float maxSpeed = 1.5f;
    public float jumpTakeOffSpeed = 6;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector2 moveStartPos, jumpStartPos;
    private Touch touchZero, touchOne;
    private int minDiffX, minDiffY;
    private Vector2 move;

    // Use this for initialization
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        minDiffX = Screen.width / 8;
        minDiffY = Screen.height / 8;
    }

    protected override void ComputeVelocity()
    {
        move.x = Input.GetAxis("Horizontal");

        switch (Input.touchCount)
        {
            case 2:
                touchOne = Input.GetTouch(1);
                break;
            case 1:
                touchZero = Input.GetTouch(0);
                break;
        }

        switch (touchOne.phase)
        {
            case TouchPhase.Began:
                jumpStartPos = touchOne.position;
                break;
            case TouchPhase.Ended:
                Vector2 endPos = touchOne.position;

                if (endPos.y - jumpStartPos.y > minDiffY)
                {
                    DragJump();
                }

                //Debug.Log("PP : touchOne gap - " + (endPos.y - jumpStartPos.y));

                jumpStartPos = Vector2.zero;
                //move = Vector2.zero;
                touchOne.phase = TouchPhase.Canceled;
                break;
        }

        switch (touchZero.phase)
        {
            case TouchPhase.Began:
                moveStartPos = touchZero.position;
                break;
            case TouchPhase.Stationary:
            case TouchPhase.Moved:
                float nowPosX = touchZero.position.x;

                if (Math.Abs(nowPosX - moveStartPos.x) > minDiffX)
                {
                    if (nowPosX > moveStartPos.x)
                    {
                        move.x += maxSpeed;
                    }
                    else if (moveStartPos.x > nowPosX)
                    {
                        move.x -= maxSpeed;
                    }
                }
                break;
            case TouchPhase.Ended:
                Vector2 endPos = touchZero.position;

                if (Input.touchCount == 1 && endPos.y - moveStartPos.y > minDiffY)
                {
                    DragJump();
                    move = Vector2.zero;
                }

                //Debug.Log("PP : touchZero gap - " + (endPos.y - moveStartPos.y));

                touchZero.phase = TouchPhase.Canceled;
                moveStartPos = Vector2.zero;
                break;
        }

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y *= 0.5f;
            }
        }

        if (move.x > 0.01f)
        {
            if (spriteRenderer.flipX)
            {
                spriteRenderer.flipX = false;
            }
        }
        else if (move.x < -0.01f)
        {
            if (!spriteRenderer.flipX)
            {
                spriteRenderer.flipX = true;
            }
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

        targetVelocity = move * maxSpeed;
    }

    public void DragJump()
    {
        if (grounded)
        {
            velocity.y = jumpTakeOffSpeed;
        }
        else
        {
            if (velocity.y > 0)
            {
                velocity.y *= 0.5f;
            }
        }
    }
}