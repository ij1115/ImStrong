using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    public GameObject playerPrefab;
    public FixedJoystick joystic;

    public List<GameObject> monsterSpawnerPool = new List<GameObject>();


    public int SpawnCount = 0;

    private void Awake()
    {
        PlayerSpawn();
    }

    //private void MonsterSpawn()
    //{
    //    foreach(var spawn in monsterSpawner)
    //    {
    //        spawn.GetComponent<MonsterSpawner>().Spawn();
    //    }
    //}

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
