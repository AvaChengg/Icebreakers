using System.Collections;
using UnityEngine;
using UniRx;
using System;

public class BreakableBlockController : BaseBlockController
{
    [SerializeField] private float _respawnTimer = 30f;
    [SerializeField] private float _syncTimer = 0.25f;
    [SerializeField] private float _syncYDropValue = 5f;
    [SerializeField] private float _reappearTimer = 0.25f;
    [SerializeField] private Collider _breakTriggerCollider;

    private bool _isBroken = false;
    private Vector3 _originalDestination;

    private IDisposable _moveCo;

    protected override void OnAwake()
    {
        base.OnAwake();
        _originalDestination = transform.position;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (_isBroken == true)
            return;

        if(other.gameObject.TryGetComponent(out ITriggerBreak triggeringGameObject)){
            Break();
        }
    }

    private void Break()
    {
        _isBroken = true;

        //Disable collider so players falls through
        ToggleCollider(false);

        _moveCo?.Dispose();

        //Sync
        _moveCo = Observable.FromCoroutine(() => MoveBlock(new Vector3(transform.position.x, transform.position.y - _syncYDropValue, transform.position.z), _syncTimer)).Subscribe().AddTo(this);

        //Respawn coroutine
        Observable.FromCoroutine(() => RespawnBlock()).Subscribe().AddTo(this);
    }

    private IEnumerator MoveBlock(Vector3 destination, float duration)
    {
        Vector3 currentPostion = transform.position;
        float time = 0;
        while(time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(currentPostion, destination, time / duration);
            yield return null;
        }

        transform.position = destination;
    }

    private IEnumerator RespawnBlock()
    {
        yield return new WaitForSeconds(_respawnTimer);
        OnRespawn();
    }

    private void OnRespawn()
    {
        ToggleCollider(true);

        _moveCo?.Dispose();
        //appear
        _moveCo = Observable.FromCoroutine(() => MoveBlock(_originalDestination, _reappearTimer)).Subscribe().AddTo(this);
    }

    private void ToggleCollider(bool isEnabled)
    {
        if (_walkingCollider == null)
            return;

        _walkingCollider.enabled = isEnabled;
    }


}
