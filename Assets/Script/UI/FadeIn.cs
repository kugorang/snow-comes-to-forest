#region

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class FadeIn : MonoBehaviour
{
    private readonly float end = 0f;

    private readonly float start = 1f;
    public float animTime = 2f;

    private Image fadeImage;

    private bool isPlaying;
    private float time;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    public void StartFadeAnim()
    {
        Debug.Log("버튼 눌림");

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
    }
}