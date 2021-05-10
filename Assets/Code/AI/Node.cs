using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // Attributes
    private int number;
    private Vector2 pos;
    private List<int> connectedNodes = new List<int>();
    private List<int> pathway = new List<int>();
    private int wallMask = LayerMask.GetMask("Wall");
    private float movementCost = 0.0f;
    private float heuristic = 0.0f;

    public Node(Vector2 pos, int number)
    {
        this.pos = pos;
        this.number = number;
    }

    public Node(Node node)
    {
        pos = node.GetPos();
        number = node.GetNumber();
        connectedNodes = node.GetConnectedNodes();
        pathway = node.GetPathway();
        wallMask = LayerMask.GetMask("Wall");
        movementCost = node.GetMovementCost();
        heuristic = node.GetHeuristic();
    }

    public void FindConnectedNodes(List<Node> nodes)
    {
        connectedNodes = new List<int>();
        wallMask = LayerMask.GetMask("Wall");

        foreach (var node in nodes)
        {
            Vector2 otherPos = node.GetPos();
            if(otherPos != pos)
            {
                if (!Physics2D.Linecast(pos, otherPos, wallMask))
                {
                    float distance = Mathf.Sqrt(Mathf.Pow(pos.x - otherPos.x, 2.0f) + Mathf.Pow(pos.y - otherPos.y, 2.0f));
                    if (distance < 20.0f)
                        connectedNodes.Add(node.GetNumber());
                }
            }
        }

        //string connectedNodesStr = "Node " + number + " at position " + pos.x + ", " + pos.y + " is connected to: ";
        //foreach(var num in connectedNodes)
        //{
        //    connectedNodesStr += num.ToString() + " at position: " + nodes[num].pos.x + ", " + nodes[num].pos.y + "\n";
        //}
        //Debug.Log(connectedNodesStr);
    }

    // Getters and Setters
    public List<int> GetConnectedNodes()
    {
        return connectedNodes;
    }

    public List<int> GetPathway()
    {
        return pathway;
    }

    public void SetPathway(List<int> pathway)
    {
        pathway.Add(number);
        this.pathway = pathway;
    }

    public Vector2 GetPos()
    {
        return pos;
    }

    public int GetNumber()
    {
        return number;
    }

    public float GetMovementCost()
    {
        return movementCost;
    }

    public void SetMovementCost(float movementCost)
    {
        this.movementCost = movementCost;
    }

    public float GetHeuristic()
    {
        return heuristic;
    }

    public void SetHeuristic(float heuristic)
    {
        this.heuristic = heuristic;
    }

    public float GetForwardCost()
    {
        return movementCost + heuristic;
    }
}
