using System.Collections;
using UnityEngine;
using UniRx;

public class PushableBlockSpawner : MonoBehaviour
{
    [SerializeField] private PushableBlockController _pushableBlockPrefab;
    [SerializeField] private float _minSpawnTime = 5;
    [SerializeField] private float _maxSpawnTime = 15;
    [SerializeField] private int _maxPushableBlocks = 5;

    private int _spawnedBlocks = 0;

    private void Awake()
    {
        if (_pushableBlockPrefab != null)
            return;

        Observable.FromCoroutine(() => SpawnPushableBlock()).Subscribe().AddTo(this);
    }

    private IEnumerator SpawnPushableBlock()
    {
        float randomTime = Random.Range(_minSpawnTime, _maxSpawnTime);  
        yield return new WaitForSeconds(randomTime);

        if(GameManager.instance != null)
        {
            Transform randomTrans = GameManager.instance.GetRandomFallingBlockSpawnPoint();
            if(randomTrans != null && _spawnedBlocks < _maxPushableBlocks)
            {
                Instantiate(_pushableBlockPrefab, randomTrans.position, Quaternion.identity);
                _spawnedBlocks++;
            }
    
        }    
       
    }
}
