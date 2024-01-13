using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    Available,
    Current,
    Completed
}

public class MazeNode : MonoBehaviour
{
    [SerializeField] GameObject[] walls;

    public void RemoveWall(int wallToRemove)
    {
        Destroy(walls[wallToRemove].gameObject);
    }

}
