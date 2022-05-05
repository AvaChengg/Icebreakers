using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour, ITriggerBreak, IPushable
{
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private ParticleSystem _collisionParticle;

    [SerializeField] private AudioClip[] _tauntListSfx;

    private Vector3 _moveInput;
    private PlayerMovement _movement;
    public PlayerData PlayerData => _playerData;
    private PlayerData _playerData;

    public bool IsBeingPushed => _movement.IsBeingPushed;

    public bool IsPushing => _movement.IsPushing;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        _playerData = new PlayerData(GetComponent<PlayerInput>().playerIndex);
    }

    // Receive move input from PlayerInput component (Vector2)
    public void OnMove(InputValue value)
    {
        // Read Vector2 data from InputValue
        Vector2 input = value.Get<Vector2>();
        _moveInput = new Vector3(input.x, 0, input.y);
        _moveInput.Normalize();

        Debug.DrawRay(transform.position, _moveInput, Color.magenta, 0.1f);
    }

    public void OnJump()
    {
        _movement.TryJump();
    }

    public void OnPush()
    {
        _movement.MoveToPush();
        _playerData.HasPushedCount++;
    }

    private void OnCollisionEnter(Collision collision)
    {
        PushCheck(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out IPushable target))
        {
            //make sure player is not behind you if you try to push while they touch you
            var heading = target.GetTransform().position - transform.position;
            var dot = Vector3.Dot(heading, target.GetTransform().forward);
            if(dot > 0.15f)
            {
                PushCheck(collision);
            }
        }
    }

    private void PushCheck(Collision collision)
    {
        if (IsPushing == false)
            return;

        if (collision.collider.gameObject.TryGetComponent(out IPushable target))
        {
            Vector3 dir = (collision.transform.position - transform.position).normalized;
            Instantiate(_collisionParticle, collision.GetContact(0).point, Quaternion.Euler(Vector3.zero));
            if (_movement.TryPush(target, dir, _movement.PushForce) == true)
            {
                _movement.EndPushing();
            }
        }
    }

    public void OnTaunt()
    {
        int r = UnityEngine.Random.Range(0, _tauntListSfx.Length);
        AudioManager.instance.PlaySFX(_tauntListSfx[r]);
    }

    public void OnBeingPushed(Vector3 dir, float force)
    {
        _movement.OnBeingPushed(dir, force);
        _playerData.BeenPushedCount++;
    }


    private void Update()
    {
        // Send move input to CharacterMovement component
        _movement.SetMoveInput(_moveInput);

        // find normalized look direction
        _movement.SetLookDirection(_moveInput);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
