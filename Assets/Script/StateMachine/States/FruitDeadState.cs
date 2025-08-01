using UnityEngine;

public class FruitDeadState : FruitBaseState
{
    public override void EnterState(FruitStateManager fruit)
    {
        // Logic for entering the dead state
    }

    public override void UpdateState(FruitStateManager fruit)
    {
        // Logic for updating the dead state
    }

    public override void OnCollisionEnter(FruitStateManager fruit, Collision collision)
    {
        // Logic for handling collisions in the dead state
    }

    public override void ExitState(FruitStateManager fruit)
    {
        // Logic for exiting the dead state
    }
}