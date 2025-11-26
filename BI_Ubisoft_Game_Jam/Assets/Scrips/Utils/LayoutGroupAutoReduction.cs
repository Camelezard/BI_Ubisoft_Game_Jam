using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
public class LayoutGroupAutoReduction : MonoBehaviour
{
    private GridLayoutGroup _Grid;
    private RectTransform _RectTransform;

    public Vector2 defaultCellSize = new Vector2(100, 100);

    void Awake()
    {
        _Grid = GetComponent<GridLayoutGroup>();
        _RectTransform = GetComponent<RectTransform>();
    }

    public void OnNumberOfHouseChange()
    {
        Fit();
    }

    void Fit()
    {
        int childCount = transform.childCount;
        if (childCount == 0)
            return;

        float width = _RectTransform.rect.width;
        float height = _RectTransform.rect.height;

        float bestCellSize = 0f;
        int bestColumns = 1;
        int bestRows = childCount;

        for (int cols = 1; cols <= childCount; cols++)
        {
            int rows = Mathf.CeilToInt((float)childCount / cols);

            float cellWidth = (width
                - _Grid.padding.left - _Grid.padding.right
                - _Grid.spacing.x * (cols - 1)) / cols;

            float cellHeight = (height
                - _Grid.padding.top - _Grid.padding.bottom
                - _Grid.spacing.y * (rows - 1)) / rows;

            float cellSize = Mathf.Min(cellWidth, cellHeight);

            if (cellSize > bestCellSize)
            {
                bestCellSize = cellSize;
                bestColumns = cols;
                bestRows = rows;
            }
        }

        if (bestCellSize >= defaultCellSize.x)
        {
            _Grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _Grid.constraintCount = Mathf.FloorToInt(
                (_RectTransform.rect.width + _Grid.spacing.x) / (defaultCellSize.x + _Grid.spacing.x)
            );

            _Grid.cellSize = defaultCellSize;
        }
        else
        {
            _Grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _Grid.constraintCount = bestColumns;
            _Grid.cellSize = new Vector2(bestCellSize, bestCellSize);
        }
    }
}