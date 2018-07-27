#region

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class UIAutoscroll : MonoBehaviour
{
    public float duration = 30.0f;
    public Scrollbar scrollbar;

    public ScrollRect scrollRect;
    public float scrollValue;

    private void OnEnable()
    {
        StartCoroutine(Scroller());
    }

    private IEnumerator Scroller()
    {
        var t = 0.0f;
        while (true)
        {
            t += Time.deltaTime / duration;
            scrollRect.verticalNormalizedPosition = 1 - Mathf.PingPong(t, 1);
            yield return null;
        }
    }
}