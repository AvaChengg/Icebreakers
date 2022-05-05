using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPlayerJoin : MonoBehaviour
{
    [SerializeField] private GameObject[] _penguins;

    public void ActivatePenguin(int playerIndex)
    {
        _penguins[playerIndex].SetActive(true);
    }

    public void DeactivatePenguin(int playerIndex)
    {
        _penguins[playerIndex].SetActive(false);
    }
}
