using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    public GameObject girlfriend;
    public GameObject bus;

    public float girlSpeed;
    public float girlTime;

    public float busSpeed;

    void FixedUpdate () {

        if (Time.time < girlTime)
        {
            GirlfriendMove();
        }
        else girlfriend.SetActive(false);


        //BusMove(); //수정 필요 (15초 후에 버스가 나타나서 이동하도록. Invoke("GameObject",시간); 사용



		
	}



    private void GirlfriendMove()
    {
        girlfriend.transform.Translate(Vector2.right * girlSpeed * Time.deltaTime, Space.World);
    }

    //private void BusMove()
    //{
    //    bus.SetActive(true);
    //    bus.transform.Translate(Vector2.right * girlSpeed * Time.deltaTime, Space.World);
    //
    //
    //}


	
}
