using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    // Attributes
    private List<Node> nodes;
    private GameObject player;
    private int playerNode;
    private bool active;
    private int wallMask;

    // Start is called before the first frame update
    void Start()
    {
        nodes = new List<Node>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerNode = -1;
        active = false;
        wallMask = LayerMask.GetMask("Wall");
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            FindPlayer();
        }
    }

    public void SetUpNodes()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Node");
        int counter = 0;

        foreach(var node in objects)
        {
            Vector3 pos = node.transform.position;
            nodes.Add(new Node(new Vector2(pos.x, pos.y), counter++));
        }

        foreach(var node in nodes)
        {
            node.FindConnectedNodes(nodes);
        }

        active = true;
    }

    private void FindPlayer()
    {
        List<Node> closeNodes = new List<Node>();
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);

        foreach(var node in nodes)
        {
            Vector2 nodePos = node.GetPos();
            if (!Physics2D.Linecast(playerPos, nodePos, wallMask))
                closeNodes.Add(node);
        }

        if(closeNodes.Count > 0)
        {
            float playerX = playerPos.x;
            float playerY = playerPos.y;
            Vector2 nodePos = closeNodes[0].GetPos();
            float closestDistance = Mathf.Sqrt(Mathf.Pow(playerX - nodePos.x, 2.0f) + Mathf.Pow(playerY - nodePos.y, 2.0f));
            playerNode = closeNodes[0].GetNumber();

            for (int i = 1; i < closeNodes.Count; ++i)
            {
                nodePos = closeNodes[i].GetPos();
                float nodeDist = Mathf.Sqrt(Mathf.Pow(playerX - nodePos.x, 2.0f) + Mathf.Pow(playerY - nodePos.y, 2.0f));

                if (nodeDist < closestDistance)
                {
                    closestDistance = nodeDist;
                    playerNode = closeNodes[i].GetNumber();
                }
            }
        }
        else
        {
            playerNode = -1;
        }
    }

    // Getters and Setters
    public int GetPlayerNode()
    {
        return playerNode;
    }

    public List<Node> GetNodes()
    {
        return nodes;
    }

    public bool GetActive()
    {
        return active;
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }
}
