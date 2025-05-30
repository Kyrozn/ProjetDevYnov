
using Mirror;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;

    public override void OnStartServer()
    {
        int enemyCount = NetworkServer.connections.Count * 3;

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
            GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
            NetworkServer.Spawn(enemy);
        }
    }
}