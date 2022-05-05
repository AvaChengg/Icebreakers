using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryManagement : MonoBehaviour
{
    [SerializeField] private Transform _winnerPosition;
    [SerializeField] private Transform[] _loserPositions;
    private PlayerController[] players;

    private void Awake()
    {
        players = FindObjectsOfType<PlayerController>();
        if (players == null) return;
        


    }

    private void Update()
    {
        int currentPos = 0;
        foreach (PlayerController player in players)
        {
            if (player.PlayerData.PlayerID == VictoryVolume.PlayerData.PlayerID)
            {
                player.gameObject.transform.position = _winnerPosition.position;
                player.gameObject.transform.rotation = _winnerPosition.rotation;
            }
            else
            {
                player.gameObject.transform.position = _loserPositions[currentPos].position;
                player.gameObject.transform.rotation = _loserPositions[currentPos].rotation;
                currentPos++;
            }
        }
    }
}
