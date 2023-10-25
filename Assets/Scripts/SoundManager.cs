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

    private AudioSource soundPlayer;

    [SerializeField] private AudioClip bg_Dungeon_Boss;
    [SerializeField] private AudioClip bg_Dungeon_SubBoss;
    [SerializeField] private AudioClip bg_Dungeon_Normal;
    [SerializeField] private AudioClip bg_Title;
    [SerializeField] private AudioClip bg_Lobby;
    [SerializeField] private AudioClip bg_Dungeon_Victory;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        soundPlayer = GetComponent<AudioSource>();
    }

    public void PlaySound(string id)
    {
        if(soundPlayer.isPlaying)
        {
            soundPlayer.Stop();
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
        if (soundPlayer.clip != bg_Dungeon_Boss)
        {
            soundPlayer.clip = bg_Dungeon_Boss;
        }

        soundPlayer.loop = true;
        soundPlayer.volume = 1f;

        soundPlayer.Play();
    }

    private void SubBossBGM()
    {
        if (soundPlayer.clip != bg_Dungeon_SubBoss)
        {
            soundPlayer.clip = bg_Dungeon_SubBoss;
        }

        soundPlayer.loop = true;
        soundPlayer.volume = 1f;

        soundPlayer.Play();
    }

    private void NormalBGM()
    {
        if (soundPlayer.clip != bg_Dungeon_Normal)
        {
            soundPlayer.clip = bg_Dungeon_Normal;
        }

        soundPlayer.loop = true;
        soundPlayer.volume = 1f;

        soundPlayer.Play();
    }

    private void TitleBGM()
    {
        if (soundPlayer.clip != bg_Title)
        {
            soundPlayer.clip = bg_Title;
        }

        soundPlayer.loop = true;
        soundPlayer.volume = 0.5f;
        soundPlayer.Play();
    }

    private void LobbyBGM()
    {
        if (soundPlayer.clip != bg_Lobby)
        {
            soundPlayer.clip = bg_Lobby;
        }

        soundPlayer.loop = true;
        soundPlayer.volume = 0.5f;
        soundPlayer.Play();
    }

    private void VictoryBGM()
    {
        if (soundPlayer.clip != bg_Dungeon_Victory)
        {
            soundPlayer.clip = bg_Dungeon_Victory;
        }

        soundPlayer.loop = false;
        soundPlayer.volume = 0.5f;
        soundPlayer.Play();
    }
}
