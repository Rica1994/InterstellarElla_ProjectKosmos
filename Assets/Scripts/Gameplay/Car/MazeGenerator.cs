using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject cubePrefab;
    public Vector2Int mazeDimensions = new Vector2Int(21, 21);
    private bool[,] visited;

    public void GenerateMaze()
    {
        visited = new bool[mazeDimensions.x, mazeDimensions.y];  // Reset the visited array

        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        Vector2Int current = new Vector2Int(Random.Range(1, mazeDimensions.x - 1), Random.Range(1, mazeDimensions.y - 1));
        current.x = (current.x % 2 == 0) ? current.x + 1 : current.x;
        current.y = (current.y % 2 == 0) ? current.y + 1 : current.y;

        visited[current.x, current.y] = true;
        stack.Push(current);

        List<Vector2Int> directions = new List<Vector2Int>
    {
        new Vector2Int(0, 2),
        new Vector2Int(0, -2),
        new Vector2Int(2, 0),
        new Vector2Int(-2, 0)
    };

        while (stack.Count > 0)
        {
            Vector2Int cell = stack.Peek();
            List<Vector2Int> unvisitedNeighbors = new List<Vector2Int>();

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = cell + dir;
                if (IsInside(next) && !visited[next.x, next.y])
                {
                    unvisitedNeighbors.Add(dir);
                }
            }

            if (unvisitedNeighbors.Count > 0)
            {
                Vector2Int chosenDirection = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];
                Vector2Int wallPos = cell + chosenDirection / 2;
                visited[wallPos.x, wallPos.y] = true;

                Vector2Int chosenCell = cell + chosenDirection;
                visited[chosenCell.x, chosenCell.y] = true;
                stack.Push(chosenCell);
            }
            else
            {
                stack.Pop();
            }
        }

        // Delete old cubes if they exist
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        for (int x = 0; x < mazeDimensions.x; x++)
        {
            for (int y = 0; y < mazeDimensions.y; y++)
            {
                if (x % 2 == 0 || y % 2 == 0)
                {
                    if (!visited[x, y])
                    {
                        GameObject cube = Instantiate(cubePrefab, transform.position + new Vector3(x, 0.5f, y), Quaternion.identity, transform);
                        cube.transform.SetParent(this.transform);
                    }
                }
            }
        }

        // Create an entrance and exit
        Vector3 entrancePos = transform.position + new Vector3(0, 0.5f, mazeDimensions.y - 2);
        Vector3 exitPos = transform.position + new Vector3(mazeDimensions.x - 1, 0.5f, 1);
        DestroyWallAtPosition(entrancePos);
        DestroyWallAtPosition(exitPos);
    }
    private void DestroyWallAtPosition(Vector3 position)
    {
        Collider[] hitColliders = Physics.OverlapBox(position, new Vector3(0.1f, 0.1f, 0.1f));
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("MazeWall")) // assuming your prefab has a tag named "MazeWall"
            {
                DestroyImmediate(hitCollider.gameObject);
            }
        }
    }

    bool IsInside(Vector2Int cell)
    {
        return cell.x > 0 && cell.y > 0 && cell.x < mazeDimensions.x - 1 && cell.y < mazeDimensions.y - 1;
    }
}