#region

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class FadeOut : MonoBehaviour
{
    private readonly float end = 1f;

    private readonly float start = 0f;
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

        StartCoroutine("PlayFadeOut");
    }

    private IEnumerator PlayFadeOut()
    {
        isPlaying = true;

        var color = fadeImage.color;
        time = 0f;
        color.a = Mathf.Lerp(start, end, time);

        while (color.a < 1f)
        {
            time += Time.deltaTime / animTime;

            color.a = Mathf.Lerp(start, end, time);

            fadeImage.color = color;

            yield return null;
        }

        isPlaying = false;
    }
}