using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    public GameObject playerPrefab;
    public FixedJoystick joystic;

    public List<GameObject> monsterSpawner = new List<GameObject>();

    private void Awake()
    {
        PlayerSpawn();
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
