using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;


public class PathLine : MonoBehaviour {

    PathRequestMaganer2 requestManager;
    Grid grid;
    bool succes;

    void Awake()
    {
        requestManager = GetComponent<PathRequestMaganer2>();
        grid = GetComponent<Grid>();
    }

    public void StartFindline(Vector3 startPos, Vector3 targetPos/*,Action<Vector3[], bool> snap*/)
    {

        StartCoroutine(FindLine(startPos, targetPos/*, snap*/));
    }


    IEnumerator FindLine(Vector3 startPos, Vector3 targetPos/*,Action<Vector3[], bool> snap*/)
    {
         Vector3[] waypoints = new Vector3[0];
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                break;               
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if ( closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        yield return null;
        waypoints = RetracePath(startNode,targetNode);
        //snap(waypoints, succes);

        requestManager.FinishedProcessingPath(waypoints, succes);
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        succes = true;
        while (currentNode != startNode)
        {
            if (!currentNode.walkable) succes = false;
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);

        path.Reverse();
        //grid.path = path;

        return waypoints;

    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 0; i < path.Count; i++)
        {
            waypoints.Add(path[i].worldPosition);
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
