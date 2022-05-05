using System.Collections;
using UnityEngine;
using UniRx;
using System;

public class ShootingBlockController : BaseBlockController, IPushable, IRespawn
{
    [SerializeField] private AudioClip _impactSfx;
    [SerializeField] private AudioClip _pushSfx;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _respawnTimer = 15f;
    [SerializeField] private float _pushForce = 1000f;
    [SerializeField] private float _speed = 50;
    [SerializeField] private float _randomZRespawnOffset = 10;

    private bool _isBeingPushed = true;
    public bool IsBeingPushed => _isBeingPushed;

    private IDisposable _respawnCo;
    private IDisposable _timedLifeCo;

    public Transform GetTransform()
    {
        return transform;
    }

    private void Start()
    {
        TimedLife();
    }

    private void FixedUpdate()
    {
        if(_isBeingPushed == true)
            _rigidbody.AddForce(_speed * transform.forward);
    }

    private void TimedLife()
    {
        _timedLifeCo?.Dispose();
        _timedLifeCo = Observable.FromCoroutine(() => TimedLifeCo()).Subscribe().AddTo(this);
    }

    private IEnumerator TimedLifeCo()
    {
        yield return new WaitForSeconds(15f);
        OnRespawn();
    }

    public void OnBeingPushed(Vector3 dir, float force)
    {
        if (_rigidbody == null)
            return;

        AudioManager.instance?.PlaySFX(_pushSfx);
        _rigidbody.AddForce(dir * force);
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
                OnImpact((target.GetTransform().position - transform.position).normalized, collision.contacts[0].point, target, _pushForce);
            }
        }
    }

    private void OnImpact(Vector3 dir, Vector3 hitPoint, IPushable target, float force)
    {
        if (target == null)
            return;

        target.OnBeingPushed((target.GetTransform().position - transform.position).normalized, force);
        AudioManager.instance?.PlaySFX(_impactSfx);
    }

    public void OnRespawn()
    {
        _respawnCo?.Dispose();
        _respawnCo = Observable.FromCoroutine(() => RespawnCo()).Subscribe().AddTo(this);
    }

    private IEnumerator RespawnCo()
    {
        _isBeingPushed = false;
        yield return new WaitForSeconds(_respawnTimer);
        _rigidbody.velocity = Vector3.zero;
        transform.position = GetRespawnSpot();
        _isBeingPushed = true;

        TimedLife();
    }

    private Vector3 GetRespawnSpot()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        pos.y = 2.1f;
        float rZ = UnityEngine.Random.Range(-13, _randomZRespawnOffset);
        pos.z = rZ;
        pos.x = 40f;
        return pos;
    }


}
