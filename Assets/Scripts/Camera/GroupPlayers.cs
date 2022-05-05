using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GroupPlayers : MonoBehaviour
{
    private CinemachineTargetGroup _cinemachineTargetGroup;

    private void Awake()
    {
        _cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>(true);

        foreach(PlayerController player in players)
        {
            _cinemachineTargetGroup.AddMember(player.transform, 1, 0);
        }
    }
}
