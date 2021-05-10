using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAI : BasicFly
{
    // Attributes
    NodeManager nm;
    List<Node> nodes;
    Player pScript;
    List<Vector2> pathway;
    int playerNode = -2;
    bool setup;
    bool pathCalculated;
    bool DebugMode = true;
    bool waitOneFrame = false;

    protected override void Start()
    {
        nm = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<NodeManager>();
        pScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        pathway = new List<Vector2>();
        setup = false;
        pathCalculated = false;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (waitOneFrame)
        {
            nodes = nm.GetNodes();
            playerNode = nm.GetPlayerNode();
            setup = true;
            waitOneFrame = false;
        }
        if (setup)
        {
            ChasePlayer();
        }
        else if(nm.GetActive())
        {
            pScript.SetCanMove(false);
            waitOneFrame = true;
        }
    }

    private void ChasePlayer()
    {
        if (PlayerClose())
        {
            pathCalculated = false;
            flySpeed = 8.0f;
            AttackPlayer();
        }
        else if (playerNode != -1)
        {
            flySpeed = 6.0f;
            TrackPlayer();
        }
    }

    private void TrackPlayer()
    {
        CheckPlayerPos();

        if (playerNode != -1)
        {
            if (!pathCalculated)
            {
                AStarSearch();
                pathCalculated = true;
                pScript.SetCanMove(true);
            }

            FollowPath();
        }
    }

    private void CheckPlayerPos()
    {
        int newPlayerNode = nm.GetPlayerNode();

        if (playerNode != newPlayerNode)
        {
            pathCalculated = false;
            playerNode = newPlayerNode;
        }
    }

    private void AStarSearch()
    {
        Node goalNode = null;

        // Setup Lists
        // Open List is a list of nodes that need to be examined
        List<Node> openList = new List<Node>();
        // Closes list keeps track of nodes that have been examined
        List<Node> closedList = new List<Node>();

        // Calulate values for current node
        Node currentNode = new Node(FindCurrentNode());
        currentNode.SetMovementCost(0.0f);
        currentNode.SetHeuristic(CalculateDistance(currentNode, nodes[playerNode]));
        // Add current node to open list to be examined
        openList.Add(new Node(currentNode));
        openList[0].SetPathway(new List<int>());

        bool foundGoal = false;
        // Search the open list until goal is found
        
        while(!foundGoal && openList.Count > 0)
        {
            // Find the lowest cost node and remove it from the list
            Node q = FindLowestCostNode(openList);
            openList.Remove(q);

            // Generate the successor nodes of q and add them to the pathway
            List<Node> successors = new List<Node>();
            foreach(var num in q.GetConnectedNodes())
            {
                successors.Add(new Node(nodes[num]));
                successors[successors.Count - 1].SetPathway(new List<int>(q.GetPathway()));
            }

            // Check each node connected to q
            foreach(var node in successors)
            {
                if (!foundGoal)
                {
                    // If the goal is found, stop searching
                    if (nodes[playerNode].GetNumber() == node.GetNumber())
                    {
                        goalNode = new Node(node);
                        foundGoal = true;
                    }
                    else
                    {
                        // Set the movement cost to q's cost + the distance between q and successor
                        node.SetMovementCost(q.GetMovementCost() + CalculateDistance(q, node));
                        // Set the heuristic to the euclidean distance between the successor and the goal
                        node.SetHeuristic(CalculateDistance(node, nodes[playerNode]));
                        // Forward cost is calculated in the node class

                        bool skip = false;
                        // Skip this successor if the node already exists in the open list, but with a lower forward cost
                        foreach(var openNode in openList)
                        {
                            if (openNode.GetNumber() == node.GetNumber() && openNode.GetForwardCost() < node.GetForwardCost())
                            {
                                skip = true;
                            }
                        }

                        if (!skip)
                        {
                            // Skip this successor if the node already exists in the closed list, but with a lower forward cost
                            foreach (var closedNode in closedList)
                            {
                                if (closedNode.GetNumber() == node.GetNumber() && closedNode.GetForwardCost() < node.GetForwardCost())
                                {
                                    skip = true;
                                }
                            }

                            // Add the node to the list of nodes to be examined if it has not yet been explored with that low of a forward cost
                            if (!skip)
                            {
                                openList.Add(new Node(node));
                            }
                        }
                    }
                }
            }

            // Add q to the list of nodes that have been examined
            closedList.Add(new Node(q));
        }
        

        // Update the pathway if goal is found, else clear the pathway list
        if (goalNode.GetNumber() == playerNode)
            GeneratePathway(goalNode);
        else
            pathway = new List<Vector2>();
    }

    private Node FindCurrentNode()
    {
        List<Node> closeNodes = new List<Node>();
        Vector2 pos = new Vector2(this.transform.position.x, this.transform.position.y);
        Node currentNode;

        foreach (var node in nodes)
        {
            Vector2 nodePos = node.GetPos();
            if (!Physics2D.Linecast(pos, nodePos, wallMask))
                closeNodes.Add(node);
        }

        if (closeNodes.Count > 0)
        {
            float enemyX = pos.x;
            float enemyY = pos.y;
            Vector2 nodePos = closeNodes[0].GetPos();
            float closestDistance = Mathf.Sqrt(Mathf.Pow(enemyX - nodePos.x, 2.0f) + Mathf.Pow(enemyY - nodePos.y, 2.0f));
            currentNode = closeNodes[0];

            for (int i = 1; i < closeNodes.Count; ++i)
            {
                nodePos = closeNodes[i].GetPos();
                float nodeDist = Mathf.Sqrt(Mathf.Pow(enemyX - nodePos.x, 2.0f) + Mathf.Pow(enemyY - nodePos.y, 2.0f));

                if (nodeDist < closestDistance)
                {
                    closestDistance = nodeDist;
                    currentNode = closeNodes[i];
                }
            }
        }
        else
        {
            currentNode = null;
        }

        return currentNode;
    }

    private float CalculateDistance(Node n, Node goal)
    {
        Vector2 startNodePos = n.GetPos();
        Vector2 goalNodePos = goal.GetPos();

        return Mathf.Sqrt(Mathf.Pow(goalNodePos.x - startNodePos.x, 2.0f) + Mathf.Pow(goalNodePos.y - startNodePos.y, 2.0f));
    }

    private Node FindLowestCostNode(List<Node> openList)
    {
        Node lowestCostNode = openList[0];
        float lowestCost = lowestCostNode.GetForwardCost();

        for (int i = 1; i < openList.Count; ++i)
        {
            Node n = openList[i];
            float nCost = n.GetForwardCost();

            if (nCost < lowestCost)
            {
                lowestCostNode = n;
                lowestCost = nCost;
            }
        }

        return lowestCostNode;
    }

    private void GeneratePathway(Node goal)
    {
        pathway.Clear();

        List<int> nodePath = goal.GetPathway();

        foreach(var num in nodePath)
        {
            pathway.Add(nodes[num].GetPos());
        }

        if (DebugMode)
        {
            OutputPathway();
        }
    }

    private void OutputPathway()
    {
        string message = "Pathway:\n";

        int counter = 0;
        foreach(var pos in pathway)
        {
            message += "Node: " + counter++ + " at position: " + pos.x + ", " + pos.y + "\n";
        }

        Debug.Log(message);
    }

    private void FollowPath()
    {
        bool found = false;
        // Find node closest to the player that enemy is in range of
        for (int i = pathway.Count - 1; i > -1; --i)
        {
            // Remove all nodes that have been past
            if (found)
                pathway.RemoveAt(i);

            if (!Physics2D.Linecast(pathway[i], this.transform.position, wallLayerMask))
            {
                Vector2 nextNodePos = pathway[i];
                // Move towards node
                MoveOnPath(nextNodePos);
                // Remove node if close
                CheckPathProgression(nextNodePos);
                found = true;
            }
        }
    }

    private void MoveOnPath(Vector2 nodePos)
    {
        Vector3 pos = transform.position;
        velocity = new Vector2(velocity.x, velocity.y);

        float angle;
        Vector3 enemyPos = this.transform.position;
        Vector2 vecEnemyToNode = new Vector2(nodePos.x - enemyPos.x, nodePos.y - enemyPos.y);
        float modVec = Mathf.Sqrt(Mathf.Pow(vecEnemyToNode.x, 2.0f) + Mathf.Pow(vecEnemyToNode.y, 2.0f));
        float dotProduct = vecEnemyToNode.x / modVec;
        if (nodePos.y < enemyPos.y)
            angle = 360.0f - (Mathf.Acos(dotProduct) * Mathf.Rad2Deg);
        else
            angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        velocity.x = flySpeed * Mathf.Cos(angle * Mathf.Deg2Rad);
        velocity.y = flySpeed * Mathf.Sin(angle * Mathf.Deg2Rad);

        if (nodePos.x >= transform.position.x) // node on right
        {
            if (!(nodePos.x < transform.position.x + 0.1f)) // not a tiny difference to stop flickering 
                sr.flipX = true;
            Debug.DrawRay(transform.position, Vector2.right, Color.red, spriteXSize + 0.1f);
            if (Physics2D.Raycast(transform.position, Vector2.right, (spriteXSize + 0.1f), wallLayerMask)) // not going to collide with a wall
                velocity.x = 0; // don't go, there is an obstacle
        }
        else // node on left
        {
            if (!(nodePos.x > transform.position.x - 0.1f)) // not a tiny difference to stop flickering 
                sr.flipX = false;
            Debug.DrawRay(transform.position, Vector2.left, Color.red, -spriteXSize - 0.1f);
            if (Physics2D.Raycast(transform.position, Vector2.left, (spriteXSize + 0.1f), wallLayerMask))
                velocity.x = 0;
        }
        if (nodePos.y >= transform.position.y) // node above
        {
            if (!(nodePos.y < transform.position.y + 0.1f)) // not a tiny difference to stop flickering 
                sr.flipY = false;
            Debug.DrawRay(transform.position, Vector2.up, Color.red, spriteYSize + 0.1f);
            if (Physics2D.Raycast(transform.position, Vector2.up, (spriteYSize + 0.1f), wallLayerMask))
                velocity.y = 0;
        }
        else // node below
        {
            if (!(nodePos.y > transform.position.y - 0.1f)) // not a tiny difference to stop flickering 
                sr.flipY = true;
            Debug.DrawRay(transform.position, Vector2.down, Color.red, -spriteYSize - 0.1f);
            if (Physics2D.Raycast(transform.position, Vector2.down, (spriteYSize + 0.1f), wallLayerMask))
                velocity.y = 0;
        }

        pos.x += velocity.x * Time.deltaTime;
        pos.y += velocity.y * Time.deltaTime;
        transform.position = pos;
    }

    private void CheckPathProgression(Vector2 nodePos)
    {
        Vector2 enemyPos = this.transform.position;
        Vector2 vecEnemyToNode = new Vector2(nodePos.x - enemyPos.x, nodePos.y - enemyPos.y);
        float distanceToNode = Mathf.Sqrt(Mathf.Pow(vecEnemyToNode.x, 2.0f) + Mathf.Pow(vecEnemyToNode.y, 2.0f));

        if (distanceToNode < 0.15f)
        {
            pathway.RemoveAt(0);
        }
    }

    public override void Hit(int damage, int attackDir)
    {
        Knockback(attackDir);
    }
}
