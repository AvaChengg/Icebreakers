using UnityEngine;

public class BlockDebugger : MonoBehaviour
{
    [SerializeField] private bool _isDisplaySafePaths = false;

    private void Awake()
    {
        #if UNITY_EDITOR
            if (_isDisplaySafePaths == true)
                OnToggleDisplaySafePaths(true);
        #endif
    }

    private void OnToggleDisplaySafePaths(bool isDisplayed)
    {
        //"Safe blocks' looks liek the breakable one, so we'll grba those for debuggign nad highligh that these are safe
        SafeBlockController[] safeBlocks = FindObjectsOfType<SafeBlockController>();
        foreach (var safeBlock in safeBlocks)
        {
            if (safeBlock != null)
                safeBlock.OnDebugPathToggle(isDisplayed);
        }
    }

}
