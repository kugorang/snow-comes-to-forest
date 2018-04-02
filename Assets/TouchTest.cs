using UnityEngine;

public class TouchTest : MonoBehaviour
{
    Vector2 startPos, endPos;

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetTouch(1).phase == TouchPhase.Began)
        //{
        //    Debug.Log("Second Touch Start : " + Input.GetTouch(1).position);
        //}
        //else if (Input.GetTouch(1).phase == TouchPhase.Ended)
        //{
        //    Debug.Log("Second Touch End : " + Input.GetTouch(1).position);

        //    Debug.Log("Second Move Len : " + Input.GetTouch(1).deltaPosition);
        //}

        if (Input.touchCount == 2)
        {
            Touch touchOne = Input.GetTouch(1);

            switch (touchOne.phase)
            {
                case TouchPhase.Began:
                    startPos = touchOne.position;
                    break;
                case TouchPhase.Ended:
                    endPos = touchOne.position;

                    Debug.Log("endPos - startPos : " + (endPos - startPos));
                    break;
            }
        }
    }
}