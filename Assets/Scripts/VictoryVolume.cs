using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VictoryVolume : MonoBehaviour
{
    private static PlayerData _playerData;
    public static PlayerData PlayerData => _playerData;
    private bool _isTriggerable = true;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out PlayerController playerController) && _isTriggerable == true)
        {
            _playerData = playerController.PlayerData;
            _isTriggerable = false;
            SceneManager.LoadScene("VictoryScene");
        }
    }
}
