using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] MazeNode nodePrefab;
    [SerializeField] GameObject floorGameObjectPrefab;
    [SerializeField] GameObject SpawnPointPrefab;
    [SerializeField] GameObject targetPrefab;

    [SerializeField] Vector2Int mazeSize;
    [SerializeField] int mazeScale;

    GameObject spawnPointInst;
    GameObject targetInst;
    GameObject floorInst;
    MazeAcademy mazeAcademy;

    private void Start() //W tym miejscu trzeba zamienic zeby zamiast podczas startu to generowalo sie podczas wcisniecia przycisku 
    {
        mazeAcademy = GetComponent<MazeAcademy>();
        GenerateNewMaze();
    }

    void setupAcademy()
    {
        mazeAcademy.SetFloorGameObject(floorInst);
        mazeAcademy.setStartPosition(spawnPointInst.transform);
        mazeAcademy.SetTargetTransform(targetInst.transform);
    }

    void GenerateMazeInstant(Vector2Int size)
    {

        DestroyOldMaze();

        List<MazeNode> nodes = new List<MazeNode>();
        floorInst = Instantiate(floorGameObjectPrefab, new Vector3(-0.5f, -0.5f, -0.5f) * mazeScale, Quaternion.identity, transform);
        floorInst.transform.localScale = new Vector3(size.x, 0.1f, size.y) * mazeScale;


        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3 nodePos = new Vector3(x - ((size.x) / 2f), 0, y - ((size.y) / 2f)) * mazeScale;
                MazeNode newNode = Instantiate(nodePrefab, nodePos, Quaternion.identity, transform);
                newNode.transform.localScale = newNode.transform.localScale * mazeScale;
                nodes.Add(newNode);
            }
        }

        List<MazeNode> currentPath = new List<MazeNode>();
        List<MazeNode> completedNodes = new List<MazeNode>();

        // Wybor poczatkowego node`a
        MazeNode StartNode = nodes[Random.Range(0, nodes.Count)];
        // Wybor koncowego node`a
        MazeNode EndNode;

        do
        {
            EndNode = nodes[Random.Range(0, nodes.Count)];
        }
        while (EndNode == StartNode);

        //Spawn punktow do respu i celu
        spawnPointInst = Instantiate(SpawnPointPrefab, StartNode.transform.position, Quaternion.identity, transform);
        targetInst = Instantiate(targetPrefab, EndNode.transform.position, Quaternion.identity, transform);


        currentPath.Add(StartNode);

        while (completedNodes.Count < nodes.Count)
        {
            // Sprawdzanie nastepnego node`a
            List<int> possibleNextNodes = new List<int>();
            List<int> possibleDirections = new List<int>();

            int currentNodeIndex = nodes.IndexOf(currentPath[currentPath.Count - 1]);
            int currentNodeX = currentNodeIndex / size.y;
            int currentNodeY = currentNodeIndex % size.y;

            if (currentNodeX < size.x - 1)
            {
                // Sprawdzanie node`a na prawo
                if (!completedNodes.Contains(nodes[currentNodeIndex + size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }
            if (currentNodeX > 0)
            {
                // Sprawdzanie node`a na lewo
                if (!completedNodes.Contains(nodes[currentNodeIndex - size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(currentNodeIndex - size.y);
                }
            }
            if (currentNodeY < size.y - 1)
            {
                // Sprawdzanie node`a powyzej
                if (!completedNodes.Contains(nodes[currentNodeIndex + 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }
            if (currentNodeY > 0)
            {
                // Sprawdzanie node`a ponizej
                if (!completedNodes.Contains(nodes[currentNodeIndex - 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - 1]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(currentNodeIndex - 1);
                }
            }

            // Wybor nastepnego node`a
            if (possibleDirections.Count > 0)
            {
                int chosenDirection = Random.Range(0, possibleDirections.Count);
                MazeNode chosenNode = nodes[possibleNextNodes[chosenDirection]];

                switch (possibleDirections[chosenDirection])
                {
                    case 1:
                        chosenNode.RemoveWall(1);
                        currentPath[currentPath.Count - 1].RemoveWall(0);
                        break;
                    case 2:
                        chosenNode.RemoveWall(0);
                        currentPath[currentPath.Count - 1].RemoveWall(1);
                        break;
                    case 3:
                        chosenNode.RemoveWall(3);
                        currentPath[currentPath.Count - 1].RemoveWall(2);
                        break;
                    case 4:
                        chosenNode.RemoveWall(2);
                        currentPath[currentPath.Count - 1].RemoveWall(3);
                        break;
                }

                currentPath.Add(chosenNode);
            }
            else
            {
                completedNodes.Add(currentPath[currentPath.Count - 1]);

                currentPath.RemoveAt(currentPath.Count - 1);
            }
        }
        
    }

    public void GenerateNewMaze()
    {
        GenerateMazeInstant(mazeSize);
        setupAcademy();
    }

    void DestroyOldMaze()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
