using UnityEngine;

public class Cell
{
    public Vector2Int gridPos;
    public House content;
}

public class Grid : MonoBehaviour
{
    [SerializeField] public House _HousPrefab;
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    private Cell[,] _Grid;

    private void OnValidate()
    {
        GenerateGrid();
    }

    void Start()
    {

        ConstructHome(new Vector2Int (3,4));
        ConstructHome(new Vector2Int (4,3));
        ConstructHome(new Vector2Int (4,4)); 
        ConstructHome(new Vector2Int (4,5)); 
        ConstructHome(new Vector2Int (5,4));
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ConstructHome(GetMouseWorldPosition());
        }
    }


    private void GenerateGrid()
    {
        _Grid = new Cell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _Grid[x, y] = new Cell
                {
                    gridPos = new Vector2Int(x, y),
                    content = null
                };
            }
        }
    }



    private Vector3 GetGridOrigin()
    {
        float lOffsetX = (width * cellSize) * 0.5f;
        float lOffsetY = (height * cellSize) * 0.5f;

        return transform.position - new Vector3(lOffsetX, 0, lOffsetY);
    }

    public Vector3 CellToWorld(int x, int y)
    {
        Vector3 lOrigin = GetGridOrigin();
        return lOrigin + new Vector3(x * cellSize + cellSize * 0.5f, 0f, y * cellSize + cellSize * 0.5f);
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
        Ray lRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane lGround = new Plane(Vector3.up, Vector3.zero);

        if (lGround.Raycast(lRay, out float distance))
        {
            return lRay.GetPoint(distance);
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
        return _Grid[x, y].content == null;
    }

    private void ConstructHome(Vector3 pCellPos)
    {
        House _House;
        Vector2Int lCellInGridPos = WorldToCell(pCellPos);

        if (IsCellFree(lCellInGridPos.x, lCellInGridPos.y))
        {
            _House = Instantiate(_HousPrefab);
            HouseManager.Instance.AddHouseInList(_House);
            PlaceHouse(_House, lCellInGridPos.x, lCellInGridPos.y);
        }
    }

    private void ConstructHome(Vector2Int pCellPos)
    {
        House _House;
        if (IsCellFree(pCellPos.x, pCellPos.y))
        {

            _House = Instantiate(_HousPrefab);
            HouseManager.Instance.AddHouseInList(_House);
            PlaceHouse(_House, pCellPos.x, pCellPos.y);
        }
    }

    public bool PlaceHouse(House _House, int x, int y)
    {
        if (!IsCellFree(x, y)) return false;

        _House.transform.position = CellToWorld(x, y);
        _Grid[x, y].content = _House;
        return true;
    }

    // Gizmo

    private void OnDrawGizmos()
    {
        if (_Grid == null) return;

        Vector3 lPos;
        Gizmos.color = Color.yellow;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                lPos = CellToWorld(x, y);
                Gizmos.DrawWireCube(lPos, Vector3.one * cellSize);
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}