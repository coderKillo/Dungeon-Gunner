using UnityEngine;

public class GridNodes
{
    private int width = 0;
    public int Width { get { return width; } }
    private int height = 0;
    public int Height { get { return height; } }

    private Node[,] grid;

    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;

        grid = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Node(new Vector2Int(x, y));
            }

        }
    }

    public Node GetNode(int x, int y)
    {
        if (x >= width || y >= height)
        {
            Debug.Log("Node out of range, x:" + x + " y:" + y);
            return null;
        }

        return grid[x, y];
    }
}
