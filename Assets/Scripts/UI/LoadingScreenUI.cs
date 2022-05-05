using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using System.Collections;

public class LoadingScreenUI : Singleton<LoadingScreenUI>
{
    [Header("Tutorial")]
    [SerializeField] private RectTransform[] _tutorialObjects;
    [SerializeField] private float _delayPopup;

    [Header("Component References")]
    [SerializeField] private RectTransform _root;
    [SerializeField] private Button _buttonToContinue;
    [SerializeField] private TMP_Text _loadingPercent;
    [SerializeField] private Image _loadingFillBar;

    [Header("Loading Settings")]
    [Tooltip("Display the amount of decimal points for loading percent text. 0 means no decimal points.")]
    [SerializeField] private int _loadingPercentTextShowDecimals = 0;

    private bool _isContinue = false;

    protected override void OnAwake()
    {
        foreach (var item in _tutorialObjects)
        {
            item.localScale = Vector3.zero;
        }

        //Make sure root is not visible if left visible during dev
        _root.gameObject.SetActive(false);

        LoadingSceneController.OnLoadingPercentChangedEvent += DisplayLoadingPercent;
        LoadingSceneController.OnLoadingStateChangedEvent += OnLoadingSceneStateChanged;

        _buttonToContinue?.onClick?.AddListener(OnContinue);

        Observable.FromCoroutine(_ => ContinueCo()).Subscribe().AddTo(this);
    }

    private IEnumerator ContinueCo()
    {
        yield return new WaitForSeconds(10);
        OnContinue();
    }

    private IEnumerator PopupCo()
    {
        foreach (var item in _tutorialObjects)
        {
            Observable.FromCoroutine(() => ScaleCo(item, 0.5f)).Subscribe().AddTo(this);
            yield return new WaitForSeconds(_delayPopup);
        }
    }


    private IEnumerator ScaleCo(RectTransform rect, float duration)
    {
        float time = 0;
        Vector3 currentScale = rect.localScale;
        while(time < duration)
        {
            time += Time.deltaTime;
            Vector3 scale = Vector3.Lerp(currentScale, Vector3.one, time / duration);
            rect.localScale = scale;
            yield return null;
        }
        rect.localScale = Vector3.one;
    }

    private void DisplayLoadingPercent(float normalizedValue)
    {
        SetLoadingPrecentText(normalizedValue);
        SetLoadingBarFillAmoint(normalizedValue);
    }

    private void SetLoadingPrecentText(float normalizedValue)
    {
        if (_loadingPercent != null)
        {
            //display percent
            _loadingPercent.SetText($"{(normalizedValue * 100).ToString($"F{_loadingPercentTextShowDecimals}")}%");
        }
    }

    private void SetLoadingBarFillAmoint(float normalizedValue)
    {
        if (_loadingFillBar != null)
        {
            _loadingFillBar.fillAmount = normalizedValue;
        }
    }

    private void OnContinue()
    {
        if (_isContinue == true)
            return;

        _isContinue = true;
        MainEventHandler.AddToEventStream(new KeyPressEvent());
    }

    private void OnLoadingSceneStateChanged(ELoadingSceneState state)
    {
        switch (state)
        {
            case ELoadingSceneState.LoadingStarted:
                OnShow();
                break;

            case ELoadingSceneState.LoadingComplete:
                OnClose();
                break;
        }
    }

    private void OnShow()
    {
        _root.gameObject.SetActive(true);

        Observable.FromCoroutine(() => PopupCo()).Subscribe().AddTo(this);
    }

    private void OnClose()
    {
        Invoke("CloseDelay", 1);
        _isContinue = false;
    }

    private void CloseDelay()
    {
        _root.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _buttonToContinue?.onClick?.RemoveListener(OnContinue);

        LoadingSceneController.OnLoadingPercentChangedEvent -= DisplayLoadingPercent;
        LoadingSceneController.OnLoadingStateChangedEvent -= OnLoadingSceneStateChanged;
    }
}
