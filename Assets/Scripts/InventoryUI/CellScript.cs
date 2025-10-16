using UnityEngine;

public class CellScript : MonoBehaviour
{
    public int CellState = 0; // 0 - empty, 1 - backpackSpace, 2 - occupied 
    

    public void SendInfoToItem()
    {
        if (CellState == 1) 
        {
            Dependencies.Instance.GetDependancy<DragUIElement>().AddAvaibleSpace(1);
        }
    }

    public void SendTransformToItem() 
    {
        Dependencies.Instance.GetDependancy<DragUIElement>().SetCellToSnap(gameObject.transform);
    }
    public void SetEmptyState()
    {
        CellState = 0;
    }
    public void SetAsbackpackState()
    {
        CellState = 1;
    }

    public void SetAsOccupiedState()
    {
        CellState = 2;
    }
}
