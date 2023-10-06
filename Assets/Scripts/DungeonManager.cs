using Cinemachine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager instance
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType<DungeonManager>();
            }
            return singleton;
        }
    }

    private static DungeonManager singleton;

    public CinemachineVirtualCamera vCam;
    public GameObject playerPrefab;
    public FixedJoystick joystic;

    public float spawnTimer = 3f;

    private List<GameObject> mobSpawner = new List<GameObject>();
    private List<GameObject> mobActive = new List<GameObject>();

    //사냥한 몬스터의 스폰 지역
    private GameObject mobSpawnPos = null;
    
    private bool spawn = true;
    public int SpawnCount = 0;

    private List<GameObject> subBossSpawner = new List<GameObject>();
    private List<GameObject> subBossActive = new List<GameObject>();

    //사냥한 서브보스의 스폰 지역
    private GameObject subBossSpawnPos = null;
    public int huntSubBossCount = 0;

    private void Awake()
    {
        huntSubBossCount = 0;
        foreach (Transform obj in gameObject.transform)
        {
            switch (obj.name)
            {
                case "MobSpawner":
                    {
                        var list = obj.GetComponentsInChildren<MonsterSpawner>();

                        foreach (var spawn in list)
                        {
                            mobSpawner.Add(spawn.gameObject);
                        }
                    }
                    break;
                case "SubBossSpawner":
                    {
                        var list = obj.GetComponentsInChildren<MonsterSpawner>();

                        foreach (var spawn in list)
                        {
                            subBossSpawner.Add(spawn.gameObject);
                        }
                    }
                    break;
            }
        }

        PlayerSpawn();

        //이전 코드
        //var list = gameObject.GetComponentsInChildren<MonsterSpawner>();
        //foreach (var spawner in list)
        //{
        //    mobSpawner.Add(spawner.gameObject);
        //}
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            mobActive[0].GetComponent<MonsterSpawner>().Die();
        }

        if(mobActive.Count<SpawnCount && 
            spawn)
        {
            MonsterSpawn();
        }

        if(subBossActive.Count<1 &&
            huntSubBossCount< 2 &&
            spawn)
        {
            SubBossSpawn();
        }
    }

    private void MonsterSpawn()
    {
        if(mobActive.Count+ mobSpawner.Count<SpawnCount)
        {
            Debug.Log("Spawn Count Err");
            return;
        }

        while (mobActive.Count<SpawnCount)
        {
            int randomIndex;
            GameObject obj;
            while(true)
            {
                randomIndex = Random.RandomRange(0, mobSpawner.Count);
                obj = mobSpawner[randomIndex];

                if(obj == mobSpawnPos)
                {
                    continue;
                }
                break;
            }
            mobActive.Add(obj);
            mobSpawner.Remove(obj);

            obj.GetComponent<MonsterSpawner>().Spawn();
        }
    }

    private void SubBossSpawn()
    {
        if (huntSubBossCount>=2)
        {
            Debug.Log("Kill Count Over");
            return;
        }

        while (subBossActive.Count < 1)
        {
            int randomIndex;
            GameObject obj;
            while (true)
            {
                randomIndex = Random.RandomRange(0, subBossSpawner.Count);
                obj = subBossSpawner[randomIndex];

                if (obj == subBossSpawnPos)
                {
                    continue;
                }
                break;
            }
            subBossActive.Add(obj);
            subBossSpawner.Remove(obj);

            obj.GetComponent<MonsterSpawner>().Spawn();
        }
    }

    public void SpawnerRelese()
    {
        foreach (var obj in mobActive)
        {
            if (obj.GetComponent<MonsterSpawner>().ActiveSpawner)
            {
                mobSpawnPos = obj;
                mobSpawner.Add(obj);
                mobActive.Remove(obj);
                StartCoroutine(SpawnRoutine());
                return;
            }
        }
    }

    public void SubBossRelese()
    {
        foreach (var obj in subBossActive)
        {
            if (obj.GetComponent<MonsterSpawner>().ActiveSpawner)
            {
                subBossSpawnPos = obj;
                subBossSpawner.Add(obj);
                subBossActive.Remove(obj);
                StartCoroutine(SpawnRoutine());
                return;
            }
        }
    }

    private void PlayerSpawn()
    {
        GameObject player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        var controller = player.GetComponent<PlayerController>();
        var movement = player.GetComponent<PlayerMovement>();
        var info = player.GetComponent<PlayerInfo>();


      
        info.StateUpdate();
        info.SetUp();

        controller.joystick = joystic;
        movement.vCamera = vCam;
        movement.vCamera.Follow = player.transform;
        movement.vCamera.LookAt = player.transform;
        movement.Setup();

    }

    private IEnumerator SpawnRoutine()
    {
        spawn = false;
        yield return new WaitForSeconds(spawnTimer);
        spawn = true;
    }
}
