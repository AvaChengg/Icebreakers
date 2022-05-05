using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;

public class RespawnTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private float timeRemaining;
     
    private void Start()
    {
        Observable.FromCoroutine(() => CountDownTimer())
            .Subscribe()
            .AddTo(this);

        //lock rotation and position when active
        gameObject.UpdateAsObservable()
            .Subscribe(_ =>
            { transform.rotation = Camera.main.transform.rotation;
            } );
    }

    private IEnumerator CountDownTimer()
    {
        while(timeRemaining > 0)
        {
            _text.text = timeRemaining.ToString();
            yield return new WaitForSeconds(1);
            timeRemaining--;
        }
    }

    public void SetRespawnTimer(Respawn respawn)
    {
        timeRemaining = respawn.RespawnDelay;
    }
}
