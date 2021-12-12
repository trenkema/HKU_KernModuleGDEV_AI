using System.Collections.Generic;
using UnityEngine;

public class MazeGeneration : MonoBehaviour
{
    public System.Action mazeGenerated;
    public int width = 10, height = 10;
    public Cell[,] grid;
    public int scaleFactor = 1;
    public CellPrefab cellPrefab;
    public float desiredWallpercentage = 0.4f;
    private List<GameObject> allCellObjects = new List<GameObject>();
    public int seed = 1234;

    // Start is called before the first frame update
    private void Awake()
    {
        Random.InitState(seed);
        GenerateMaze();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            seed = Random.Range(0, int.MaxValue);
            Random.InitState(seed);
            width = Random.Range(10, 100);
            height = Random.Range(10, 100);
            desiredWallpercentage = Random.Range(0.2f, 1.0f);
            DestroyMazeObjects();
            GenerateMaze();
        }
    }

    private void DestroyMazeObjects()
    {
        allCellObjects.Clear();
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
    }

    public void GenerateMaze()
    {
        grid = new Cell[width, height];
        grid.Initialize();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Cell();
                grid[x, y].gridPosition = new Vector2Int(x, y);
                grid[x, y].walls = Wall.DOWN | Wall.LEFT | Wall.RIGHT | Wall.UP;
            }
        }

        Stack<Cell> cellStack = new Stack<Cell>();
        List<Cell> visitedCells = new List<Cell>();
        cellStack.Push(grid[0, 0]);
        Cell currentCell;
        while (cellStack.Count > 0)
        {
            currentCell = cellStack.Pop();
            List<Cell> neighbours = GetUnvisitedNeighbours(currentCell, visitedCells, cellStack);
            if (neighbours.Count > 1)
            {
                cellStack.Push(currentCell);
            }

            if (neighbours.Count != 0)
            {
                Cell randomUnvisitedNeighbour = neighbours[Random.Range(0, neighbours.Count)];
                RemoveWallBetweenCells(currentCell, randomUnvisitedNeighbour);
                visitedCells.Add(randomUnvisitedNeighbour);
                cellStack.Push(randomUnvisitedNeighbour);
            }
        }

        //Remove a couple random walls to make the maze more 'open'
        int totalWallsInMaze = GetWallCount(grid);
        int totalPossibleWallsInmaze = 4 * width * height;
        float wallPercentage = totalWallsInMaze / (float)totalPossibleWallsInmaze;
        Debug.Log("Wall Percentage: " + wallPercentage);
        while (wallPercentage > desiredWallpercentage)
        {
            int randomX = Random.Range(0, width);
            int randomY = Random.Range(0, height);
            Cell randomCell = grid[randomX, randomY];
            List<Cell> neighbours = GetNeighbours(randomCell);
            if (neighbours.Count > 0)
            {
                Cell randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];
                bool wallsRemoved = RemoveWallBetweenCells(randomCell, randomNeighbour);
                if (wallsRemoved)
                {
                    totalWallsInMaze -= 2;
                    wallPercentage = totalWallsInMaze / (float)totalPossibleWallsInmaze;
                }
            }
        }

        //Generate Objects
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CellPrefab cellObject = Instantiate(cellPrefab, new Vector3(x * scaleFactor, 0, y * scaleFactor), Quaternion.identity, transform);
                cellObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                cellObject.SpawnWalls(grid[x, y]);
                allCellObjects.Add(cellObject.gameObject);
            }
        }

        mazeGenerated?.Invoke();
    }
    private int GetWallCount(Cell[,] grid)
    {
        int walls = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                walls += cell.GetNumWalls();
            }
        }
        return walls;
    }

    private List<Cell> GetNeighbours(Cell cell)
    {
        List<Cell> result = new List<Cell>();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                int cellX = cell.gridPosition.x + x;
                int cellY = cell.gridPosition.y + y;
                if (cellX < 0 || cellX >= width || cellY < 0 || cellY >= height || Mathf.Abs(x) == Mathf.Abs(y))
                {
                    continue;
                }
                Cell canditateCell = grid[cellX, cellY];
                result.Add(canditateCell);
            }
        }
        return result;
    }

    /// <summary>
    /// Gets the unvisited neighbours for a cell
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="visitedCells"></param>
    /// <param name="cellstack"></param>
    /// <returns></returns>
    private List<Cell> GetUnvisitedNeighbours(Cell cell, List<Cell> visitedCells, Stack<Cell> cellstack)
    {
        List<Cell> result = new List<Cell>();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                int cellX = cell.gridPosition.x + x;
                int cellY = cell.gridPosition.y + y;
                if (cellX < 0 || cellX >= width || cellY < 0 || cellY >= height || Mathf.Abs(x) == Mathf.Abs(y))
                {
                    continue;
                }
                Cell canditateCell = grid[cellX, cellY];
                if (!visitedCells.Contains(canditateCell) && !cellstack.Contains(canditateCell))
                {
                    result.Add(canditateCell);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// This function assumes the two inputcells are next to each other
    /// </summary>
    /// <param name="cellOne"></param>
    /// <param name="cellTwo"></param>
    private bool RemoveWallBetweenCells(Cell cellOne, Cell cellTwo)
    {
        int numWallCellOne = cellOne.GetNumWalls();
        Vector2Int dirVector = cellTwo.gridPosition - cellOne.gridPosition;
        if (dirVector.x != 0)
        {
            cellOne.RemoveWall(dirVector.x > 0 ? Wall.RIGHT : Wall.LEFT);
            cellTwo.RemoveWall(dirVector.x > 0 ? Wall.LEFT : Wall.RIGHT);
        }
        if (dirVector.y != 0)
        {
            cellOne.RemoveWall(dirVector.y > 0 ? Wall.UP : Wall.DOWN);
            cellTwo.RemoveWall(dirVector.y > 0 ? Wall.DOWN : Wall.UP);
        }

        //Is a wall succesfully removed?
        if (numWallCellOne != cellOne.GetNumWalls()) { return true; }
        return false;
    }

    public Cell GetCellForWorldPosition(Vector3 worldPos)
    {
        return grid[(int)(Mathf.RoundToInt(worldPos.x) / scaleFactor), (int)(Mathf.RoundToInt(worldPos.z) / scaleFactor)];
    }
}

[System.Serializable]
public class Cell
{
    public Vector2Int gridPosition;
    public Wall walls; //bit Encoded
    public void RemoveWall(Wall wallToRemove)
    {
        walls = (walls & ~wallToRemove);
    }

    public int GetNumWalls()
    {
        int numWalls = 0;
        if (((walls & Wall.DOWN) != 0)) { numWalls++; }
        if (((walls & Wall.UP) != 0)) { numWalls++; }
        if (((walls & Wall.LEFT) != 0)) { numWalls++; }
        if (((walls & Wall.RIGHT) != 0)) { numWalls++; }
        return numWalls;
    }

    public bool HasWall(Wall wall)
    {
        return (walls & wall) != 0;
    }
}

[System.Flags]
public enum Wall
{
    LEFT = 0x1,
    UP = 0x2,
    RIGHT = 0x4,
    DOWN = 0x8
}