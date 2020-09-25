using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;


using GOList = System.Collections.Generic.List<UnityEngine.GameObject>;

public class GridAI
{

    // public things
    public static GridAI GetInstance()
    {
        if(instance == null)
        {
            instance = new GridAI();
        }
        return instance;
    }

    public GOList GetClosePositions(Vector2 pos)
    {
        int x = ((int)pos.x - minX) / cellSize;
        int y = ((int)pos.y - minY) / cellSize;

        GOList result = new GOList();

        for (int i = -1; i < 2; ++i)
        {
            for (int j = -1; j < 2; ++j)
            {
                int ix = i + x;
                int jy = j + y;
                if (ix >= 0 && ix < numX &&
                    jy >= 0 && jy < numY)
                {
                    result.AddRange(grid[ix, jy]);
                }
            }
        }

        return result;
    }
    
    public GOList GetClosePositions(Vector2 pos, float distance)
    {
        int cells = (int) distance / cellSize;


        int x = ((int)pos.x - minX) / cellSize;
        int y = ((int)pos.y - minY) / cellSize;

        GOList result = new GOList();
        for (int i = -cells / 2; i < cells / 2; ++i)
        {
            for (int j = -cells / 2; j < cells / 2; ++j)
            {
                int ix = i + x;
                int jy = j + y;
                if (ix >= 0 && ix < numX &&
                    jy >= 0 && jy < numY)
                {
                    result.AddRange(grid[ix, jy]);
                }
            }
        }
        return result;
    }

    public void InitializePosition(GameObject go, Vector2 pos)
    {
        int newX = ((int)pos.x - minX) / cellSize;
        int newY = ((int)pos.y - minY) / cellSize;

        grid[newX, newY].Add(go);

        check.Add(go, new Vector2(newX, newY));
    }

    public void UpdatePosition(GameObject go, Vector2 oldPos, Vector2 newPos)
    {
        int oldX = ((int)oldPos.x - minX) / cellSize;
        int oldY = ((int)oldPos.y - minY) / cellSize;

        int newX = ((int)newPos.x - minX) / cellSize;
        int newY = ((int)newPos.y - minY) / cellSize;

        if (oldX != newX || oldY != newY)
        {
            Vector2 pi = check[go];
            grid[(int)pi.x, (int)pi.y].Remove(go);
            grid[newX, newY].Add(go);
            check[go] = new Vector2(newX, newY);
        }
    }


    public void RemoveFromGrid(GameObject go)
    {
        Vector2 pi = check[go];

        grid[(int)pi.x, (int)pi.y].Remove(go);
        check.Remove(go);

    }

    public int GetNumAIs()
    {
        int num = 0;
        foreach(GOList gol in grid)
        {
            num += gol.Count;
        }
        return num;
    }

    // private things
    private static GridAI instance = null;

    private const int minX = -50, maxX = 50, minY = -50, maxY = 50;
    private const int cellSize = 1;

    private const int numX = (maxX - minX) / cellSize + 1;
    private const int numY = (maxY - minY) / cellSize + 1;

    private GOList[,] grid = new GOList[numX,numY];

    private Dictionary<GameObject, Vector2> check = new Dictionary<GameObject, Vector2>();
    private GridAI()
    {
        for (int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                grid[i, j] = new GOList();
            }
        }
    }

    public void ShowGrid()
    {
        Vector2 start = new Vector2(minX * cellSize, maxY * cellSize);
        for(int i = 0; i <= maxX * 2.0f; ++i)
        {
            Debug.DrawLine(start + new Vector2(0.0f, -cellSize * i) , 
                start + new Vector2(0.0f, -cellSize * i) + new Vector2(1.0f, 0.0f) * maxX * cellSize * 2.0f, Color.green);

            Debug.DrawLine(start + new Vector2(cellSize * i, 0.0f), 
                start + new Vector2(cellSize * i, 0.0f) + new Vector2(0.0f, -1.0f) * maxY * cellSize* 2.0f, Color.green);
        }
    }
}
