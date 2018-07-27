#region

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#endregion

public class Stage3_modified : MonoBehaviour
{
    private readonly float end = 0f;
    private readonly bool isClear = false;

    private readonly float start = 1f;
    private bool alreadyHave;

    public float animTime = 2f;

    public GameObject fade;
    private Image fadeImage;

    private bool getCard;

    private bool isPlaying;
    public GameObject textBox;
    private float time;

    private void Awake()
    {
        fadeImage = fade.GetComponent<Image>();
    }

    private void Start()
    {
        StartFadeInAnim();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!alreadyHave)
            if (other.CompareTag("card"))
            {
                getCard = true;
                alreadyHave = true;
                other.gameObject.SetActive(false);
            }


        if (other.CompareTag("teddy"))
        {
            if (isClear)
            {
                //가동교 고쳐지고 앞으로 갈 수 있는 코드
            }
            else
            {
                textBox.SetActive(true);
                GetComponent<PlayerPlatformerController>().enabled = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "monster")
        {
            if (getCard && other.contacts[0].normal.y > 0.9f)
            {
                other.gameObject.SetActive(false);
                getCard = false;
                alreadyHave = false;
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public void NextBtnPress()
    {
        textBox.SetActive(false);
        GetComponent<PlayerPlatformerController>().enabled = true;
    }

    public void StartFadeInAnim()
    {
        Debug.Log("fade in");

        if (isPlaying)
            return;

        StartCoroutine("PlayFadeIn");
    }

    private IEnumerator PlayFadeIn()
    {
        isPlaying = true;

        var color = fadeImage.color;
        time = 0f;
        color.a = Mathf.Lerp(start, end, time);

        while (color.a > 0f)
        {
            time += Time.deltaTime / animTime;

            color.a = Mathf.Lerp(start, end, time);

            fadeImage.color = color;

            yield return null;
        }

        isPlaying = false;
        gameObject.GetComponent<PlayerPlatformerController>().enabled = true;
    }
}