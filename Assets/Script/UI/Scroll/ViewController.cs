using UnityEngine;


[RequireComponent(typeof(RectTransform))] // Rect Transform 컴포넌트는 필수로 존재한다.
public class ViewController : MonoBehaviour {

    // Rect Transform 컴포넌트를 캐시한다.
    private RectTransform cachedRectTransform;

    public RectTransform CachedRectTransform
    {
        get
        {
            if (cachedRectTransform == null)
            {
                cachedRectTransform = GetComponent<RectTransform>();
            }
            return cachedRectTransform;
            
        }
    }
    
    // 뷰의 타이틀을 가져와서 설정하는 프로퍼티
    public virtual string Title
    {
        get { return ""; }
        set { }
    }
}
