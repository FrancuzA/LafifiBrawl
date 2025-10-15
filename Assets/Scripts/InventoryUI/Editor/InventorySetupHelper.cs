using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

#if UNITY_EDITOR
/// <summary>
/// Pomocniczy skrypt do automatycznej konfiguracji Grid Inventory
/// Użyj: GameObject → UI → Setup Grid Inventory
/// </summary>
public class InventorySetupHelper : MonoBehaviour
{
    [MenuItem("GameObject/UI/Setup Grid Inventory", false, 0)]
    static void CreateInventorySystem()
    {
        // Znajdź lub stwórz Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Stwórz InventoryPanel
        GameObject panel = new GameObject("InventoryPanel");
        panel.transform.SetParent(canvas.transform, false);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(640, 320);
        
        // Stwórz GridContainer
        GameObject gridContainer = new GameObject("GridContainer");
        gridContainer.transform.SetParent(panel.transform, false);
        RectTransform gridRect = gridContainer.AddComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0, 0);
        gridRect.anchorMax = new Vector2(0, 0);
        gridRect.pivot = new Vector2(0, 0);
        gridRect.anchoredPosition = Vector2.zero;
        
        // Stwórz główny obiekt systemu
        GameObject inventorySystem = new GameObject("InventorySystem");
        
        // Dodaj komponenty
        InventoryGrid invGrid = inventorySystem.AddComponent<InventoryGrid>();
        InventoryUI invUI = inventorySystem.AddComponent<InventoryUI>();
        Test testScript = inventorySystem.AddComponent<Test>();
        
        // Użyj reflection aby ustawić pola prywatne
        var gridType = typeof(InventoryGrid);
        var originField = gridType.GetField("originPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        originField?.SetValue(invGrid, gridContainer.transform);
        
        var uiType = typeof(InventoryUI);
        var gridField = uiType.GetField("inventoryGrid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var containerField = uiType.GetField("gridContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        gridField?.SetValue(invUI, invGrid);
        containerField?.SetValue(invUI, gridContainer.transform);
        
        Selection.activeGameObject = inventorySystem;
        
        Debug.Log("✓ Inventory System utworzony!\n\n" +
                  "Teraz musisz:\n" +
                  "1. Stworzyć prefaby (CellPrefab i ItemPrefab) - zobacz INSTRUKCJA_KONFIGURACJI.txt\n" +
                  "2. Przypisać je w InventoryUI\n" +
                  "3. Stworzyć InventoryItems (Create → Inventory → Item)\n" +
                  "4. Przypisać je w Test Script");
    }
}
#endif

