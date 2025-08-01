using UnityEngine;

public class FruitIdleState : FruitBaseState
{
    public override void EnterState(FruitStateManager fruit)
    {
        // Logic for entering the idle state
    }

    public override void UpdateState(FruitStateManager fruit)
    {
        // Logic for updating the idle state
    }

    public override void OnCollisionEnter(FruitStateManager fruit, Collision collision)
    {
        // Logic for handling collisions in the idle state
    }

    public override void ExitState(FruitStateManager fruit)
    {
        // Logic for exiting the idle state
    }
}