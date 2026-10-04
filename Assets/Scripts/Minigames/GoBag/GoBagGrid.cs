using System.Collections.Generic;
using UnityEngine;

public class GoBagGrid
{
    private readonly int width;
    private readonly int height;
    private readonly GoBagItem[,] cells;
    private readonly Dictionary<GoBagItem, Vector2Int> itemPositions = new Dictionary<GoBagItem, Vector2Int>();

    public int Width => width;
    public int Height => height;

    public GoBagGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        cells = new GoBagItem[width, height];
    }

    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    public bool CanPlace(GoBagItem item, int x, int y)
    {
        if (item == null) return false;

        for (int i = 0; i < item.Width; i++)
        {
            for (int j = 0; j < item.Height; j++)
            {
                int cellX = x + i;
                int cellY = y + j;

                if (!IsInsideGrid(cellX, cellY)) return false;

                GoBagItem other = cells[cellX, cellY];
                if (other != null && other != item) return false;
            }
        }
        return true;
    }

    public bool Place(GoBagItem item, int x, int y)
    {
        if (!CanPlace(item, x, y)) return false;

        Remove(item);

        for (int i = 0; i < item.Width; i++)
        {
            for (int j = 0; j < item.Height; j++)
            {
                cells[x + i, y + j] = item;
            }
        }

        itemPositions[item] = new Vector2Int(x, y);
        return true;
    }

    public bool TryPlaceAnywhere(GoBagItem item, out Vector2Int position)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (Place(item, x, y))
                {
                    position = new Vector2Int(x, y);
                    return true;
                }
            }
        }

        position = Vector2Int.zero;
        return false;
    }

    public void Remove(GoBagItem item)
    {
        if (item == null) return;
        if (!itemPositions.ContainsKey(item)) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (cells[x, y] == item) cells[x, y] = null;
            }
        }

        itemPositions.Remove(item);
    }

    public bool Contains(GoBagItem item)
    {
        if (item == null) return false;
        return itemPositions.ContainsKey(item);
    }

    public static bool CanFitAll(int width, int height, List<GoBagItem> items, out GoBagItem firstMisfit)
    {
        GoBagGrid testGrid = new GoBagGrid(width, height);

        foreach (GoBagItem source in SortBiggestFirst(items))
        {
            GoBagItem copy = new GoBagItem(source.Choice);

            if (testGrid.TryPlaceAnywhere(copy, out _)) continue;

            copy.Rotate();
            if (testGrid.TryPlaceAnywhere(copy, out _)) continue;

            firstMisfit = source;
            return false;
        }

        firstMisfit = null;
        return true;
    }

    private static List<GoBagItem> SortBiggestFirst(List<GoBagItem> source)
    {
        List<GoBagItem> sorted = new List<GoBagItem>(source);

        for (int i = 1; i < sorted.Count; i++)
        {
            GoBagItem current = sorted[i];
            int j = i - 1;
            while (j >= 0 && GetArea(sorted[j]) < GetArea(current))
            {
                sorted[j + 1] = sorted[j];
                j--;
            }
            sorted[j + 1] = current;
        }
        return sorted;
    }

    private static int GetArea(GoBagItem item)
    {
        return item.Width * item.Height;
    }
}
