using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    Node[,] nodes;
    int width, height;

    // Van Ravi Bechoe
    public Dictionary<int, Wall> WallMapping = new Dictionary<int, Wall>()
    {
        { 1, Wall.LEFT },
        { -2, Wall.UP },
        { -1, Wall.RIGHT },
        { 2, Wall.DOWN }
    };

    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid, int _width, int _height, int scaleFactor)
    {
        // calculate grid position
        startPos = new Vector2Int((int)Mathf.Round(startPos.x * 1f / scaleFactor), (int)Mathf.Round(startPos.y * 1f / scaleFactor));
        endPos = new Vector2Int((int)Mathf.Round(endPos.x * 1f / scaleFactor), (int)Mathf.Round(endPos.y * 1f / scaleFactor));

        // invalid point, no need to continue
        if ((endPos.x < 0 || endPos.y < 0 || endPos.x > _width || endPos.y > _height) ||
            (startPos.x < 0 || startPos.y < 0 || startPos.x > _width || startPos.y > _height))
        {
            Debug.Log("Invalid end point");
            return null;
        }

        width = _width;
        height = _height;
        nodes = new Node[width, height];
        nodes.Initialize();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nodes[x, y] = new Node(grid[x, y].gridPosition, null);
            }
        }

        Node startNode = nodes[startPos.x, startPos.y];
        Node endNode = nodes[endPos.x, endPos.y];

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode); // starting point for the pathfinding

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                // if other node(s) have better scores start looking from their point
                if (openSet[i].FScore < currentNode.FScore || openSet[i].FScore == currentNode.FScore && openSet[i].HScore < currentNode.HScore)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Generate path to follow by reversing it
            if (currentNode == endNode)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                List<Node> nodePath = RetracePath(startNode, endNode);
                for (int i = 0; i < nodePath.Count; i++)
                {
                    path.Add(nodePath[i].position * scaleFactor);
                }
                return path;
            }

            // Get all neighbours of the node
            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                bool wallBlock = CrossPatternWallCheck(currentNode, neighbour, grid);

                // skip this iteration if node is unreachable or if we already checked it
                if (closedSet.Contains(neighbour) || wallBlock)
                {
                    continue;
                }

                int costToNeighbour = currentNode.GScore + GetDistance(currentNode, neighbour);
                if (costToNeighbour < neighbour.GScore || !openSet.Contains(neighbour))
                {
                    neighbour.GScore = costToNeighbour;
                    neighbour.HScore = GetDistance(neighbour, endNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        Debug.Log("Couldnt find a path!");
        return null;
    }

    // Create reversed path for the agent to navigate
    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != null)
        {
            if (!path.Contains(currentNode))
            {
                path.Add(currentNode);
                if (currentNode == startNode)
                {
                    break;
                }
                currentNode = currentNode.parent;
            }
            else
            {
                Debug.Log("Failed to calculate whole path, triggered infinite loop");
                break;
            }
        }
        path.Reverse();
        return path;
    }

    // Van Ravi Bechoe
    bool CrossPatternWallCheck(Node currentNode, Node neighbour, Cell[,] grid)
    {
        Cell cell = grid[currentNode.position.x, currentNode.position.y];
        int key = (currentNode.position.x - neighbour.position.x) + ((currentNode.position.y - neighbour.position.y) * 2);
        return (key != 0 && cell.HasWall(WallMapping[key])) ? true : false;
    }

    // Calculate distance between A and B
    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.position.x - nodeB.position.x);
        int distY = Mathf.Abs(nodeA.position.y - nodeB.position.y);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }

    // Get all possible neighbours of a node based on the location on the grid
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int neighbourX = node.position.x + x;
                int neighbourY = node.position.y + y;

                if ((x + y) % 2 == 0)
                {
                    continue;
                }

                if (neighbourX < width && neighbourY < height && neighbourX >= 0 && neighbourY >= 0)
                {
                    Node neighbour = nodes[neighbourX, neighbourY];
                    neighbours.Add(neighbour);
                }
            }
        }

        return neighbours;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore
        { //GScore + HScore
            get { return GScore + HScore; }
        }
        public int GScore; //Current Travelled Distance
        public int HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent)
        {
            this.position = position;
            this.parent = parent;
        }
    }
}
