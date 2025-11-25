using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Unity.Mathematics;

public class Cell
{
    public Vector2Int gridPos;
    public House content;
    public Vector3 worldPos;
}

public class Grid : Singleton<Grid>
{

    public House _HousePeview { get; private set; } = null;
    [SerializeField] public House _HousLargStartPrefab;
    [SerializeField] public House _HousMediumStartPrefab;
    [SerializeField] public House _HousSmallStartPrefab;
    [SerializeField] public GameObject _HouseCOntainer;
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
        ConstructHome(new Vector2Int(4, 5), _HousMediumStartPrefab, true);
        ConstructHome(new Vector2Int(5, 4), _HousMediumStartPrefab, true);
        ConstructHome(new Vector2Int(5, 5), _HousLargStartPrefab, true);
        ConstructHome(new Vector2Int(5, 6), _HousMediumStartPrefab, true);
        ConstructHome(new Vector2Int(6, 5), _HousMediumStartPrefab, true);

        DialogManager.OnDialogOver += OnDialogueOver;
    }


    void Update()
    {
        if (_CanBuild && _IsHouseSelected)
        {
            Previsualisation();
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                ConstructHome(GetMouseWorldPosition());
            }
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
        if (!_CanBuild) return;

        if (_HousePeview == null)
        {
            _HousePeview = Instantiate(pNewPrefab,Vector3.zero,quaternion.identity,_HouseCOntainer.gameObject.transform);
            _IsHouseSelected = true;
            return;
        }

        if (_HousePeview.name.Contains(pNewPrefab.name))
        {
            AvortConstruction();
            return;
        }

        AvortConstruction();
        _HousePeview = Instantiate(pNewPrefab);
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
        
        House lHouse = _Grid[x, y].content;
        if (lHouse == null || lHouse.isDestroy) return true;
        else return false;
    }

    // ------------------------Construction--------------------------
    private void OnDialogueOver()
    {
        _CanBuild = true;
    }

    private void Previsualisation()
    {
        Vector2Int lCellInGridPos = WorldToCell(GetMouseWorldPosition());
        _HousePeview.transform.position = CellToWorld(lCellInGridPos.x, lCellInGridPos.y);

        bool inside = IsInsideGrid(lCellInGridPos.x, lCellInGridPos.y);
        SetPreviewTransparency(_HousePeview, inside ? .5f : 0.0f);

    }

    private void AvortConstruction()
    {
        if (_HousePeview == null) return;

        Destroy(_HousePeview.gameObject);
        _HousePeview = null;
        _IsHouseSelected = false;
    }

    private void ConstructHome(Vector3 pCellPos)
    {
        if (!_CanBuild || !_IsHouseSelected) return;


        Vector2Int lCellInGridPos = WorldToCell(pCellPos);

        if (IsCellFree(lCellInGridPos.x, lCellInGridPos.y) && ShopManager.Instance.Buy())
        {

            HouseManager.Instance.AddHouseInList(_HousePeview);
            PlaceHouse(_HousePeview, lCellInGridPos.x, lCellInGridPos.y);
            _IsHouseSelected = false;
        }
    }

private void ConstructHome(Vector2Int pCellPos, House pPrefab = null, bool pForceConstruct = false)
{
    if (pForceConstruct || ShopManager.Instance.Buy())
    {
        if (IsCellFree(pCellPos.x, pCellPos.y))
        {
            House houseToPlace = pPrefab ? Instantiate(pPrefab) : _HousePeview;

            HouseManager.Instance.AddHouseInList(houseToPlace);
            PlaceHouse(houseToPlace, pCellPos.x, pCellPos.y);
        }
    }
}

    public bool PlaceHouse(House _House, int x, int y)
    {
        if (!IsCellFree(x, y)) return false;

        _House.transform.position = CellToWorld(x, y);
        _Grid[x, y].content = _House;

        _HousePeview = null;
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

    private void SetPreviewTransparency(House house, float alpha)
    {
        Renderer[] renderers = house.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;

                if (alpha < 1f)
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                else
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            }
        }
    }

    public Vector3 GetRandom3DPosInFreeCells()
    {
        return new Vector3(GetRandomPosInFreeCells().x, 0, GetRandomPosInFreeCells().y);
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

        lRandListIndex = UnityEngine.Random.Range(0, cells.Count - 1);
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