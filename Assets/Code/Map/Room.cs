using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private int x, y;
    private GameObject roomLayout, mapLayout;
    private GameObject leftDoor, rightDoor, topDoor, bottomDoor;
    private bool unlocked = false;
    private GameObject resPoint;
    private GameObject[] enemySpawns;
    private List<GameObject> enemiesInRoom;
    public Room(int _x, int _y, GameObject _roomLayout, GameObject _mapLayout)
    {
        x = _x;
        y = _y;
        roomLayout = _roomLayout;
        mapLayout = _mapLayout;
    }
    public void CreateRoom()
    {
        leftDoor = GameObject.FindGameObjectWithTag("LeftDoor");
        rightDoor = GameObject.FindGameObjectWithTag("RightDoor");
        topDoor = GameObject.FindGameObjectWithTag("TopDoor");
        bottomDoor = GameObject.FindGameObjectWithTag("BottomDoor");
        resPoint = GameObject.FindGameObjectWithTag("ResPoint");

        enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawn");
    }
    public GameObject GetRoomLayout()
    {
        return roomLayout;
    }
    public void SetRoomLayout(GameObject layout)
    {
        roomLayout = layout;
    }
    public GameObject GetMapRoomLayout()
    {
        return mapLayout;
    }
    public int GetXPos()
    {
        return x;
    }
    public int GetYPos()
    {
        return y;
    }
    public bool GetUnlocked()
    {
        return unlocked;
    }
    public void SetUnlocked()
    {
        unlocked = true;
    }
    public Vector3 GetSpawnPlayerLocation(int _enterDirection)
    {
        //Debug.Log("Right: " + rightDoor.transform.position);
        // return 0 = left
        // return 1 = right
        // return 2 = top
        // return 3 = bottom
        switch(_enterDirection)
        {
            case 0:
                return rightDoor.transform.position + (Vector3.left * 5.0f);
            case 1:
                return leftDoor.transform.position + (Vector3.right * 5.0f);
            case 2:
                return bottomDoor.transform.position + (Vector3.up * 6.0f) + (Vector3.right * 5.0f);
            case 3:
                return topDoor.transform.position + (Vector3.down * 5.0f);
            default:
                return leftDoor.transform.position + (Vector3.right * 5.0f);
        }
    }
    public Vector3 GetRespawnPlayerLocation()
    {
        if (resPoint != null)
            return resPoint.transform.position + Vector3.up * 2.0f;
        else
            return Vector3.zero;
    }
    public void SpawnEnemies()
    {
        for(int i = 0; i < enemySpawns.Length; i++)
        {
            enemySpawns[i].GetComponent<EnemySpawn>().SpawnEnemy();
        }
    }
    public void ClearEnemies()
    {
        for (int i = 0; i < enemySpawns.Length; i++)
        {
            enemySpawns[i].GetComponent<EnemySpawn>().DestroyEnemy();
        }
    }
}
