using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPlayer : MonoBehaviour
{
    private SpawnPoint[] _spawnPoints; // make sure there are at least 8 spawn points one for each player

    public void SetPlayerSpawn(PlayerInput[] players)
    {
        _spawnPoints = FindObjectsOfType<SpawnPoint>();

        if (_spawnPoints.Length < players.Length)
        {
            Debug.LogError("There are not enough spawnpoints for each player");
            return;
        }

        foreach (PlayerInput player in players)
        {

            //grab random spawn point to spawn player in
            SpawnPoint spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            while (spawnPoint.spawnTaken)
            {
                spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            }

            Respawn playerRespawn = player.GetComponent<Respawn>();

            //set player spawnpoint and prevent another player from spawning there
            playerRespawn.SpawnPoint = spawnPoint.Position;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player.transform.position = playerRespawn.SpawnPoint;
            spawnPoint.spawnTaken = true;
            player.gameObject.SetActive(true);
        }
    }
}
