using UnityEngine;

public class SafeBlockController : BaseBlockController
{
    [SerializeField] private Material _debugHighlightMaterial;
    
    public void OnDebugPathToggle(bool isDebugOn)
    {
        if(isDebugOn == true)
        {
            DebugSafePathHighlight();
        }
        else
        {
            RestoreOriginalMaterial();
        }
    }

    private void DebugSafePathHighlight()
    {
        if (_debugHighlightMaterial == null || _meshRenderer == null)
            return;

        _meshRenderer.material = _debugHighlightMaterial;
    }


}
