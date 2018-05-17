using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_modified : MonoBehaviour {

    public GameObject player;
    public int height;
    public string nextStage;
    public string tag;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(tag))
        {
            SceneManager.LoadScene(nextStage);
        }
    }

    void FixedUpdate()
    {
        Restart();
    }

    private void Restart()
    {
        if (player.transform.position.y < height)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


}
