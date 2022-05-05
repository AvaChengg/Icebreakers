using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [Header("Base Settings")]
    [Tooltip("Set to true if you want your singleton to exist across scenes.")]
    [SerializeField] private bool _dontDestroyOnLoad = false;

    private static T _instance;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                //Check if object was found in the scene.
                if (_instance == null)
                {
                    Debug.LogError($"Failed to find singleton of type {typeof(T)} in scene. Object does not exist.");
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(gameObject);
            return;
        }

        if (_dontDestroyOnLoad == true)
        {
            DontDestroyOnLoad(gameObject);
        }

        OnAwake();
    }

    //If you have anything your child class requires inside of Awake, add to the OnAwake override.
    protected virtual void OnAwake() { }
}