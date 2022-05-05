using UnityEngine;

public interface IPushable : ITargetable
{
    void OnBeingPushed(Vector3 dir, float force);
    bool IsBeingPushed { get; }
}
