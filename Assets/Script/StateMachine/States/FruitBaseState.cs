using UnityEngine;

public abstract class FruitBaseState
{
    public abstract void EnterState(FruitStateManager fruit);

    public abstract void UpdateState(FruitStateManager fruit);

    public abstract void OnCollisionEnter(FruitStateManager fruit, Collision collision);

    public abstract void ExitState(FruitStateManager fruit);
}