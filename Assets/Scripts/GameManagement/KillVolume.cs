using UnityEngine;

public class KillVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IRespawn respawn))
        {
            respawn.OnRespawn();
        }
    }
}
