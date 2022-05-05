using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UniRx;
using System.Threading.Tasks;

/// <summary>
/// A Unity scene load wrapper that does allows for async calls without using IEnumerators and/or exisiting in the scene. Events for your UI to easily hook into and display back loading percentage/states/etc
/// </summary>
public static class LoadingSceneController
{
    public static event Action<float> OnLoadingPercentChangedEvent;
    public static event Action<ELoadingSceneState> OnLoadingStateChangedEvent;

    private static bool _isFinishLoadRequested = false;
    private static IDisposable _loadFinishCo;

    #region Async Loading Functions
    /// <summary>
    /// Load a scene async by scene name.
    /// </summary>
    /// <param name="sceneName">The name of the scene.</param>
    /// <param name="onLoadingComplete">Called when loading is complete.</param>
    public static void LoadScene(string sceneName, Action onLoadingComplete = null)
    {

        LoadSceneAsync(sceneName, onLoadingComplete);
    }

    private static async void LoadSceneAsync(string sceneName, Action onComplete)
    {
        _isFinishLoadRequested = false;
        _loadFinishCo?.Dispose();

        //listen for keypress
        _loadFinishCo = MainEventHandler.EventStream<KeyPressEvent>().Subscribe(delegate { _isFinishLoadRequested = true; });

        OnLoadingPercentChangedEvent?.Invoke(0);
        OnLoadingStateChangedEvent?.Invoke(ELoadingSceneState.LoadingStarted);

        AsyncOperation asyncOperation = null;

        try
        {
            OnLoadingStateChangedEvent?.Invoke(ELoadingSceneState.LoadingInProgress);

            //Begin to load the Scene you specified, make sure the scene name exists and is in build settings
            asyncOperation = SceneManager.LoadSceneAsync(sceneName);

            //Don't let the Scene activate until you allow it to
            asyncOperation.allowSceneActivation = false;
        }
        catch (Exception e)
        {
            OnLoadingStateChangedEvent?.Invoke(ELoadingSceneState.LoadingFailed);
            //More than likely failed because scene name does not exist or because you did not add it to build settings
            Debug.LogError($"Failed to load scene with exception: {e}");
        }

        while (true)
        {
            OnLoadingPercentChangedEvent?.Invoke(asyncOperation.progress);

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f) //Do not change this value of 0.9f -> recommended by Unity
            {
                while(_isFinishLoadRequested == false)
                {
                    await Task.Delay(100);
                }

                asyncOperation.allowSceneActivation = true;
                break;
            }
        }
        
         OnLoadingPercentChangedEvent?.Invoke(1);
         OnLoadingStateChangedEvent?.Invoke(ELoadingSceneState.LoadingComplete);

        _isFinishLoadRequested = false;
        _loadFinishCo?.Dispose();

        //extra stuff?
        onComplete?.Invoke();
    }
    #endregion
}
