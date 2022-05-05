using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoadPlayerMaterial : MonoBehaviour
{
    [SerializeField] private Material[] _playerMaterials = new Material[8];
    [SerializeField] private Mesh _clothingMesh;

    public void LoadPlayerColors(PlayerInput[] players)
    {
        foreach (PlayerInput player in players)
        {
            if (_playerMaterials[player.playerIndex] == null) break;

            //find correct gameobject with mesh
            MeshRenderer[] meshes = player.GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer mesh in meshes)
            {
                if(mesh.materials.Length == 1)
                {
                    mesh.material = _playerMaterials[player.playerIndex];
                }
            }
        }
    }
}
