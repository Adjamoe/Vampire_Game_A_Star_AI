using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    GameObject enemy;
    RoomManager rm;
    public int enemyToSpawn;
    private void Awake()
    {
        rm = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
    }

    public void SpawnEnemy()
    {
        enemy = Instantiate(rm.GetEnemy(enemyToSpawn), transform.position, Quaternion.identity);
        if(enemyToSpawn == 4)
        {
            enemy.GetComponent<BossBehaviour>().Start();
        }
    }
    public void DestroyEnemy()
    {
        Destroy(enemy);
    }
}
