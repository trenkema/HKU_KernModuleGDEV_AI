using System.Collections.Generic;
using UnityEngine;
public class Agent : MonoBehaviour
{
    [SerializeField] GameObject scaleableAgent;
    [SerializeField] Color targetVisualColor;
    [SerializeField] GameObject mazeGameObject;

    public int moveButton = 0;
    public float moveSpeed = 3;
    private Astar Astar = new Astar();
    private List<Vector2Int> path = new List<Vector2Int>();
    private Plane ground = new Plane(Vector3.up, 0f);
    private MeshRenderer agentRenderer;
    private GameObject targetVisual;
    private MazeGeneration maze;
    private LineRenderer line;
    private Vector3 spawnPos;
    private int pathIndex = 0;

    private void Awake()
    {
        maze = FindObjectOfType<MazeGeneration>();
        maze.mazeGenerated += MazeGenerated;

        agentRenderer = GetComponentInChildren<MeshRenderer>();

        spawnPos = transform.position;

        scaleableAgent.transform.localScale *= maze.scaleFactor;

        targetVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        targetVisual.transform.position = spawnPos;
        targetVisual.transform.localScale = scaleableAgent.transform.localScale;
        targetVisual.GetComponent<MeshRenderer>().material.color = targetVisualColor;

        line = GetComponent<LineRenderer>();
        line.material.color = agentRenderer.material.color;
        line.material.color = agentRenderer.material.color;
    }

    // Move the maze on the Y axis, to make the agents not intersect with the maze
    public void MazeGenerated()
    {
        float mazePosY = mazeGameObject.transform.position.y - (scaleableAgent.transform.localScale.y / 4);
        mazeGameObject.transform.position = new Vector3(mazeGameObject.transform.position.x, mazePosY, mazeGameObject.transform.position.z);
    }

    public void FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        path = null;
        path = Astar.FindPathToTarget(startPos, endPos, grid, maze.width, maze.height, maze.scaleFactor);

        if (path == null)
        {
            transform.position = spawnPos;
            targetVisual.transform.position = spawnPos;
        }

        DrawPath();
    }

    private void DrawPath()
    {
        if (path != null && path.Count > 0)
        {
            line.positionCount = path.Count;
            line.SetPosition(0, transform.position);

            for (int i = 0; i < path.Count; i++)
            {
                line.SetPosition(i, Vector2IntToVector3(path[i], 0.1f));
            }
        }
    }


    //Move to clicked position
    public void Update()
    {
        if (Input.GetMouseButtonDown(moveButton))
        {
            Vector3 mousePos = MouseToWorld();
            Vector2Int targetPos = Vector3ToVector2Int(mousePos);
            targetPos = new Vector2Int((int)Mathf.Round(targetPos.x * 1f / maze.scaleFactor), (int)Mathf.Round(targetPos.y * 1f / maze.scaleFactor));
            targetPos *= maze.scaleFactor;

            targetVisual.transform.position = new Vector3(targetPos.x, transform.position.y, targetPos.y);
            FindPathToTarget(Vector3ToVector2Int(transform.position), targetPos, maze.grid);
            pathIndex = 0;
        }

        if (transform.position.x < 0 || transform.position.x > maze.width * maze.scaleFactor ||
            transform.position.y < 0 || transform.position.y > maze.height * maze.scaleFactor)
        {
            transform.position = spawnPos;
            path = null;
        }

        if (path != null && path.Count > 0)
        {
            if (transform.position != Vector2IntToVector3(path[0]))
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector2IntToVector3(path[0]) - transform.position), 360f * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, Vector2IntToVector3(path[0]), moveSpeed * Time.deltaTime);
            }
            else
            {
                path.RemoveAt(0);
                DrawPath();
            }
        }
    }
    public Vector3 MouseToWorld()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float distToGround = -1f;
        ground.Raycast(ray, out distToGround);
        Vector3 worldPos = ray.GetPoint(distToGround);

        return worldPos;
    }

    private Vector2Int Vector3ToVector2Int(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }
    private Vector3 Vector2IntToVector3(Vector2Int pos, float YPos = 0)
    {
        return new Vector3(Mathf.RoundToInt(pos.x), YPos, Mathf.RoundToInt(pos.y));
    }
    private void OnDrawGizmos()
    {
        if (path != null && path.Count > 0)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.color = agentRenderer.material.color;
                Gizmos.DrawLine(Vector2IntToVector3(path[i], 0.5f), Vector2IntToVector3(path[i + 1], 0.5f));
            }

        }
    }
}
