using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _acceleration = 10f;
    [SerializeField] private float _turnSpeed = 10f;
    [SerializeField] private float _slideDistance = 10f;
    [SerializeField] private float _slideSpeed = 2f;
    [SerializeField] private float _pushSpeed = 100f;
    [field: SerializeField] public float SpeedMultiplier { get; set; } = 1f;

    [Header("SFX")]
    [SerializeField] private AudioClip _pushingStartSFX;
    [SerializeField] private AudioClip _beingPushedSFX;
    [SerializeField] private AudioClip _jumpSFX;

    [Header("Jumping")]
    [SerializeField] private float _gravity = 10f;
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _airControl = 0.1f;
    [SerializeField] private float _jumpDelay = 2f;

    [Header("Pushing")]
    [SerializeField] private float _pushingDuration = 0.5f;
    [SerializeField] private float _pushForce = 200f;
    public float PushForce => _pushForce;

    [Header("On Being Pushed")]
    [SerializeField] private float _beingPushedDuration = 0.5f;

    // Check if grounding or not
    [Header("Grounding")]
    [SerializeField] private float _groundCheckRadius = 0.25f;
    [SerializeField] private float _groundCheckOffset = -0.4f;
    [SerializeField] private float _groundCheckDistance = 0.4f;
    [SerializeField] private LayerMask _groundMask;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _jumpParticle;

    // Public get, private set properties
    [field: SerializeField] public bool IsGrounded { get; private set; }

    public event Action<bool> OnIsPushingEnent;

    public Vector3 Velocity => _rigidbody.velocity;
    public float MaxSpeed => _moveSpeed;

    private Vector3 _moveInput;
    private Vector3 _lookDirection;
    private Vector3 _groundNormal = Vector3.up;
    private Rigidbody _rigidbody;

    //Pushing vars
    public bool IsPushing { get; private set; } = false;
    private IDisposable _pushingCo;

    //Is being pushedVars
    public bool IsBeingPushed { get; private set; } = false;
    private IDisposable _beingPushedCo;

    private void Awake()
    {
        // Configure rigidbody
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rigidbody.useGravity = false;

        // Set default look direction
        _lookDirection = transform.forward;
    }

    #region Debug

    private void OnDrawGizmosSelected()
    {
        // Set gizmos color
        Gizmos.color = Color.red;
        if (IsGrounded) Gizmos.color = Color.green;

        // Find start/end positions of spherecast
        Vector3 start = transform.position + Vector3.up * _groundCheckOffset;
        Vector3 end = start + Vector3.down * _groundCheckDistance;

        // Draw wire spheres
        Gizmos.DrawWireSphere(start, _groundCheckRadius);
        Gizmos.DrawWireSphere(end, _groundCheckRadius);

        // Draw ground normal
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, _groundNormal);
    }
    #endregion

    public void SetMoveInput(Vector3 moveInput)
    {
        _moveInput = moveInput;
    }

    public void SetLookDirection(Vector3 lookDirection)
    {
        //don't rotate if we are mid push
        if (IsPushing == true)
            return;

        if (lookDirection.magnitude < 0.01) return;
        _lookDirection = lookDirection;
    }

    public void TryJump()
    {
        // Return early if not grounded (stop immediately)
        if (IsGrounded == false || IsPushing == true || IsBeingPushed == true) 
            return;

        // Calculate jump velocity
        float jumpVelocity = Mathf.Sqrt(2f * _gravity * _jumpHeight);

        // Override y velocity of character
        Vector3 velocity = new Vector3(_rigidbody.velocity.x, jumpVelocity, _rigidbody.velocity.z);
        _rigidbody.velocity = velocity;

        AudioManager.instance?.PlaySFX(_jumpSFX);
        _jumpParticle.Play();
    }

    private bool CheckGrounded()
    {
        // Find start position
        Vector3 start = transform.position + Vector3.up * _groundCheckOffset;

        // Perform spherecast - start, radius, direction, hit info, distance, layermask
        if (Physics.SphereCast(start, _groundCheckRadius, Vector3.down, out RaycastHit hit, _groundCheckDistance, _groundMask))
        {
            _groundNormal = hit.normal;
            return true;
        }
        _groundNormal = Vector3.up;
        return false;
    }

    private void FixedUpdate()
    {
        // Check for ground
        IsGrounded = CheckGrounded();

        // Calculate velocity differential
        Vector3 currentVelocity = _rigidbody.velocity;
        Vector3 targetVelocity = _moveInput * _moveSpeed * SpeedMultiplier;
        Vector3 velocityDifferential = targetVelocity - currentVelocity;
        velocityDifferential.y = 0f;

        // Find air control modifier
        float airModifier = IsGrounded ? 1f : _airControl;
        // Calculate move force
        Vector3 moveForce = velocityDifferential * _acceleration * airModifier;
        // Add gravity
        moveForce += -_groundNormal * _gravity;

        // Apply acceleration
        _rigidbody.AddForce(moveForce);


        // Turn lookDirection into a rotation
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(_lookDirection); // LookRotation turns a direction into a rotation
        // Interpolate from current to target rotation
        Quaternion rotation = Quaternion.Slerp(currentRotation, targetRotation, _turnSpeed * Time.fixedDeltaTime);
        // Move to new rotation
        _rigidbody.MoveRotation(rotation);
    }

    #region Push
    public bool TryPush(IPushable target, Vector3 pushDirection, float force)
    {
        if (IsGrounded == false || target == null)
        {
            return false;
        }

        target.OnBeingPushed(pushDirection, force);
        return true;
    }

    public void OnBeingPushed(Vector3 dir, float force)
    {
        AudioManager.instance?.PlaySFX(_beingPushedSFX);

        OnBeingPushedEnd();
        _beingPushedCo = Observable.FromCoroutine(() => OnBeingPushedCo(dir, force)).Subscribe().AddTo(this);
    }

    private void OnBeingPushedEnd()
    {
        _beingPushedCo?.Dispose();
        IsBeingPushed = false;
    }

    private IEnumerator OnBeingPushedCo(Vector3 dir, float force)
    {

        //Stop velocity first of person being pushed (otherwise if they move while beign pushed they'll get thrown much further
        _rigidbody.velocity = Vector3.zero;

        _rigidbody.AddForce(new Vector3(dir.x, 0, dir.z) * force);
        yield return new WaitForSeconds(_beingPushedDuration);
        OnBeingPushedEnd();
    }

    public void MoveToPush()
    {
        if (IsGrounded == false || IsPushing == true || IsBeingPushed == true) 
            return;

        EndPushing();
        _pushingCo = Observable.FromCoroutine(() => MoveSlide(_pushingDuration,
                                                               _pushSpeed,
                                                              delegate {
                                                                  StartPushing();
                                                              },
                                                              delegate {
                                                                  EndPushing();
                                                              })).Subscribe().AddTo(this);
    }

    private void StartPushing()
    {
        IsPushing = true;
        AudioManager.instance?.PlaySFX(_pushingStartSFX);
        OnIsPushingEnent?.Invoke(IsPushing);
    }

    public void EndPushing()
    {
        _pushingCo?.Dispose();
        IsPushing = false;
        _rigidbody.velocity = Vector3.zero;
        OnIsPushingEnent?.Invoke(IsPushing);

    }

    private IEnumerator MoveSlide(float duration, float force, Action onStart, Action onComplete)
    {
        onStart?.Invoke();

        float time = 0;
        while(time < duration)
        {
            Vector3 velocity = _lookDirection * force;
            _rigidbody.velocity = velocity;
            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        onComplete?.Invoke();
    }
    #endregion
}
