using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    public GameObject playerPrefab;
    public FixedJoystick joystic;

    public List<GameObject> monsterSpawnerPool = new List<GameObject>();
    private List<GameObject> activeSpawner = new List<GameObject>();
    private List<GameObject> deactivationSpawner = new List<GameObject>();

    public int SpawnCount = 0;

    private void Awake()
    {
        PlayerSpawn();

        foreach (var obj in monsterSpawnerPool)
        {
            deactivationSpawner.Add(obj);
        }
    }


    private void Update()
    {
        Debug.Log(activeSpawner.Count);
        if(activeSpawner.Count<SpawnCount)
        {
            MonsterSpawn();
        }
    }
    private void MonsterSpawn()
    {
        if (SpawnCount > monsterSpawnerPool.Count)
        {
            Debug.Log("Spawn Count err");
            return;
        }

        while (activeSpawner.Count<SpawnCount)
        {
            int randomIndex = Random.RandomRange(0, deactivationSpawner.Count);
            var obj = deactivationSpawner[randomIndex];
            activeSpawner.Add(obj);
            deactivationSpawner.Remove(obj);

            obj.GetComponent<MonsterSpawner>().Spawn();
        }
    }

    private void PlayerSpawn()
    {
        GameObject player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        var controller = player.GetComponent<PlayerController>();
        var movement = player.GetComponent<PlayerMovement>();

        controller.joystick = joystic;
        movement.vCamera = vCam;
        movement.vCamera.Follow = player.transform;
        movement.vCamera.LookAt = player.transform;
        movement.Setup();
    }
}
