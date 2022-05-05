using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UniRx;

public class MultiplayerManager : MonoBehaviour
{
    private ShowPlayerJoin _showPlayerJoin;

    private void Awake()
    {
        _showPlayerJoin = GetComponent<ShowPlayerJoin>();
    }

    private void Start()
    {
        AutoJoin();
    }

    private void AutoJoin()
    {
        for (int i = 0; i < 8; i++)
        {
            PlayerInputManager.instance.JoinPlayer();
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        _showPlayerJoin.ActivatePenguin(playerInput.playerIndex);
        playerInput.transform.position = transform.position;
        DontDestroyOnLoad(playerInput.gameObject);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log($"Player: {playerInput.playerIndex} Left");
        _showPlayerJoin.DeactivatePenguin(playerInput.playerIndex);
        Destroy(playerInput.gameObject);
    }
}
