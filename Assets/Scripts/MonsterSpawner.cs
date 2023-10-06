using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefabs;
    public MonsterType type = MonsterType.Mob;
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

        switch(type)
        {
            case MonsterType.Mob:
                if (monsters.Count == 0)
                {
                    dm.SpawnerRelese();
                    ActiveSpawner = false;
                }
                break;

            case MonsterType.SubBoss:
                break;

            case MonsterType.Boss:
                break;
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

        switch(type)
        {
            case MonsterType.Mob:
                { 
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
                        info.SetType(type);
                        info.StateUpdate();
                        info.SetUp();

                        monsters.Add(obj);
                    }
                }
                break;

            case MonsterType.SubBoss:
                {
                    Vector3 spawnPos;
                    spawnPos.x = Random.Range(collider.bounds.min.x, collider.bounds.max.x);
                    spawnPos.y = 0f;
                    spawnPos.z = Random.Range(collider.bounds.min.z, collider.bounds.max.z);
                    var obj = Instantiate(monsterPrefabs, spawnPos, Quaternion.identity);
                    obj.transform.parent = this.transform;
                    obj.transform.localScale *= 1.5f;
                    var info = obj.GetComponent<MonsterInfo>();
                    info.SetType(type);
                    info.StateUpdate();
                    info.SetUp();
                    monsters.Add(obj);
                }
                break;
        }

    }
}
