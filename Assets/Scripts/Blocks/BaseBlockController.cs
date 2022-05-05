using UnityEngine;

public abstract class BaseBlockController : MonoBehaviour, IBlock
{
    [SerializeField] protected MeshRenderer _meshRenderer;
    [SerializeField] protected Collider _walkingCollider;

    protected Material _originalMaterial;

    private void Awake()
    {
        OnAwake();
    }

    //Override this instead of unity Awake
    protected virtual void OnAwake()
    {
        CacheOriginalMaterial();
    }

    private void CacheOriginalMaterial()
    {
        //Get meshRenderer material if it can, if it's null look for the first one
        _originalMaterial = _meshRenderer.material ?? GetComponentInChildren<MeshRenderer>().material;
    }

    //Used for debug
    protected void RestoreOriginalMaterial()
    {
        //cache material of original. Used to swap from debug highlight
        if (_meshRenderer != null)
            _meshRenderer.material = _originalMaterial;
    }

}
