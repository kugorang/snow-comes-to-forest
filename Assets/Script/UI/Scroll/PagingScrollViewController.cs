#region

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#endregion

[RequireComponent(typeof(ScrollRect))]
public class PagingScrollViewController :
    ViewController, IBeginDragHandler, IEndDragHandler
// ViewController 클래스를 상속하고 IBeginDragHandler 인터페이스와
// IEndDragHandler인터페이스를 상속한다
{
    private AnimationCurve animationCurve; // 자동 스크롤에 관련된 애니메이션 커브
    [SerializeField] private float animationDuration = 0.3f;

    // ScrollRect컴포넌트를 캐시한다
    private ScrollRect cachedScrollRect;

    private Rect currentViewRect; // 스크롤 뷰의 사각형 크기
    private Vector2 destPosition; // 최종적인 스크롤 위치
    private Vector2 initialPosition; // 자동 스크롤을 시작할 때의 스크롤 위치

    private bool isAnimating; // 애니메이션 재생 중임을 나타내는 플래그
    [SerializeField] private float key1InTangent;
    [SerializeField] private float key1OutTangent = 0.1f;
    [SerializeField] private float key2InTangent;
    [SerializeField] private float key2OutTangent;
    private int prevPageIndex; // 이전 페이지의 인덱스

    public ScrollRect CachedScrollRect
    {
        get
        {
            if (cachedScrollRect == null) cachedScrollRect = GetComponent<ScrollRect>();
            return cachedScrollRect;
        }
    }

    // 드래그가 시작될 때 호출된다
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 애니메이션 도중에 플래그를 리셋한다
        isAnimating = false;
    }

    // 드래그가 끝날 때 호출된다
    public void OnEndDrag(PointerEventData eventData)
    {
        var grid = CachedScrollRect.content.GetComponent<GridLayoutGroup>();

        // 현재 동작 중인 스크롤 뷰를 멈춘다
        CachedScrollRect.StopMovement();

        // GridLayoutGroup의 cellSize와 spacing을 이용하여 한 페이지의 폭을 계산한다
        var pageWidth = -(grid.cellSize.x + grid.spacing.x);

        // 스크롤의 현재 위치로부터 맞출 페이지의 인덱스를 계산한다
        var pageIndex =
            Mathf.RoundToInt(CachedScrollRect.content.anchoredPosition.x / pageWidth);

        if (pageIndex == prevPageIndex && Mathf.Abs(eventData.delta.x) >= 1)
        {
            // 일정 속도 이상으로 드래그할 경우 해당 방향으로 한 페이지 진행시킨다
            CachedScrollRect.content.anchoredPosition +=
                new Vector2(eventData.delta.x, 0.0f);
            pageIndex += (int) Mathf.Sign(-eventData.delta.x);
        }

        // 첫 페이지 또는 끝 페이지일 경우에는 그 이상 스크롤하지 않도록 한다
        if (pageIndex < 0)
            pageIndex = 0;
        else if (pageIndex > grid.transform.childCount - 1) pageIndex = grid.transform.childCount - 1;

        prevPageIndex = pageIndex; // 현재 페이지의 인덱스를 유지한다

        // 최종적인 스크롤 위치를 계산한다
        var destX = pageIndex * pageWidth;
        destPosition = new Vector2(destX, CachedScrollRect.content.anchoredPosition.y);

        // 시작할 때의 스크롤 위치를 저장해둔다
        initialPosition = CachedScrollRect.content.anchoredPosition;

        // 애니메이션 커브를 작성한다
        var keyFrame1 = new Keyframe(Time.time, 0.0f, key1InTangent, key1OutTangent);
        var keyFrame2 = new Keyframe(Time.time + animationDuration, 1.0f, key2InTangent, key2OutTangent);
        animationCurve = new AnimationCurve(keyFrame1, keyFrame2);

        // 애니메이션 재생 중임을 나타내는 플래그를 설정한다
        isAnimating = true;
    }


    // 매 프레임마다 Update 메서드가 처리된 다음에 호출된다
    private void LateUpdate()
    {
        if (isAnimating)
        {
            if (Time.time >= animationCurve.keys[animationCurve.length - 1].time)
            {
                // 애니메이션 커브의 마지막 키프레임을 지나가면 애니메이션을 끝낸다
                CachedScrollRect.content.anchoredPosition = destPosition;
                isAnimating = false;
                return;
            }

            // 애니메이션 커브를 사용하여 현재 스크롤 위치를 계산해서 스크롤 뷰를 이동시킨다
            var newPosition = initialPosition +
                              (destPosition - initialPosition) * animationCurve.Evaluate(Time.time);
            CachedScrollRect.content.anchoredPosition = newPosition;
        }
    }

    // 인스턴스를 로드할 때 Awake 메서드가 처리된 다음에 호출된다
    private void Start()
    {
        // 「Scroll Content」のPaddingを初期化する
        UpdateView();
    }

    // 매 프레임마다 호출된다
    private void Update()
    {
        if (CachedRectTransform.rect.width != currentViewRect.width ||
            CachedRectTransform.rect.height != currentViewRect.height)
            UpdateView();
    }

    // Scroll Content의 Padding을 갱신한는 메서드
    private void UpdateView()
    {
        // 스크롤 뷰의 사각형 크기를 보존해둔다
        currentViewRect = CachedRectTransform.rect;

        // GridLayoutGroup의 cellSize를 사용하여 Scroll Content의 Padding을 계산하여 설정한다
        var grid = CachedScrollRect.content.GetComponent<GridLayoutGroup>();
        var paddingH = Mathf.RoundToInt((currentViewRect.width - grid.cellSize.x) / 2.0f);
        var paddingV = Mathf.RoundToInt((currentViewRect.height - grid.cellSize.y) / 2.0f);
        grid.padding = new RectOffset(paddingH, paddingH, paddingV, paddingV);
    }
}