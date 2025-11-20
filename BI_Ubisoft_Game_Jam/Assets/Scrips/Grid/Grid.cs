using UnityEngine;

public class Cell
{
    public Vector2Int gridPos;
    public GameObject content;
}

[ExecuteAlways]
public class Grid : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    private Cell[,] grid;

    private void OnValidate()
    {
        GenerateGrid();
    }

    [SerializeField] public GameObject prefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = GetMouseWorldPosition();
            Vector2Int cell = WorldToCell(mousePos);

            if (IsCellFree(cell.x, cell.y))
            {
                GameObject obj = Instantiate(prefab);
                PlaceObject(obj, cell.x, cell.y);
            }
        }
    }


    private void GenerateGrid()
    {
        grid = new Cell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Cell
                {
                    gridPos = new Vector2Int(x, y),
                    content = null
                };
            }
        }
    }



    private Vector3 GetGridOrigin()
    {
        float offsetX = (width * cellSize) * 0.5f;
        float offsetY = (height * cellSize) * 0.5f;

        return transform.position - new Vector3(offsetX, 0, offsetY);
    }

    public Vector3 CellToWorld(int x, int y)
    {
        Vector3 origin = GetGridOrigin();
        return origin + new Vector3(x * cellSize + cellSize * 0.5f, 0f, y * cellSize + cellSize * 0.5f);
    }

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        Vector3 origin = GetGridOrigin();
        Vector3 localPos = worldPos - origin;

        int x = Mathf.FloorToInt(localPos.x / cellSize);
        int y = Mathf.FloorToInt(localPos.z / cellSize);

        return new Vector2Int(x, y);
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero); 

        if (ground.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }


    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    public bool IsCellFree(int x, int y)
    {
        if (!IsInsideGrid(x, y)) return false;
        return grid[x, y].content == null;
    }

    public bool PlaceObject(GameObject obj, int x, int y)
    {
        if (!IsCellFree(x, y)) return false;

        obj.transform.position = CellToWorld(x, y);
        grid[x, y].content = obj;
        return true;
    }

    // Gizmo

    private void OnDrawGizmos()
    {
        if (grid == null) return;

        Gizmos.color = Color.yellow;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = CellToWorld(x, y);
                Gizmos.DrawWireCube(pos, Vector3.one * cellSize);
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}