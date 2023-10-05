using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefabs;
    private SphereCollider collider;
    private List<GameObject> monsters = new List<GameObject>();
    public bool ActiveSpawner {  get; private set; }
    private DungeonManager dm;

    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
        ActiveSpawner = false;
        dm = DungeonManager.instance;
    }

    public void Update()
    {
        if (!ActiveSpawner)
            return;

        if(monsters.Count == 0)
        {
            dm.SpawnerRelese();
            ActiveSpawner = false;
        }
    }

    public void Die()
    {
        for(int i=monsters.Count-1;i>=0;--i)
        {
            Destroy(monsters[i]);
        }
        monsters.Clear();
    }

    public void Spawn()
    {
        ActiveSpawner = true;
        int spawnCount = Random.RandomRange(3, 6);

        Vector3 spawnPos;

        for (int i = 0; i < spawnCount; i++)
        {
            spawnPos.x = Random.Range(collider.bounds.min.x, collider.bounds.max.x);
            spawnPos.y = 0f;
            spawnPos.z = Random.Range(collider.bounds.min.z, collider.bounds.max.z);

            var obj = Instantiate(monsterPrefabs, spawnPos, Quaternion.identity);
            obj.transform.parent = this.transform;
            
            var info = obj.GetComponent<MonsterInfo>();
            info.StateUpdate();
            info.SetUp();

            monsters.Add(obj);
        }
    }
}
