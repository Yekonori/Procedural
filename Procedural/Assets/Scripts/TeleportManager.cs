using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    private List<List<int>> teleportPaths = new List<List<int>>()
    {
        new List<int> () {1, 2, 3, 4 },
        new List<int> () {2, 3, 4, 1 },
        new List<int> () {3, 4, 1, 2 },
        new List<int> () {4, 1, 2, 3 },
    };

    private Queue<int> currentPath = new Queue<int>();

    public static TeleportManager Get;

    private void Awake()
    {
        if (Get == null)
        {
            Get = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public bool UpdateQueue(int teleportNumber)
    {
        if (currentPath.Count == 4)
        {
            currentPath.Dequeue();
        }

        currentPath.Enqueue(teleportNumber);

        return CheckPerfectPath();
    }

    private bool CheckPerfectPath()
    {
        List<int> list = currentPath.ToList();

        if (list == teleportPaths[0] || list == teleportPaths[1] || list == teleportPaths[2] || list == teleportPaths[3])
        {
            return true;
        }

        return false;
    }
}
