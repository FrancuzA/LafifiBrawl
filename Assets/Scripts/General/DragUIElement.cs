using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isDragging = false;
    private Vector3 offset;
    private Image image;
    private Vector3 originalScale;
    private Vector3 targetScale;

    private CanvasGroup canvasGroup;
    private float originalZPosition;
    private Vector3 originalPosition;
    public Vector3 startPosition;
    public Vector3 centerOffsetValue;

    [Header("ItemInfo")]
    public List<GameObject> cellsToCheck;
    public int requairedSpace;
    public int currentSpaceInGrid = 0;
    public LayerMask layer;
    public Transform cellToSnap;

    [Header("Audio")]
    public EventReference backpackSoundRef;
    public EventInstance backpackSound;

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
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
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

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        targetScale = originalScale;

        CheckSpace();
        if(currentSpaceInGrid == requairedSpace)
        {
            Debug.Log("test");
            FindCellToSnap();
            PlaceItemOnGrid();
        }
        else
        {
            gameObject.transform.SetPositionAndRotation(originalPosition, Quaternion.identity);
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

        GraphicRaycaster raycaster = FindObjectOfType<GraphicRaycaster>();
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

            GraphicRaycaster raycaster = FindObjectOfType<GraphicRaycaster>();

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
        backpackSound = RuntimeManager.CreateInstance(backpackSoundRef);
        backpackSound.start();
        backpackSound.release();
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

            GraphicRaycaster raycaster = FindObjectOfType<GraphicRaycaster>();

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

            GraphicRaycaster raycaster = FindObjectOfType<GraphicRaycaster>();

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

}