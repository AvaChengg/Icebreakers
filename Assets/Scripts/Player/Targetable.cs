using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    [SerializeField] private int _team;

    // Public get
    public int Team { get => _team; }
}
