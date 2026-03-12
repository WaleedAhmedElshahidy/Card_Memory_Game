using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridLayoutConfig : MonoBehaviour
{
    // YuGiOh card ratio is portrait 2:3
    private const float CardWidthRatio = 2f;
    private const float CardHeightRatio = 3f;

    private GridLayoutGroup grid;

    private void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
    }

    public void SetupGrid(int totalCards, RectTransform container)
    {
        int cols = GetColumns(totalCards);
        int rows = Mathf.CeilToInt((float)totalCards / cols);

        float containerWidth = container.rect.width;
        float containerHeight = container.rect.height;

        float spacingX = grid.spacing.x;
        float spacingY = grid.spacing.y;

        // calculate max cell size that fits container
        float cellWidth = (containerWidth - spacingX * (cols - 1) - grid.padding.left - grid.padding.right) / cols;
        float cellHeight = (containerHeight - spacingY * (rows - 1) - grid.padding.top - grid.padding.bottom) / rows;

        // enforce portrait aspect ratio 2:3
        float ratioFromWidth = cellWidth * (CardHeightRatio / CardWidthRatio);
        float ratioFromHeight = cellHeight * (CardWidthRatio / CardHeightRatio);

        // use whichever is smaller so cards always fit
        if (ratioFromWidth <= cellHeight)
        {
            cellHeight = ratioFromWidth;
        }
        else
        {
            cellWidth = ratioFromHeight;
        }

        grid.cellSize = new Vector2(cellWidth, cellHeight);
        grid.constraintCount = cols;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
    }

    private int GetColumns(int totalCards)
    {
        if (totalCards == 4) return 2;
        if (totalCards == 6) return 3;
        if (totalCards == 8) return 4;
        if (totalCards == 9) return 3;
        if (totalCards == 12) return 4;
        if (totalCards == 16) return 4;
        if (totalCards == 20) return 5;
        if (totalCards == 30) return 6;

        return Mathf.CeilToInt(Mathf.Sqrt(totalCards));
    }
}