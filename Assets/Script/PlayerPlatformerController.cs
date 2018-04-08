using System;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{
    public float maxSpeed = 1.5f;
    public float jumpTakeOffSpeed = 6;

    private SpriteRenderer spriteRenderer;
    //private Animator animator;

    private Vector2 moveStartPos;
    private Touch touchZero, touchOne;
    private int screenHalfX, beforeTouchNum;
    private Vector2 move;
    private bool isLeft;
    //private int lastInput;

    // Use this for initialization
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();

        screenHalfX = Screen.width / 2;
        touchZero.phase = TouchPhase.Canceled;
        touchOne.phase = TouchPhase.Canceled;
        beforeTouchNum = 0;
        isLeft = true;
    }

    protected override void ComputeVelocity()
    {
        move.x = Input.GetAxis("Horizontal");

        switch (Input.touchCount)
        {
            case 2:
                touchZero = Input.GetTouch(0);
                touchOne = Input.GetTouch(1);

                if (beforeTouchNum != Input.touchCount)
                {
                    //Debug.Log("touchZero : " + touchZero.position);
                    Debug.Log("Input 2 - touchZero.phase : " + touchZero.phase);
                    //Debug.Log("touchOne : " + touchOne.position);
                    Debug.Log("Input 2 - touchOne.phase : " + touchOne.phase);
                }

                break;
            case 1:
                touchZero = Input.GetTouch(0);

                if (beforeTouchNum != Input.touchCount)
                {
                    //Debug.Log("touchZero : " + touchZero.position);
                    Debug.Log("Input 1 - touchZero.phase : " + touchZero.phase);
                }

                break;
        }

        // 두 번째 터치
        if (Input.touchCount == 2)
        {
            if (touchZero.phase == TouchPhase.Began
                && touchOne.phase == TouchPhase.Began
                && grounded)
            {
                DragJump();
            }
            else if (touchZero.phase == TouchPhase.Moved
                || touchZero.phase == TouchPhase.Stationary)
            {
                Debug.Log("touchZero Phase - " + touchZero.phase);

                if (touchOne.phase == TouchPhase.Began && grounded)
                {
                    DragJump();
                    Debug.Log("DragJump");
                }

                if (!isLeft)
                {
                    move.x += maxSpeed;
                    Debug.Log("move Right");
                }
                else
                {
                    move.x -= maxSpeed;
                    Debug.Log("move Left");
                }
            }
            else if (touchOne.phase == TouchPhase.Moved
                || touchOne.phase == TouchPhase.Stationary)
            {
                if (touchZero.phase == TouchPhase.Began && grounded)
                {
                    DragJump();
                }

                if (!isLeft)
                {
                    move.x += maxSpeed;
                }
                else
                {
                    move.x -= maxSpeed;
                }
            }
        }
        else if (Input.touchCount == 1)
        {
            //Debug.Log("touchCount : 1");

            switch (touchZero.phase)
            {
                case TouchPhase.Began:
                    moveStartPos = touchZero.position;
                    break;
                case TouchPhase.Stationary:
                case TouchPhase.Moved:
                    if (beforeTouchNum == 2)
                    {
                        moveStartPos = touchZero.position;
                    }

                    if (moveStartPos.x >= screenHalfX)
                    {
                        move.x += maxSpeed;
                        //Debug.Log("asdf");
                        isLeft = false;
                    }
                    else
                    {
                        move.x -= maxSpeed;
                        //Debug.Log("fdsa");
                        isLeft = true;
                    }

                    break;
                case TouchPhase.Ended:
                    moveStartPos = Vector2.zero;
                    touchZero.phase = TouchPhase.Canceled;
                    break;
            }
        }

        beforeTouchNum = Input.touchCount;

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

        //animator.SetBool("grounded", grounded);
        //animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

        //Debug.Log("MoveX Final : " + move.x);

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