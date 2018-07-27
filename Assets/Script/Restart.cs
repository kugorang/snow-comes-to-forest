#region

using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

public class Restart : MonoBehaviour
{
    public int height;

    public GameObject player;

    private void FixedUpdate()
    {
        if (player.transform.position.y < height) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}