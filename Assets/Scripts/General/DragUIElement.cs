using System.Collections.Generic;
using General;
using General.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragUIElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Drag Settings")]
    [Range(0, 1)] public float alphaThreshold = 0.1f;

    [Header("Visual Feedback")]
    [SerializeField] private float dragScaleFactor = 1.05f;
    [SerializeField] private float scaleSpeed = 10f;

    [Header("ItemInfo")]
    public List<GameObject> cellsToCheck;
    public Vector3 centerOffsetValue;
    public int cost;
    public TextMeshProUGUI costText;
    public bool isBought = true;
    public UnitsStats stats;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;


    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform cellToSnap;
    private Image image;
    private Vector3 originalPosition;
    private Vector3 startPosition;
    private Vector3 offset;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private float originalZPosition;
    private int requairedSpace;
    private int currentSpaceInGrid = 0;
    private bool isDragging = false;
    private Shopmanager shop;
    private AudioManager Audio;
    
    
    

    private void Awake()
    {
        startPosition = gameObject.transform.position;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();
        originalScale = rectTransform.localScale;
        targetScale = originalScale;
        requairedSpace = cellsToCheck.Count;
        originalZPosition = rectTransform.position.z;
        originalPosition = gameObject.transform.position;
        canvasGroup = GetComponent<CanvasGroup>();
        costText.text = cost.ToString();
        shop = Dependencies.Instance.GetDependency<Shopmanager>();
        Audio = Dependencies.Instance.GetDependency<AudioManager>();
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


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {  
            if (!isBought)
            {
                if (cost > shop.Money) return;
                
                isBought = true; 
                shop.Money -= cost; 
                shop.UpdateMoneyCount();
                Audio.PlayBuySound();
                shop.Add(stats);
            }

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay && isBought)
            {
                if (costText != null) DestroyText();
                Dependencies.Instance.UnregisterDependency<DragUIElement>();
                Dependencies.Instance.RegisterDependency<DragUIElement>(this);
                ResetAvaibleSpsace();
                originalZPosition = rectTransform.position.z;
                
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    rectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector3 worldPoint);

                offset = rectTransform.position - worldPoint;
                offset.z = 0;

                isDragging = true;
                targetScale = originalScale * dragScaleFactor;

                if (canvasGroup != null)
                {
                    canvasGroup.blocksRaycasts = false;
                }

                RemoveItemFromGrid();
            }
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Audio.PlaySellSound();
            shop.Money += cost; 
            shop.UpdateMoneyCount();
            Destroy(gameObject);
            shop.Remove(stats);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        targetScale = originalScale;

        CheckSpace();
        if(currentSpaceInGrid == requairedSpace)
        {
            FindCellToSnap();
            PlaceItemOnGrid();
        }
        else
        {
            gameObject.transform.SetPositionAndRotation(originalPosition, Quaternion.identity);
            if (originalPosition != startPosition) { PlaceItemOnGrid(); }
        }
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
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

    public void CheckSpace()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("No EventSystem found in scene!");
            return;
        }

        GraphicRaycaster raycaster = FindFirstObjectByType<GraphicRaycaster>();
        if (raycaster == null)
        {
            Debug.LogError("No GraphicRaycaster found! Make sure your Canvas has one.");
            return;
        }


        foreach (GameObject cell in cellsToCheck)
        {
            if (cell == null) continue;

            Vector2 screenPosition;

            RectTransform rectTransform = cell.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                screenPosition = rectTransform.position;
            }
            else
            {
                screenPosition = Camera.main.WorldToScreenPoint(cell.transform.position);
            }


            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = screenPosition;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            if (results.Count > 0)
            {
                foreach (RaycastResult result in results)
                {
                    CellScript cellScript = result.gameObject.GetComponent<CellScript>();
                    if (cellScript != null)
                    {
                        cellScript.SendInfoToItem();
                        break;
                    }
                }
            }
            else
            {
                Debug.Log($"No UI elements found at position for cell: {cell.name}");
            }
        }
    }

    public void AddAvaibleSpace(int hasSpace)
    {
        currentSpaceInGrid += hasSpace;
    }

    public void ResetAvaibleSpsace()
    {
        currentSpaceInGrid = 0;
    }

    public void FindCellToSnap() 
    {
            Vector2 screenPosition;
            RectTransform rectTransform = cellsToCheck[0].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                screenPosition = rectTransform.position;
            }
            else
            {
                screenPosition = Camera.main.WorldToScreenPoint(cellsToCheck[0].transform.position);
            }

            GraphicRaycaster raycaster = FindFirstObjectByType<GraphicRaycaster>();

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = screenPosition;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            if (results.Count > 0)
            {
                foreach (RaycastResult result in results)
                {
                  CellScript cellScript = result.gameObject.GetComponent<CellScript>();
                   if (cellScript != null)
                  {
                    cellScript.SendTransformToItem();
                    break;
                  }
                }
            }
            else
            {
                Debug.Log($"No UI elements found at position for cell: {cellsToCheck[0].name}");
            }
        
    }

    public void SetCellToSnap(Transform position)
    {
        cellToSnap = position;
    }

    public void PlaceItemOnGrid()
    {
        gameObject.transform.SetPositionAndRotation(cellToSnap.position + centerOffsetValue, Quaternion.identity);
        originalPosition = gameObject.transform.position;
        if (audioManager == null)audioManager = Dependencies.Instance.GetDependency<AudioManager>();
        audioManager.StopBackpackSound();
        audioManager.PlayBackpackSound();
        foreach (GameObject cell in cellsToCheck)
        {
            if (cell == null) continue;

            Vector2 screenPosition;

            RectTransform rectTransform = cell.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                screenPosition = rectTransform.position;
            }
            else
            {
                screenPosition = Camera.main.WorldToScreenPoint(cell.transform.position);
            }

            GraphicRaycaster raycaster = FindFirstObjectByType<GraphicRaycaster>();

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = screenPosition;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            if (results.Count > 0)
            {
                foreach (RaycastResult result in results)
                {
                    CellScript cellScript = result.gameObject.GetComponent<CellScript>();
                    if (cellScript != null)
                    {
                        cellScript.SetAsOccupiedState();
                        break;
                    }
                }
            }
            else
            {
                Debug.Log($"No UI elements found at position for cell: {cell.name}");
            }
        }
    }

    public void RemoveItemFromGrid()
    {
        foreach (GameObject cell in cellsToCheck)
        {
            if (cell == null) continue;

            Vector2 screenPosition;

            RectTransform rectTransform = cell.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                screenPosition = rectTransform.position;
            }
            else
            {
                screenPosition = Camera.main.WorldToScreenPoint(cell.transform.position);
            }

            GraphicRaycaster raycaster = FindFirstObjectByType<GraphicRaycaster>();

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = screenPosition;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            if (results.Count > 0)
            {
                foreach (RaycastResult result in results)
                {
                    CellScript cellScript = result.gameObject.GetComponent<CellScript>();
                    if (cellScript != null)
                    {
                        cellScript.SetAsbackpackState(); ;
                        break;
                    }
                }
            }
            else
            {
                Debug.Log($"No UI elements found at position for cell: {cell.name}");
            }
        }
    }

    public void DestroyText()
    {
        Destroy(costText.gameObject);
    }

}