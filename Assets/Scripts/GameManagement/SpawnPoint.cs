using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Vector3 Position => transform.position;
    public bool spawnTaken { get; set; }
}
