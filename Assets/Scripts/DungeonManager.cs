using Cinemachine;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private List<GameObject> monsterSpawnerPool = new List<GameObject>();
    private List<GameObject> activeSpawner = new List<GameObject>();

    private GameObject NonSpawn =null;
    private bool spawn = true;
    public int SpawnCount = 0;

    private void Awake()
    {
        PlayerSpawn();

        var list = gameObject.GetComponentsInChildren<MonsterSpawner>();

        foreach (var spawner in list)
        {
            monsterSpawnerPool.Add(spawner.gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            activeSpawner[0].GetComponent<MonsterSpawner>().Die();
        }
        if(activeSpawner.Count<SpawnCount && spawn)
        {
            MonsterSpawn();
        }
    }

    private void MonsterSpawn()
    {
        if(activeSpawner.Count+monsterSpawnerPool.Count<SpawnCount)
        {
            Debug.Log("Spawn Count Err");
            return;
        }

        while (activeSpawner.Count<SpawnCount)
        {
            int randomIndex;
            GameObject obj;
            while(true)
            {
                randomIndex = Random.RandomRange(0, monsterSpawnerPool.Count);
                obj = monsterSpawnerPool[randomIndex];

                if(obj == NonSpawn)
                {
                    continue;
                }
                break;
            }
            activeSpawner.Add(obj);
            monsterSpawnerPool.Remove(obj);

            obj.GetComponent<MonsterSpawner>().Spawn();
        }
    }

    public void SpawnerRelese()
    {
        foreach (var obj in activeSpawner)
        {
            if (obj.GetComponent<MonsterSpawner>().ActiveSpawner)
            {
                NonSpawn = obj;
                monsterSpawnerPool.Add(obj);
                activeSpawner.Remove(obj);
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
