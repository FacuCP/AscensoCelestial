using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private Collider spawnArea;
    public GameObject[] enemyPrefabs;
    private Vector3 areaSize;
    private Vector3 areaCenter;
    private int cantidad;

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
        areaSize = spawnArea.bounds.size;
        areaCenter = spawnArea.bounds.center;
    }

    public void SpawnEnemies(int cant)
    {
        cantidad = cant;
        for(int i=0;i<cant;i++) SpawnEnemy();
    }

    private void SpawnEnemy() {
        float x = UnityEngine.Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2);
        float y = areaCenter.y; // o cualquier valor fijo según necesidad
        float z = UnityEngine.Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2);
        Vector3 spawnPos = new Vector3(x, y, z);

        int prefabIndex = UnityEngine.Random.Range(0, enemyPrefabs.Length);
        GameObject enemy = Instantiate(enemyPrefabs[prefabIndex], spawnPos, Quaternion.identity);
        BaseEnemySM sm = enemy.GetComponent<BaseEnemySM>();

        HandlerVida hV = sm.GetComponentInChildren<HandlerVida>();
        void OnDeath()
        {
            hV.Murio -= OnDeath;
            RestaEnemigo();
        }

        hV.Murio += OnDeath;
    }

    public Action SpawnTerminado;

    private void RestaEnemigo()
    {
        cantidad--;
        if(cantidad == 0)
        {
            SpawnTerminado?.Invoke();
        }
    }
}
