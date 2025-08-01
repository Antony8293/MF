
using UnityEngine;

public class FruitDraggingState : FruitBaseState
{
    public override void EnterState(FruitStateManager fruit)
    {
        // Logic for entering the dragging state
    }

    public override void UpdateState(FruitStateManager fruit)
    {
        // Logic for updating the dragging state
    }

    public override void OnCollisionEnter(FruitStateManager fruit, Collision collision)
    {
        // Logic for handling collisions in the dragging state
    }

    public override void ExitState(FruitStateManager fruit)
    {
        fruit._moveCircle.enabled = false; // Disable movement when exiting dragging state
        fruit.SwitchState(fruit.idleState); // Switch to idle state
    }

}