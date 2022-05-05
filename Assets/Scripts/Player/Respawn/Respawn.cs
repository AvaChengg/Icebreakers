using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Respawn : MonoBehaviour, IRespawn
{
    [Header("SFX")]
    [SerializeField] private AudioClip _dieSFX;
    [SerializeField] private AudioClip _respawnSFX;

    [SerializeField] private float _respawnDelay = 0f;
    public Vector3 SpawnPoint { get; set; } = Vector3.zero;
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;
    private PlayerController _playerController;

    public float RespawnDelay => _respawnDelay;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _playerController = GetComponent<PlayerController>();
    }

    public void OnRespawn()
    {
        StartCoroutine(Respawning());
    }

    private IEnumerator Respawning()
    {

        AudioManager.instance?.PlaySFX(_dieSFX);
        _playerController.PlayerData.DeathCount++;

        _playerInput.DeactivateInput();
        yield return new WaitForSeconds(_respawnDelay);

        AudioManager.instance?.PlaySFX(_respawnSFX);

        _playerInput.ActivateInput();
        _rigidbody.velocity = Vector3.zero;
        transform.position = SpawnPoint;
    }
}
