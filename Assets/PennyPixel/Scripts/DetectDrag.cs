using System;
using UnityEngine;

public class DetectDrag : MonoBehaviour
{
    private Vector2 startPos, endPos;
    private int minimumDiff;
    public Transform character;
    private Touch touch;

    private void Awake()
    {
        minimumDiff = Screen.height / 8;
    }

    private void Update()
    {
        switch (Input.touchCount)
        {
            case 2:
                touch = Input.GetTouch(1);
                break;
            case 1:
                touch = Input.GetTouch(0);
                break;
        }

        switch (touch.phase)
        {
            case TouchPhase.Began:
                startPos = touch.position;
                break;
            case TouchPhase.Ended:
                endPos = touch.position;

                if (endPos.y - startPos.y > minimumDiff)
                {
                    character.GetComponent<PlayerPlatformerController>().DragJump();
                }

                break;
        }
    }
}
