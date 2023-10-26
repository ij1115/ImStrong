using Cinemachine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using Unity.VisualScripting;

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


    public Camera currCamera;
    public CinemachineVirtualCamera vCam;
    public GameObject playerPrefab;
    private GameObject playerManager;
    public FixedJoystick joystic;
    public FloatingJoystick fjoystick;
    public Slider hpSlider;

    private List<GameObject> mobSpawner = new List<GameObject>();
    private List<GameObject> mobActive = new List<GameObject>();

    private bool spawn = false;

    private List<GameObject> subBossSpawner = new List<GameObject>();
    private List<GameObject> subBossActive = new List<GameObject>();

    private List<GameObject> portalSpawner = new List<GameObject>();
    private List<GameObject> portalActive = new List<GameObject>();

    private List<GameObject> BossSpawner = new List<GameObject>();
    private List<GameObject> BossActive = new List<GameObject>();

    private bool stageClear = false;

    private void Awake()
    {
        UIManager.Instance.ChangeCamera(currCamera);
        foreach (Transform obj in gameObject.transform)
        {
            if (obj == null)
                return;

            switch (obj.name)
            {
                case "MobSpawner":
                    {
                        var list = obj.GetComponentsInChildren<Spawner>();

                        foreach (var spawn in list)
                        {
                            mobSpawner.Add(spawn.gameObject);
                        }
                    }
                    break;
                case "SubBossSpawner":
                    {
                        var list = obj.GetComponentsInChildren<Spawner>();

                        foreach (var spawn in list)
                        {
                            subBossSpawner.Add(spawn.gameObject);
                        }
                    }
                    break;
                case "PortalSpawner":
                    {
                        var list = obj.GetComponentsInChildren<Spawner>();

                        foreach (var spawn in list)
                        {
                            portalSpawner.Add(spawn.gameObject);
                        }
                    }
                    break;
                case "BossSpawner":
                    {
                        var list = obj.GetComponentsInChildren<Spawner>();

                        foreach (var spawn in list)
                        {
                            BossSpawner.Add(spawn.gameObject);
                        }
                    }
                    break;
            }
        }

        PlayerSpawn();
    }

    public void Reset()
    {
        spawn = false;
        stageClear = false;

        mobActive.Clear();
        
        subBossActive.Clear();

        portalActive.Clear();

        BossActive.Clear();
    }

    private void Update()
    {
        if (!stageClear && !spawn)
        {
            StageSpawn();
            spawn = true;

            return;
        }

        if (!stageClear && spawn)
        {
            if (mobActive.Count == 0 && subBossActive.Count == 0 && BossActive.Count == 0)
            {
                SoundManager.Instance.PlaySound("Victory");
                PortalSpawn();
                stageClear = true;
            }
        }

        if (GameManager.instance.isGameover)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                GameManager.instance.isGameover = false;
                UIManager.Instance.Open(SceneState.Lobby);
                GameManager.instance.ChangeScene("Lobby");
            }

            return;
        }

    }
    

    private void MonsterSpawn(int index)
    {
        if (mobActive.Count > 0)
        {
            foreach (var mob in mobActive)
            {
                if (mob == mobSpawner[index].gameObject)
                {
                    return;
                }
            }
        }

        mobActive.Add(mobSpawner[index]);

        mobSpawner[index].GetComponent<Spawner>().Spawn();
    
    }

    private void SubBossSpawn()
    {
        if (subBossActive.Count>0)
        {
            return;
        }

        subBossActive.Add(subBossSpawner[0]);

        subBossSpawner[0].GetComponent<Spawner>().Spawn();
    }

    private void BossSpawn()
    {
        if(BossActive.Count>0)
        {
            return;
        }

        BossActive.Add(BossSpawner[0]);

        BossSpawner[0].GetComponent<Spawner>().Spawn();
    }
    private void PortalSpawn()
    {
        if(portalActive.Count>0)
        { 
            return;
        }

        portalActive.Add(portalSpawner[0]);

        portalSpawner[0].GetComponent<Spawner>().Spawn();
    }

    public void SpawnerRelese(GameObject reObj)
    {
        foreach (var obj in mobActive)
        {
            if (obj.GetComponent<Spawner>().ActiveSpawner && obj == reObj)
            {
                mobActive.Remove(obj);
                return;
            }
        }
    }

    public void SubBossRelese()
    {
        foreach (var obj in subBossActive)
        {
            if (obj.GetComponent<Spawner>().ActiveSpawner)
            {
                subBossActive.Remove(obj);
                return;
            }
        }
    }

    public void BossRelese()
    {
        foreach (var obj in BossActive)
        {
            if (obj.GetComponent<Spawner>().ActiveSpawner)
            {
                BossActive.Remove(obj);
                return;
            }
        }
    }

    public void PortalRelese()
    {
        foreach (var obj in portalActive)
        {
            if (obj.GetComponent<Spawner>().ActiveSpawner)
            {
                portalActive.Remove(obj);
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

        controller.joystick = UIManager.Instance.uis[2].GetComponent<DungeonUi>().SetJoystick();
        controller.fjoystick = UIManager.Instance.uis[2].GetComponent<DungeonUi>().SetFJoyStick();
        movement.fSkillSlider = UIManager.Instance.uis[2].GetComponent<DungeonUi>().fSkillSlider.GetComponent<Slider>();
        movement.sSkillSlider = UIManager.Instance.uis[2].GetComponent<DungeonUi>().sSkillSlider.GetComponent<Slider>();
        movement.vCamera = vCam;
        movement.vCamera.Follow = player.transform;
        movement.vCamera.LookAt = player.transform;
        movement.Setup();

        playerManager = player;
    }

    private void StageSpawn()
    {
        switch(GameData.Instance.data.stageLev%8)
        {
            case 1:
                SoundManager.Instance.PlaySound("Normal");
                MonsterSpawn(0);
                break;
            case 2:
                SoundManager.Instance.PlaySound("Normal");
                MonsterSpawn(1);
                MonsterSpawn(2);
                break;
            case 3:
                SoundManager.Instance.PlaySound("Normal");
                MonsterSpawn(1);
                MonsterSpawn(2);
                break;
            case 4:
                SoundManager.Instance.PlaySound("SubBoss");
                SubBossSpawn();
                break;
            case 5:
                SoundManager.Instance.PlaySound("Normal");
                MonsterSpawn(0);
                MonsterSpawn(1);
                MonsterSpawn(2);
                break;
            case 6:
                SoundManager.Instance.PlaySound("Normal");
                MonsterSpawn(0);
                MonsterSpawn(1);
                MonsterSpawn(2);
                break;
            case 7:
                SoundManager.Instance.PlaySound("Normal");
                MonsterSpawn(0);
                MonsterSpawn(1);
                MonsterSpawn(2);
                MonsterSpawn(3);
                break;
            case 0:
                SoundManager.Instance.PlaySound("Boss");
                BossSpawn();
                break;
        }
    }
}
