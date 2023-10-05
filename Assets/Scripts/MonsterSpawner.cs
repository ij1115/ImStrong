using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefabs;
    private SphereCollider collider;
    private List<GameObject> monsters = new List<GameObject>();
    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        int spawnCount = Random.RandomRange(3, 6);

        Vector3 spawnPos;

        for (int i = 0; i < spawnCount; i++)
        {
            spawnPos.x = Random.Range(collider.bounds.min.x, collider.bounds.max.x);
            spawnPos.y = 0f;
            spawnPos.z = Random.Range(collider.bounds.min.z, collider.bounds.max.z);
            
            Instantiate(monsterPrefabs, spawnPos, Quaternion.identity);
            monsters.Add(monsterPrefabs);
        }
    }
}
