using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(ItemShape))]
public class ItemShapeDrawer : PropertyDrawer
{
    private const float CELL_SIZE = 20f;
    private const float GRID_SIZE = 5f;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        var occupiedCellsProp = property.FindPropertyRelative("occupiedCells");
        
        // Etykieta
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        
        // Przyciski do dodawania predefiniowanych kształtów
        var buttonRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        
        EditorGUI.BeginChangeCheck();
        
        GUILayout.BeginHorizontal();
        /*if (GUILayout.Button("1x1"))
        {
            SetShape(occupiedCellsProp, ItemShape.CreateSingleCell().occupiedCells);
        }
        if (GUILayout.Button("2x1"))
        {
            SetShape(occupiedCellsProp, ItemShape.CreateRectangle(2, 1).occupiedCells);
        }
        if (GUILayout.Button("1x2"))
        {
            SetShape(occupiedCellsProp, ItemShape.CreateRectangle(1, 2).occupiedCells);
        }
        if (GUILayout.Button("2x2"))
        {
            SetShape(occupiedCellsProp, ItemShape.CreateRectangle(2, 2).occupiedCells);
        }*/
        if (GUILayout.Button("Clear"))
        {
            occupiedCellsProp.ClearArray();
            occupiedCellsProp.InsertArrayElementAtIndex(0);
            var element = occupiedCellsProp.GetArrayElementAtIndex(0);
            element.vector2IntValue = Vector2Int.zero;
        }
        GUILayout.EndHorizontal();
        
        // Siatka do ręcznego edytowania
        var gridRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 5, GRID_SIZE * CELL_SIZE, GRID_SIZE * CELL_SIZE);
        
        DrawGrid(gridRect, occupiedCellsProp);
        
        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }
        
        EditorGUI.EndProperty();
    }
    
    private void DrawGrid(Rect gridRect, SerializedProperty occupiedCellsProp)
    {
        // Pobierz obecne zajęte komórki
        var occupiedCells = new HashSet<Vector2Int>();
        for (int i = 0; i < occupiedCellsProp.arraySize; i++)
        {
            occupiedCells.Add(occupiedCellsProp.GetArrayElementAtIndex(i).vector2IntValue);
        }
        
        // Rysuj siatkę i obsługuj kliknięcia
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                var cellRect = new Rect(
                    gridRect.x + x * CELL_SIZE,
                    gridRect.y + y * CELL_SIZE,
                    CELL_SIZE - 1,
                    CELL_SIZE - 1
                );
                
                var cellPos = new Vector2Int(x, y);
                bool isOccupied = occupiedCells.Contains(cellPos);
                
                // Rysuj komórkę
                EditorGUI.DrawRect(cellRect, isOccupied ? Color.green : Color.gray);
                
                // Obsługa kliknięcia
                if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
                {
                    if (isOccupied)
                    {
                        // Usuń komórkę
                        RemoveCell(occupiedCellsProp, cellPos);
                    }
                    else
                    {
                        // Dodaj komórkę
                        AddCell(occupiedCellsProp, cellPos);
                    }
                    Event.current.Use();
                }
            }
        }
    }
    
    private void SetShape(SerializedProperty occupiedCellsProp, List<Vector2Int> cells)
    {
        occupiedCellsProp.ClearArray();
        for (int i = 0; i < cells.Count; i++)
        {
            occupiedCellsProp.InsertArrayElementAtIndex(i);
            occupiedCellsProp.GetArrayElementAtIndex(i).vector2IntValue = cells[i];
        }
    }
    
    private void AddCell(SerializedProperty occupiedCellsProp, Vector2Int cell)
    {
        int index = occupiedCellsProp.arraySize;
        occupiedCellsProp.InsertArrayElementAtIndex(index);
        occupiedCellsProp.GetArrayElementAtIndex(index).vector2IntValue = cell;
    }
    
    private void RemoveCell(SerializedProperty occupiedCellsProp, Vector2Int cell)
    {
        for (int i = 0; i < occupiedCellsProp.arraySize; i++)
        {
            if (occupiedCellsProp.GetArrayElementAtIndex(i).vector2IntValue == cell)
            {
                occupiedCellsProp.DeleteArrayElementAtIndex(i);
                break;
            }
        }
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight + GRID_SIZE * CELL_SIZE + 10;
    }
}
