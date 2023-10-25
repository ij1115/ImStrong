using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
            }
            return instance;
        }
    }

    [SerializeField] private AudioSource bgmPlayer;
    [SerializeField] private AudioSource sfxPlayer;

    [SerializeField] private AudioClip bg_Dungeon_Boss;
    [SerializeField] private AudioClip bg_Dungeon_SubBoss;
    [SerializeField] private AudioClip bg_Dungeon_Normal;
    [SerializeField] private AudioClip bg_Title;
    [SerializeField] private AudioClip bg_Lobby;
    [SerializeField] private AudioClip bg_Dungeon_Victory;
    [SerializeField] private AudioClip axe_Attack;
    [SerializeField] private AudioClip axe_FSkill_1;
    [SerializeField] private AudioClip axe_FSkill_2;
    [SerializeField] private AudioClip axe_SSkill;
    [SerializeField] private AudioClip spear_Attack;
    [SerializeField] private AudioClip spear_FSkill;
    [SerializeField] private AudioClip spear_SSkill;
    [SerializeField] private AudioClip sword_Attack;
    [SerializeField] private AudioClip sword_FSkill;
    [SerializeField] private AudioClip sword_SSkill;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SfxPlay(string id)
    {
        switch (id)
        {
            case "Axe_Attack":
                sfxPlayer.PlayOneShot(axe_Attack);
                break;

            case "Axe_FSkill_1":
                sfxPlayer.PlayOneShot(axe_FSkill_1);
                break;
            case "Axe_FSkill_2":
                sfxPlayer.PlayOneShot(axe_FSkill_2);
                break;
            case "Axe_SSkill":
                sfxPlayer.PlayOneShot(axe_SSkill);
                break;
            case "Spear_Attack":
                sfxPlayer.PlayOneShot(spear_FSkill);
                break;
            case "Spear_FSkill":
                sfxPlayer.PlayOneShot(spear_SSkill);
                break;
            case "Spear_SSkill":
                sfxPlayer.PlayOneShot(spear_SSkill);
                break;
            case "Sword_Attack":
                sfxPlayer.PlayOneShot(sword_Attack);
                break;
            case "Sword_FSkill":
                sfxPlayer.PlayOneShot(sword_FSkill);
                break;
            case "Sword_SSkill":
                sfxPlayer.PlayOneShot(sword_SSkill);
                break;
        }
    }
    public void PlaySound(string id)
    {
        if(bgmPlayer.isPlaying)
        {
            bgmPlayer.Stop();
        }

        switch(id)
        {
            case "Boss":
                BossBGM();
                break;

            case "SubBoss":
                SubBossBGM();
                break;

            case "Normal":
                NormalBGM();
                break;

            case "Title":
                TitleBGM();
                break;

            case "Lobby":
                LobbyBGM();
                break;

            case "Victory":
                VictoryBGM();
                break;
        }
    }

    private void BossBGM()
    {
        if (bgmPlayer.clip != bg_Dungeon_Boss)
        {
            bgmPlayer.clip = bg_Dungeon_Boss;
        }

        bgmPlayer.loop = true;
        bgmPlayer.volume = 1f;

        bgmPlayer.Play();
    }

    private void SubBossBGM()
    {
        if (bgmPlayer.clip != bg_Dungeon_SubBoss)
        {
            bgmPlayer.clip = bg_Dungeon_SubBoss;
        }

        bgmPlayer.loop = true;
        bgmPlayer.volume = 1f;

        bgmPlayer.Play();
    }

    private void NormalBGM()
    {
        if (bgmPlayer.clip != bg_Dungeon_Normal)
        {
            bgmPlayer.clip = bg_Dungeon_Normal;
        }

        bgmPlayer.loop = true;
        bgmPlayer.volume = 1f;

        bgmPlayer.Play();
    }

    private void TitleBGM()
    {
        if (bgmPlayer.clip != bg_Title)
        {
            bgmPlayer.clip = bg_Title;
        }

        bgmPlayer.loop = true;
        bgmPlayer.volume = 0.5f;
        bgmPlayer.Play();
    }

    private void LobbyBGM()
    {
        if (bgmPlayer.clip != bg_Lobby)
        {
            bgmPlayer.clip = bg_Lobby;
        }

        bgmPlayer.loop = true;
        bgmPlayer.volume = 0.5f;
        bgmPlayer.Play();
    }

    private void VictoryBGM()
    {
        if (bgmPlayer.clip != bg_Dungeon_Victory)
        {
            bgmPlayer.clip = bg_Dungeon_Victory;
        }

        bgmPlayer.loop = false;
        bgmPlayer.volume = 0.5f;
        bgmPlayer.Play();
    }
}
