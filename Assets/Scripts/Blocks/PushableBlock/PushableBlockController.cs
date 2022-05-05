using System.Collections;
using UnityEngine;
using UniRx;

public class PushableBlockController : BaseBlockController, IPushable, IRespawn
{

    [SerializeField] private AudioClip _pushSfx;
    [SerializeField] private AudioClip _impactSfx;
    [SerializeField] private AudioClip _syncSfx;
    [SerializeField] private AudioClip _respawnSfx;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _pushForce = 2000;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float _groundCheckOffset;
    [SerializeField] private float _groundedCheckHalfExtents = 0.9f;
    [SerializeField] private float _respawnTimer = 4f;

    public bool IsBeingPushed => false;
    private bool _hasLanded = false;

    public Transform GetTransform()
    {
        return transform;
    }

    private void Update()
    {
        if (IsGrounded() == false)
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y,0);
        }
        else if(_hasLanded == false)
        {
            _hasLanded = true;
            AudioManager.instance?.PlaySFX(_impactSfx);
        }
    }

    public void OnBeingPushed(Vector3 dir, float force)
    {
        if (rigidbody == null)
            return;

        AudioManager.instance?.PlaySFX(_pushSfx);
        rigidbody.AddForce(dir * force);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IPushable target))
        {
            //make sure player is not behind you if you try to push while they touch you
            var heading = target.GetTransform().position - transform.position;
            var dot = Vector3.Dot(heading, target.GetTransform().forward);
            if (dot > 0.15f)
            {
                OnImpact((target.GetTransform().position - transform.position).normalized, collision.contacts[0].point, target);
            }
        }
    }

    private void OnImpact(Vector3 dir, Vector3 hitPoint, IPushable target)
    {
        if (target == null)
            return;

        target.OnBeingPushed((target.GetTransform().position - transform.position).normalized, _pushForce);
        AudioManager.instance?.PlaySFX(_impactSfx);
    }

    private bool IsGrounded()
    {
        // Find start position
        Vector3 start = transform.position + new Vector3(0, _groundCheckOffset, 0);

        // Perform spherecast - start, radius, direction, hit info, distance, layermask
        if (Physics.BoxCast(start, new Vector3(0.9f, _groundedCheckHalfExtents, 0.9f), Vector3.down, transform.rotation, 0.1f, _groundMask))
        {
            return true;
        }

        return false;
    }

    public void OnRespawn()
    {
        Observable.FromCoroutine(() => RespawnCo()).Subscribe().AddTo(this);
    }

    private IEnumerator RespawnCo()
    {
        AudioManager.instance?.PlaySFX(_syncSfx);
        yield return new WaitForSeconds(_respawnTimer);
        rigidbody.velocity = Vector3.zero;
        if (GameManager.instance != null)
        {
            Transform trans = GameManager.instance.GetRandomFallingBlockSpawnPoint();
            if (trans != null)
                transform.position = trans.position;
        }

        _hasLanded = false;
        AudioManager.instance?.PlaySFX(_respawnSfx);
    }

    
}
