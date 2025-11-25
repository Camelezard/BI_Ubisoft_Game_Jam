using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector2Int gridPos;
    public House content;
    public Vector3 worldPos;
}

public class Grid : Singleton<Grid>
{
    [SerializeField] public House _HousPrefab;
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    private bool _CanBuild = false;
    private bool _IsHouseSelected = false;

    private Cell[,] _Grid;

    private void OnValidate()
    {
        GenerateGrid();
    }

    void Start()
    {
        ConstructHome(new Vector2Int(4, 5),true);
        ConstructHome(new Vector2Int(5, 4),true);
        ConstructHome(new Vector2Int(5, 5),true);
        ConstructHome(new Vector2Int(5, 6),true);
        ConstructHome(new Vector2Int(6, 5),true);

        DialogManager.OnDialogOver += OnDialogueOver;
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
                    content = null,
                    worldPos = CellToWorld(x, y)
                };
            }
        }
    }

    public void ChangSelectHouse(House pNewPrefab)
    {
        _HousPrefab = pNewPrefab;
        _IsHouseSelected = true;
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

    // ------------------------Construction--------------------------
    private void OnDialogueOver()
    {
        _CanBuild = true;
    }

    private void ConstructHome(Vector3 pCellPos)
    {
        if (!_CanBuild || !_IsHouseSelected) return;

        House _House;
        Vector2Int lCellInGridPos = WorldToCell(pCellPos);

        if (IsCellFree(lCellInGridPos.x, lCellInGridPos.y) && ShopManager.Instance.Buy())
        {
            _House = Instantiate(_HousPrefab);
            HouseManager.Instance.AddHouseInList(_House);
            PlaceHouse(_House, lCellInGridPos.x, lCellInGridPos.y);
        }

        _IsHouseSelected = false;
    }

    private void ConstructHome(Vector2Int pCellPos, bool pForceConstruct = false)
    {
        if (pForceConstruct || ShopManager.Instance.Buy())
        {

            House _House;
            if (IsCellFree(pCellPos.x, pCellPos.y))
            {

                _House = Instantiate(_HousPrefab);
                HouseManager.Instance.AddHouseInList(_House);
                PlaceHouse(_House, pCellPos.x, pCellPos.y);
            }
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

    public Vector3 GetRandom3DPosInFreeCells()
    {
       return new Vector3( GetRandomPosInFreeCells().x,0,GetRandomPosInFreeCells().y);
    }
    public Vector2 GetRandomPosInFreeCells()
    {
        List<Cell> cells = new List<Cell>();
        Vector3 lPos;
        Cell lCell;
        Cell lRanCell;
        int lRandListIndex = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                lCell = _Grid[x, y];
                if (lCell.content == null)
                {
                    cells.Add(lCell);
                }

                lPos = CellToWorld(x, y);
            }
        }

        lRandListIndex = Random.Range(0, cells.Count - 1);
        lRanCell = cells[lRandListIndex];

        //print ("total in cell = " + cells.Count);

        foreach (Cell cell in cells)
        {
            Debug.DrawLine(cell.worldPos, cell.worldPos + Vector3.up, Color.red, 1);
        }

        Vector2 lFialRandPos = new Vector2(lRanCell.worldPos.x, lRanCell.worldPos.z);
        return lFialRandPos;
    }

    void OnDestroy()
    {
        DialogManager.OnDialogOver += OnDialogueOver;
    }
}