using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private AudioClip _battleMusic;
    [SerializeField] private AudioClip _gameOverSfx;


    public Transform[] _fallingBlockSpawnPoints;

    private PlayerInput[] _players;


    private void Start()
    {
        MainEventHandler.EventStream<GameOverEvent>().Subscribe(OnGameOver).AddTo(this);
        _players = FindObjectsOfType<PlayerInput>(true);

        AudioManager.instance?.PlayMusic(_battleMusic);

        StartCoroutine(LoadGame());
    }

    public Transform GetRandomFallingBlockSpawnPoint()
    {       
        int r = Random.Range(0, _fallingBlockSpawnPoints.Length);
        return _fallingBlockSpawnPoints[r];
    }

    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        AudioManager.instance?.PlayMusic(_gameOverSfx);

        foreach (PlayerInput player in _players)
        {
            if(player.playerIndex == 0)
            {
                player.SwitchCurrentActionMap("UI");
            }
            else
            {
                player.DeactivateInput();
            }
        }
    }

    private void StartGame()
    {
        foreach(PlayerInput player in _players)
        {
            player.gameObject.SetActive(true);
            player.ActivateInput();
            player.SwitchCurrentActionMap("Player");
        }   
    }

    private IEnumerator LoadGame()
    {
        if (GetComponent<LevelRandomizer>().LoadRandomLevel())
        {
            while(FindObjectOfType<SpawnPoint>() == null)
            {
                yield return null;
            }
            //Set up players for start of game
            StartGame();
            GetComponent<LoadPlayerMaterial>()?.LoadPlayerColors(_players);
            GetComponent<SpawnPlayer>()?.SetPlayerSpawn(_players);
        }

    }
}
