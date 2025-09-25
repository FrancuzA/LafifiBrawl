using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragUIElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Drag Settings")]
    public float distanceFromTarget;
    [Range(0, 1)] public float alphaThreshold = 0.1f;

    [Header("Visual Feedback")]
    [SerializeField] private float dragScaleFactor = 1.05f;
    [SerializeField] private float scaleSpeed = 10f;
    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isDragging = false;
    private Vector3 offset;
    private Image image;
    private Vector3 originalScale;
    private Vector3 targetScale;

   
    private float originalZPosition;
    private Vector3 originalPosition;
    public Vector3 startPosition;
    

    private Transform originalParent;
    private int originalSiblingIndex;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        startPosition = gameObject.transform.position;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();
        originalScale = rectTransform.localScale;
        targetScale = originalScale;

        originalZPosition = rectTransform.position.z;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        try
        {
            image.alphaHitTestMinimumThreshold = alphaThreshold;
        }
        catch (System.InvalidOperationException)
        {
            Debug.LogWarning("Alpha hit test disabled - texture not readable", this);
        }
    }

    private void OnEnable()
    {
        gameObject.transform.SetLocalPositionAndRotation(new Vector3(0, 0, -0.1f), Quaternion.identity);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            Dependencies.Instance.UnregisterDependency<DragUIElement>();
            Dependencies.Instance.RegisterDependency<DragUIElement>(this);
            originalZPosition = rectTransform.position.z;
            originalPosition = gameObject.transform.position;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint);

            offset = rectTransform.position - worldPoint;
            offset.z = 0;

            isDragging = true;
            targetScale = originalScale * dragScaleFactor;

            originalParent = transform.parent;
            originalSiblingIndex = transform.GetSiblingIndex();

            transform.SetParent(canvas.transform);
            transform.SetAsLastSibling();

            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = false;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        targetScale = originalScale;
        gameObject.transform.SetPositionAndRotation(originalPosition, Quaternion.identity);

        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && canvas.renderMode == RenderMode.WorldSpace)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint);

            worldPoint += (Vector3)offset;
            worldPoint.z = originalZPosition;
            rectTransform.position = worldPoint;
        }
    }

    private void Update()
    {
        if (rectTransform.localScale != targetScale)
        {
            rectTransform.localScale = Vector3.Lerp(
                rectTransform.localScale,
                targetScale,
                Time.deltaTime * scaleSpeed);
        }
    }

}